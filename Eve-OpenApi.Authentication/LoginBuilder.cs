using EveOpenApi.Authentication;
using EveOpenApi.Authentication.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	public class LoginBuilder
	{
		public ILoginConfig Config { get; private set; }

		public ILoginCredentials Credentials { get; private set; }

		public LoginBuilder()
		{
		}

		public LoginBuilder(ILoginConfig config)
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

		public ILogin BuildOauth()
		{
			if (Config is null)
				throw new NullReferenceException("Config cannot be null");

			if (Credentials is null)
				throw new NullReferenceException("Credentials cannot be null");

			IHttpHandler httpHandler = new HttpHandler();
			IResponseManager responseManager = new ResponseManager(Credentials);
			IValidationManager validationManager = new ValidationManager(Config, httpHandler);
			ITokenManager tokenManager = new TokenManager(Config, Credentials, responseManager, validationManager, httpHandler);

			return new OauthLogin(Config, Credentials, tokenManager);
		}

		public ILogin BuildEve()
		{
			Config = LoginConfig.Eve;
			return BuildOauth();
		}
	}
}
