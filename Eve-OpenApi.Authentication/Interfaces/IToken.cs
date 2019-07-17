using System;

namespace EveOpenApi.Authentication
{
	public interface IToken
	{
		string AccessToken { get; }

		string RefreshToken { get; }

		uint Expires { get; }

		string TokenType { get; }
	}
}