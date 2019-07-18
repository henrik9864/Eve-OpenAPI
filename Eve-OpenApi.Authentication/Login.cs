using EveOpenApi.Authentication.Managers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public class Login : ILogin
	{
		public ILoginConfig Config { get; }

		public ILoginCredentials Credentials { get; }

		ITokenManager tokenManager;

		Dictionary<string, List<IToken>> userTokens;

		internal Login(ILoginConfig config, ILoginCredentials credentials, ITokenManager tokenManager)
		{
			Config = config;
			Credentials = credentials;
			this.tokenManager = tokenManager;

			userTokens = new Dictionary<string, List<IToken>>();
		}

		public async Task<IToken> AddToken(IScope scope)
		{
			var result = await tokenManager.GetToken(scope);
			AddToken(result.owner, result.token);

			return result.token;
		}

		public async Task<string> GetAuthUrl(IScope scope)
		{
			var auth = tokenManager.GenerateAuthUrl(scope);

			await Task.Factory.StartNew(async () =>
			{
				var response = await tokenManager.ListenForResponse(scope, auth.state);
				AddToken(response.owner, response.token);

				Console.WriteLine("Yay");
			});

			return auth.authUrl;
		}

		void AddToken(string owner, IToken token)
		{
			if (!userTokens.TryGetValue(owner, out List<IToken> tokens))
			{
				tokens = new List<IToken>();
				userTokens[owner] = tokens;
			}

			tokens.Add(token);
		}
	}
}
