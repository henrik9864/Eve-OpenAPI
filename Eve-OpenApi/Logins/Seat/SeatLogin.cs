using EveOpenApi.Interfaces;
using EveOpenApi.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi
{
	public class SeatLogin : ILogin
	{
		public ILoginSetup LoginSetup { get; }

		public ISeatLoginConfig LoginConfig { get; }

		public IToken this[string user, string scope]
		{
			get
			{
				return GetToken(user, (Scope)scope);
			}
		}

		Dictionary<string, List<IToken>> userTokens;
		ITokenFactory<SeatToken> tokenFactory;

		internal SeatLogin(ISeatLoginConfig loginConfig, ILoginSetup loginSetup, ITokenFactory<SeatToken> tokenFactory)
		{
			LoginSetup = loginSetup;
			LoginConfig = loginConfig;
			this.tokenFactory = tokenFactory;

			userTokens = new Dictionary<string, List<IToken>>();
			AddToken(LoginConfig.User, loginConfig.Token);
		}

		public IToken GetToken(string user, IScope scope)
		{
			IToken token = userTokens[user].Find(a => a.Scope.IsSubset(scope));

			if (token == null)
				throw new Exception($"No token with scope '{scope}' found");

			return token;
		}

		public bool TryGetToken(string user, IScope scope, out IToken token)
		{
			token = userTokens[user].Find(a => a.Scope.IsSubset(scope));
			return token != null;
		}

		public IList<string> GetUsers()
		{
			return userTokens.ToList()
				.ConvertAll(x => x.Key);
		}

		public IList<IToken> GetTokens(string user)
		{
			return userTokens[user];
		}

		void AddToken(string user, string token)
		{
			if (!userTokens.TryGetValue(user, out List<IToken> tokens))
			{
				tokens = new List<IToken>();
				userTokens.Add(user, tokens);
			}

			tokens.Add(tokenFactory.CreateToken(token, user));
		}
	}
}
