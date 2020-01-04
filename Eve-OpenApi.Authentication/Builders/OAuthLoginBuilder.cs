using EveOpenApi.Authentication.Interfaces;
using EveOpenApi.Api.Factories;
using EveOpenApi.Authentication;
using EveOpenApi.Authentication.Managers;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;

namespace EveOpenApi.Authentication
{
	public class OAuthLoginBuilder
	{
		public ILoginConfig Config { get; private set; }

		public ILoginCredentials Credentials { get; private set; }

		private List<TokenSave> Tokens { get; set; }

		public OAuthLoginBuilder()
		{
			Tokens = new List<TokenSave>();
		}

		public OAuthLoginBuilder WithConfig(ILoginConfig config)
		{
			Config = config;
			return this;
		}

		/// <summary>
		/// Without the client secret the library will use the PKCE protocol
		/// </summary>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public OAuthLoginBuilder WithCredentials(string clientID, string callback)
		{
			Credentials = new OauthLoginCredentials()
			{
				ClientID = clientID,
				Callback = callback,
			};

			return this;
		}

		public OAuthLoginBuilder WithCredentials(string clientID, string clientSecret, string callback)
		{
			Credentials = new OauthLoginCredentials()
			{
				ClientID = clientID,
				ClientSecret = clientSecret,
				Callback = callback
			};

			return this;
		}

		public OAuthLoginBuilder FromFileEncrypted(string path)
		{
			string encryptedJson = File.ReadAllText(path);

			return FromEncrypted(encryptedJson);
		}

		public OAuthLoginBuilder FromEncrypted(string encryptedJson)
		{
			string passPhrase = string.IsNullOrEmpty(Credentials.ClientSecret) ? Credentials.ClientID : Credentials.ClientSecret;
			string json = StringCipher.Decrypt(encryptedJson, passPhrase);

			return FromString(json);
		}

		public OAuthLoginBuilder FromFile(string path)
		{
			string encryptedJson = File.ReadAllText(path);

			return FromString(encryptedJson);
		}

		public OAuthLoginBuilder FromString(string json)
		{
			(string type, List<TokenSave> tokens) = JsonSerializer.Deserialize<(string, List<TokenSave>)>(json);

			if (type != "OAuth")
				throw new Exception($"Unknown token types '{type}' expected type 'OAuth'");

			Tokens = tokens;

			return this;
		}

		public async Task<IOauthLogin> Build()
		{
			if (Config is null)
				throw new NullReferenceException("Config cannot be null");

			if (Credentials is null)
				throw new NullReferenceException("Credentials cannot be null");

			IHttpHandler httpHandler = new HttpHandler();
			IOauthTokenFactory tokenFactory = new OauthTokenFactory();
			IFactory<IAuthResponse> authResponseFactory = new AuthResponseFactory();

			IResponseManager responseManager = new ResponseManager(Credentials, authResponseFactory);
			IValidationManager validationManager = new ValidationManager(Config, httpHandler);
			ITokenManager tokenManager = new TokenManager(Config, Credentials, responseManager, validationManager, tokenFactory, httpHandler);

			IOauthLogin login = new OauthLogin(Config, Credentials, tokenManager);
			for (int i = 0; i < Tokens.Count; i++) // Read all tokens from saved file. It just felt so right to have a comment there
				await login.AddToken(Tokens[i].RefreshToken, (Scope)Tokens[i].Scope);

			return login;
		}
	}
}
