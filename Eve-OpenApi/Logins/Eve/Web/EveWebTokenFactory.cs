using EveOpenApi.Eve;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Logins.Eve.Web
{
	internal class EveWebTokenFactory : ITokenFactoryAsync<EveToken>
	{
		public async Task<EveToken> CreateTokenAsync(params object[] context)
		{
			IScope scope = (IScope)context[0];
			string code = (string)context[1];
			string clientID = (string)context[2];
			string clientSecret = (string)context[3];

			return await EveAuthentication.GetWebToken(scope, code, clientID, clientSecret);
		}
	}
}
