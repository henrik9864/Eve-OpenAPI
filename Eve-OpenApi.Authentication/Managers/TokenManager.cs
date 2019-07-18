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
			string state = RandomString(8);
			string authUrl = GenerateAuthUrl(scope.ScopeString, state);

			AuthResponse response =  await responseManager.GetResponse(authUrl, 10000);

			if (state != response.State)
				throw new Exception("Invalid auth response state.");

			IToken token = await GenerateToken(response);
			IJwtToken jwtToken = await validationManager.ValidateTokenAsync(token);

			return (token, jwtToken.Name);
		}

		public (string authUrl, string state) GenerateAuthUrl(IScope scope)
		{
			string state = RandomString(8);
			string url = GenerateAuthUrl(scope.ScopeString, state);

			return (url, state);
		}

		public async Task<(IToken token, string owner)> ListenForResponse(IScope scope, string state)
		{
			AuthResponse response = await responseManager.AwaitResponse(20000);

			if (state != response.State)
				throw new Exception("Invalid auth response state.");

			IToken token = await GenerateToken(response);
			IJwtToken jwtToken = await validationManager.ValidateTokenAsync(token);

			return (token, jwtToken.Name);
		}

		async Task<IToken> GenerateToken(AuthResponse response)
		{
			var tokenRequest = GenerateTokenRequest(response.Code);
			var tokenResponse = await client.SendAsync(tokenRequest);

			Stream tokenStream = await tokenResponse.Content.ReadAsStreamAsync();
			return await JsonSerializer.ReadAsync<Token>(tokenStream);
		}

		string GenerateAuthUrl(string scope, string state)
		{
			return $"{config.AuthenticationEndpoint}?" +
				$"response_type=code&" +
				$"client_id={credentials.ClientID}&" +
				$"redirect_uri={Uri.EscapeDataString(credentials.Callback)}&" +
				$"scope={Uri.EscapeDataString(scope)}&" +
				$"state={Uri.EscapeDataString(state)}";
		}

		HttpRequestMessage GenerateTokenRequest(string code)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{config.TokenEndpoint}");
			Dictionary<string, string> data = new Dictionary<string, string>()
			{
				{ "grant_type", "authorization_code" },
				{ "code", code }
			};

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
	}
}
