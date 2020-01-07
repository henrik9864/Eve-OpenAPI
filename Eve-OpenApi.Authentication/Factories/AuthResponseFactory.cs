using EveOpenApi.Authentication;
using EveOpenApi.Authentication.Managers;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Factories
{
	class AuthResponseFactory : IFactory<IAuthResponse>
	{
		public IAuthResponse Create(params object[] context)
		{
			string code = context[0].ToString();
			string state = context[1].ToString();

			return new AuthResponse()
			{
				Code = code,
				State = state
			};
		}
	}
}
