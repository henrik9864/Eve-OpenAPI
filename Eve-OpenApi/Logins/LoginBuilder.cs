using EveOpenApi.Eve;
using EveOpenApi.Interfaces;
using EveOpenApi.Logins.Eve.Client;
using EveOpenApi.Logins.Eve.Web;
using EveOpenApi.Logins.Seat;
using EveOpenApi.Seat;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi
{
	public class LoginBuilder
	{
		IHttpHandler client;

		public LoginBuilder()
		{
			client = new HttpHandler();
		}

		public IClientLogin BuildEve(string clientID, string callback)
		{
			IEveLoginConfig config = new EveLoginConfig(clientID, callback);
			ILoginSetup loginSetup = new EveLoginSetup();
			ITokenFactoryAsync<EveToken> tokenFactory = new EveTokenFactory(client);

			return new EveLogin(config, loginSetup, tokenFactory, client);
		}

		public IWebLogin BuildEve(string clientID, string clientSecret, string callback)
		{
			IEveWebLoginConfig config = new EveWebLoginConfig(clientID, clientSecret, callback);
			ILoginSetup loginSetup = new EveLoginSetup();
			ITokenFactoryAsync<EveToken> tokenFactory = new EveWebTokenFactory(client);

			return new EveWebLogin(config, loginSetup, tokenFactory, client);
		}

		public ILogin BuildSeat(string user, string token)
		{
			ISeatLoginConfig config = new SeatLoginConfig(user, token);
			ILoginSetup loginSetup = new SeatLoginSetup();
			ITokenFactory<SeatToken> tokenFactory = new SeatTokenFactory();

			return new SeatLogin(config, loginSetup, tokenFactory);
		}
	}
}
