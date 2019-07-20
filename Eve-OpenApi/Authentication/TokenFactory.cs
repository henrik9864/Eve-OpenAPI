using EveOpenApi.Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EveOpenApi.Authentication
{
	internal class TokenFactory : ITokenFactory
	{
		public IToken Create(string accessToken, string refreshToken, uint expires, string tokenType, IScope scope)
		{
			OauthToken token = new OauthToken(accessToken, refreshToken, expires, tokenType);
			return new Token(token, scope);
		}

		public IToken FromJson(string json, IScope scope)
		{
			OauthToken token = JsonSerializer.Parse<OauthToken>(json);
			return new Token(token, scope);
		}
	}
}
