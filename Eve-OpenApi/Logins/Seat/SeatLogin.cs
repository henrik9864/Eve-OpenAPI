using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi
{
	public class SeatLogin : ILogin
	{
		public IInterfaceSetup Setup { get; }

		public IToken this[string scope]
		{
			get
			{
				return GetToken((Scope)scope);
			}
		}

		List<IToken> tokens;

		public SeatLogin(string token)
		{
			Setup = new SeatInterfaceSetup();
			tokens = new List<IToken>();
			tokens.Add(new SeatToken(token));
		}

		public IToken GetToken(IScope scope)
		{
			IToken token = tokens.Find(a => a.Scope.IsSubset(scope));

			if (token == null)
				throw new Exception($"No token with scope '{scope}' found");

			return token;
		}

		public bool TryGetToken(IScope scope, out IToken token)
		{
			token = tokens.Find(a => a.Scope.IsSubset(scope));
			return token != null;
		}

		public async Task<IToken> AddToken(IScope scope)
		{
			await Task.CompletedTask;
			throw new Exception("Unable to create a new SeAT token");
		}
	}
}
