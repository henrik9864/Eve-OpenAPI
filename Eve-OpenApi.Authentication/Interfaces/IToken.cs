using System;

namespace EveOpenApi.Authentication
{
	public interface IToken
	{
		string AccessToken { get; }

		string RefreshToken { get; }

		IScope Scope { get; }

		DateTime Expires { get; }

		string TokenType { get; }
	}
}