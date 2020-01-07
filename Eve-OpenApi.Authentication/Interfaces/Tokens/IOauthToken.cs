using EveOpenApi.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication.Interfaces
{
	public interface IOauthToken : IToken
	{
		string AccessToken { get; }

		string RefreshToken { get; }

		string TokenType { get; }

		DateTime Expires { get; }
	}
}
