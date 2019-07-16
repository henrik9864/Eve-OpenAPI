using EveOpenApi.Eve;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Logins.Eve.Client
{
	public class EveTokenFactory : ITokenFactoryAsync<EveToken>
	{
		public async Task<EveToken> CreateTokenAsync(params object[] context)
		{
			IScope scope = (IScope)context[0];
			string clientID = (string)context[1];
			string callback = (string)context[2];

			return await EveAuthentication.CreateToken(scope, clientID, callback);
		}
	}
}
