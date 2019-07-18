using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication.Managers
{
	internal class TokenManager : ITokenManager
	{
		ILoginConfig config;
		ILoginCredentials credentials;
		IHttpHandler client;

		IResponseManager responseManager;
		IValidationManager validationManager;

		public TokenManager(ILoginConfig config, ILoginCredentials credentials, IResponseManager responseManager, IValidationManager validationManager, IHttpHandler client)
		{
			this.config = config;
			this.credentials = credentials;
			this.responseManager = responseManager;
			this.validationManager = validationManager;
			this.client = client;
		}

		public async Task<(IToken token, string owner)> GetToken(IScope scope)
		{
			AuthUrl authUrl = GenerateAuthUrl(scope.ScopeString);
			AuthResponse response =  await responseManager.GetResponse(authUrl.Url, 10000);

			if (authUrl.State != response.State)
				throw new Exception("Invalid auth response state.");

			IToken token = await GenerateToken(response, authUrl);
			IJwtToken jwtToken = await validationManager.ValidateTokenAsync(token);

			return (token, jwtToken.Name);
		}

		public AuthUrl GenerateAuthUrl(IScope scope)
		{
			return GenerateAuthUrl(scope.ScopeString);
		}

		public async Task<(IToken token, string owner)> ListenForResponse(IScope scope, AuthUrl authUrl)
		{
			AuthResponse response = await responseManager.AwaitResponse(20000);

			if (authUrl.State != response.State)
				throw new Exception("Invalid auth response state.");

			IToken token = await GenerateToken(response, authUrl);
			IJwtToken jwtToken = await validationManager.ValidateTokenAsync(token);

			return (token, jwtToken.Name);
		}

		async Task<IToken> GenerateToken(AuthResponse response, AuthUrl authUrl)
		{
			var tokenRequest = GenerateTokenRequest(response.Code, authUrl.CodeVerifier);
			var tokenResponse = await client.SendAsync(tokenRequest);

			Stream tokenStream = await tokenResponse.Content.ReadAsStreamAsync();
			return await JsonSerializer.ReadAsync<Token>(tokenStream);
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

		HttpRequestMessage GenerateTokenRequest(string code, string codeVerifier)
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
