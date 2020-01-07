using EveOpenApi.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication.Interfaces
{
	interface IOauthTokenFactory
	{
		IOauthToken Create(string accessToken, string refreshToken, uint expires, string tokenType, IScope scope);

		IOauthToken FromJson(string json, IScope scope);
	}
}
