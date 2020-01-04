using EveOpenApi.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication.Interfaces
{
	interface ITokenFactory
	{
		IToken Create(string accessToken, string refreshToken, uint expires, string tokenType, IScope scope);

		IToken FromJson(string json, IScope scope);
	}
}
