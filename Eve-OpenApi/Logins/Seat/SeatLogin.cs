using EveOpenApi.Interfaces;
using EveOpenApi.Seat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi
{
	public class SeatLogin : ILogin
	{
		public IInterfaceSetup Setup { get; }

		public IToken this[string user, string scope]
		{
			get
			{
				return GetToken(user, (Scope)scope);
			}
		}

		Dictionary<string, List<IToken>> userTokens;

		public SeatLogin(string user, string token)
		{
			Setup = new SeatInterfaceSetup();
			userTokens = new Dictionary<string, List<IToken>>();
			AddToken(user, token);
		}

		public void AddToken(string user, string token)
		{
			if (!userTokens.TryGetValue(user, out List<IToken> tokens))
			{
				tokens = new List<IToken>();
				userTokens.Add(user, tokens);
			}

			tokens.Add(new SeatToken(token));
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
	}
}
