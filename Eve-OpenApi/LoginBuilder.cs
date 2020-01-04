using EveOpenApi.Api.Factories;
using EveOpenApi.Authentication;
using EveOpenApi.Authentication.Interfaces;
using EveOpenApi.Authentication.Managers;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public class LoginBuilder
	{
		public ILoginConfig Config { get; private set; }

		public ILoginCredentials Credentials { get; private set; }

		private List<TokenSave> Tokens { get; set; }

		public LoginBuilder()
		{
			Tokens = new List<TokenSave>();
		}

		public LoginBuilder(ILoginConfig config) : this()
		{
			Config = config;
		}

		/// <summary>
		/// Without the client secret the library will use the PKCE protocol
		/// </summary>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public LoginBuilder WithCredentials(string clientID, string callback)
		{
			Credentials = new LoginCredentials()
			{
				ClientID = clientID,
				Callback = callback,
			};

			return this;
		}

		public LoginBuilder WithCredentials(string clientID, string clientSecret, string callback)
		{
			Credentials = new LoginCredentials()
			{
				ClientID = clientID,
				ClientSecret = clientSecret,
				Callback = callback
			};

			return this;
		}

		public LoginBuilder FromFile(string path)
		{
			string passPhrase = string.IsNullOrEmpty(Credentials.ClientSecret) ? Credentials.ClientID : Credentials.ClientSecret;
			string encryptedJson = File.ReadAllText(path);

			string json = StringCipher.Decrypt(encryptedJson, passPhrase);
			Tokens = JsonSerializer.Deserialize<List<TokenSave>>(json);

			return this;
		}

		public async Task<ILogin> BuildOauth()
		{
			if (Config is null)
				throw new NullReferenceException("Config cannot be null");

			if (Credentials is null)
				throw new NullReferenceException("Credentials cannot be null");

			IHttpHandler httpHandler = new HttpHandler();
			ITokenFactory tokenFactory = new TokenFactory();
			IFactory<IAuthResponse> authResponseFactory = new AuthResponseFactory();

			IResponseManager responseManager = new ResponseManager(Credentials, authResponseFactory);
			IValidationManager validationManager = new ValidationManager(Config, httpHandler);
			ITokenManager tokenManager = new TokenManager(Config, Credentials, responseManager, validationManager, tokenFactory, httpHandler);

			ILogin login = new OauthLogin(Config, Credentials, tokenManager);
			for (int i = 0; i < Tokens.Count; i++) // Readd all tokens from saved file.
				await login.AddToken(Tokens[i].RefreshToken, (Scope)Tokens[i].Scope);

			return login;
		}

		public Task<ILogin> BuildEve()
		{
			Config = LoginConfig.Eve;
			return BuildOauth();
		}
	}
}
