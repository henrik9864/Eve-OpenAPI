using EveOpenApi.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	partial class Token : IToken
	{
		public string AccessToken { get; }

		public string RefreshToken { get; }

		public IScope Scope { get; }

		public DateTime Expires { get; }

		public string TokenType { get; }

		public Token(OauthToken token, IScope scope)
		{
			AccessToken = token.AccessToken;
			RefreshToken = token.RefreshToken;
			Scope = scope;
			Expires = DateTime.Now + new TimeSpan(0, 0, (int)token.Expires);
			TokenType = token.TokenType;
		}
	}
}
