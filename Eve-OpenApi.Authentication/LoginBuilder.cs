using EveOpenApi.Authentication;
using EveOpenApi.Authentication.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	public class LoginBuilder
	{
		public ILoginConfig Config { get; }

		public ILoginCredentials Credentials { get; private set; }

		public LoginBuilder(ILoginConfig config)
		{
			Config = config;
		}

		public LoginBuilder WithCredentials(string clientID, string callback)
		{
			Credentials = new LoginCredentials()
			{
				ClientID = clientID,
				Callback = callback,
			};

			return this;
		}

		public LoginBuilder WithCredentials(string clientID, string clientSecret, string callback, string authType)
		{
			Credentials = new LoginCredentials()
			{
				ClientID = clientID,
				ClientSecret = clientSecret,
				Callback = callback,
				AuthType = authType
			};

			return this;
		}

		public ILogin Build()
		{
			IHttpHandler httpHandler = new HttpHandler();
			IResponseManager responseManager = new ResponseManager(Credentials);
			IValidationManager validationManager = new ValidationManager(Config, httpHandler);
			ITokenManager tokenManager = new TokenManager(Config, Credentials, responseManager, validationManager, httpHandler);
			return new Login(Config, Credentials, tokenManager);
		}
	}
}
