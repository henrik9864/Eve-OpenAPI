using EveOpenApi.Authentication.Interfaces;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Authentication.Managers
{
	internal class TokenManager : ITokenManager
	{
		ILoginConfig config;
		ILoginCredentials credentials;
		IHttpHandler client;

		IResponseManager responseManager;
		IValidationManager validationManager;

		ITokenFactory tokenFactory;

		public TokenManager(ILoginConfig config, ILoginCredentials credentials, IResponseManager responseManager, IValidationManager validationManager, ITokenFactory tokenFactory, IHttpHandler client)
		{
			this.config = config;
			this.credentials = credentials;
			this.responseManager = responseManager;
			this.validationManager = validationManager;
			this.tokenFactory = tokenFactory;
			this.client = client;
		}

		public async Task<(IToken token, string owner)> GetToken(IScope scope)
		{
			AuthUrl authUrl = GenerateAuthUrl(scope.ScopeString);
			IAuthResponse response =  await responseManager.GetResponse(authUrl.Url, 10000);

			if (authUrl.State != response.State)
				throw new Exception("Invalid auth response state.");

			IToken token = await GetToken(response, authUrl, scope);
			IJwtToken jwtToken = await validationManager.ValidateTokenAsync(token);

			return (token, jwtToken.Name);
		}

		public async Task<(IToken token, string owner)> RefreshToken(string refreshToken, IScope scope)
		{
			IToken token = await GetToken(refreshToken, scope);
			IJwtToken jwtToken = await validationManager.ValidateTokenAsync(token);

			return (token, jwtToken.Name);
		}

		public AuthUrl GenerateAuthUrl(IScope scope)
		{
			return GenerateAuthUrl(scope.ScopeString);
		}

		public async Task<(IToken token, string owner)> ListenForResponse(IScope scope, AuthUrl authUrl)
		{
			IAuthResponse response = await responseManager.AwaitResponse(20000);

			if (authUrl.State != response.State)
				throw new Exception("Invalid auth response state.");

			IToken token = await GetToken(response, authUrl, scope);
			IJwtToken jwtToken = await validationManager.ValidateTokenAsync(token);

			return (token, jwtToken.Name);
		}

		/// <summary>
		/// Creates a token from the SSO response
		/// </summary>
		/// <param name="response"></param>
		/// <param name="authUrl"></param>
		/// <param name="scope"></param>
		/// <returns></returns>
		async Task<IToken> GetToken(IAuthResponse response, AuthUrl authUrl, IScope scope)
		{
			var tokenRequest = GetTokenRequest(response.Code, authUrl.CodeVerifier);
			var tokenResponse = await client.SendAsync(tokenRequest);

			string json = await tokenResponse.Content.ReadAsStringAsync();
			return tokenFactory.FromJson(json, scope);
		}

		async Task<IToken> GetToken(string refreshToken, IScope scope)
		{
			var tokenRequest = GetRefreshTokenRequest(refreshToken, scope.ScopeString);
			var tokenResponse = await client.SendAsync(tokenRequest);

			string json = await tokenResponse.Content.ReadAsStringAsync();
			return tokenFactory.FromJson(json, scope);
		}

		AuthUrl GenerateAuthUrl(string scope)
		{
			string state = RandomString(8);
			string codeVerifier = GetCodeVerifier();

			string url = $"{config.AuthenticationEndpoint}?" +
				$"response_type=code&" +
				$"client_id={credentials.ClientID}&" +
				$"redirect_uri={Uri.EscapeDataString(credentials.Callback)}&" +
				$"scope={Uri.EscapeDataString(scope)}&" +
				$"state={Uri.EscapeDataString(state)}";

			if (string.IsNullOrEmpty(credentials.ClientSecret))
				url += $"&code_challenge={GenerateCodeChallenge(codeVerifier)}&code_challenge_method=S256";

			return new AuthUrl(url, state, codeVerifier);
		}

		HttpRequestMessage GetTokenRequest(string code, string codeVerifier)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{config.TokenEndpoint}");
			Dictionary<string, string> data = new Dictionary<string, string>()
			{
				{ "grant_type", "authorization_code" },
				{ "code", code }
			};

			if (string.IsNullOrEmpty(credentials.ClientSecret))
				AddPKCE(codeVerifier, ref data);
			else
				AddAuthorization(config.AuthType, ref data, ref request);

			request.Content = new FormUrlEncodedContent(data.ToArray());
			return request;
		}

		HttpRequestMessage GetRefreshTokenRequest(string refreshToken, string scope)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{config.TokenEndpoint}");
			Dictionary<string, string> data = new Dictionary<string, string>()
			{
				{ "grant_type", "refresh_token" },
				{ "refresh_token", refreshToken },
				{ "scope", scope },
				{ "client_id", credentials.ClientID }
			};

			request.Content = new FormUrlEncodedContent(data.ToArray());
			return request;
		}

		void AddAuthorization(string type, ref Dictionary<string, string> data, ref HttpRequestMessage request)
		{
			switch (type)
			{
				case "":
					data.Add("redirect_uri", credentials.Callback);
					data.Add("client_id", credentials.ClientID);
					data.Add("client_secret", credentials.ClientSecret);
					break;
				case "basic":
					string endodedString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{credentials.ClientID}:{credentials.ClientSecret}"));
					request.Headers.TryAddWithoutValidation("Authorization", $"Basic {endodedString}");
					break;
				default:
					throw new Exception($"Unknwon authorizaiton type '{type}'");
			}
		}

		/// <summary>
		/// Add PKCE query parameters
		/// </summary>
		/// <param name="codeVerifier"></param>
		/// <param name="data"></param>
		void AddPKCE(string codeVerifier, ref Dictionary<string, string> data)
		{
			data.Add("redirect_uri", credentials.Callback);
			data.Add("client_id", credentials.ClientID);
			data.Add("code_verifier", codeVerifier);
		}

		/// <summary>
		/// Generate random string of X length.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		static string RandomString(int length)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
			char[] stringChars = new char[length];
			using (var numberGen = new RNGCryptoServiceProvider())
			{
				for (int i = 0; i < length; i++)
				{
					stringChars[i] = chars[RandomNumber(numberGen, chars.Length)];
				}
			}

			return new string(stringChars);
		}

		/// <summary>
		/// Generate a random number in range.
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="range"></param>
		/// <returns></returns>
		static int RandomNumber(RNGCryptoServiceProvider generator, int range)
		{
			byte[] randomBytes = new byte[2];
			generator.GetBytes(randomBytes);
			int randomNumber = BitConverter.ToUInt16(randomBytes, 0);
			return (int)Math.Floor((randomNumber / 65536f) * range);
		}

		/// <summary>
		/// Generate a 32 byte code for Auth0
		/// </summary>
		/// <returns></returns>
		static string GetCodeVerifier()
		{
			string randomString = RandomString(32); // Used to make the code challenge and verifier
			byte[] verifierBytes = System.Text.Encoding.UTF8.GetBytes(randomString);

			return UrlSafeBase64(verifierBytes);
		}

		/// <summary>
		/// Generate a code challenge from the 32 byte code
		/// </summary>
		/// <param name="codeVerifier"></param>
		/// <returns></returns>
		static string GenerateCodeChallenge(string codeVerifier)
		{
			byte[] verifierBytes = System.Text.Encoding.UTF8.GetBytes(codeVerifier);
			byte[] hash;
			using (SHA256 myHashGen = SHA256.Create())
			{
				hash = myHashGen.ComputeHash(verifierBytes);
			}

			return UrlSafeBase64(hash);
		}

		/// <summary>
		/// URL safe encode bytes to base64
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		static string UrlSafeBase64(byte[] bytes)
		{
			char[] padding = { '=' };
			return Convert.ToBase64String(bytes).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
		}
	}
}
