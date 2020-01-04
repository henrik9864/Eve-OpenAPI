using EveOpenApi.Authentication.Interfaces;
using EveOpenApi.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	internal class OauthToken : IOauthToken
	{
		public string AccessToken { get; }

		public string RefreshToken { get; }

		public IScope Scope { get; }

		public DateTime Expires { get; }

		public string TokenType { get; }

		public OauthToken(OauthTokenSave token, IScope scope)
		{
			AccessToken = token.AccessToken;
			RefreshToken = token.RefreshToken;
			Scope = scope;
			Expires = DateTime.Now + new TimeSpan(0, 0, (int)token.Expires);
			TokenType = token.TokenType;
		}

		public string GetToken()
		{
			return AccessToken;
		}
	}
}
