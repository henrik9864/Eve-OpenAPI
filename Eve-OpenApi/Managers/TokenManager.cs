using EveOpenApi.Api;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Managers
{
	internal class TokenManager : BaseManager, ITokenManager
	{
		public TokenManager(HttpClient client, IApiConfig config, ILogin login) : base(client, login, config)
		{
		}


		/// <summary>
		/// Add auth token to all requests
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task AddAuthTokens(IApiRequest request)
		{
			if (Login is null && !string.IsNullOrEmpty(request.Scope))
				throw new Exception("No login provided");

			if (Login != null || Config.AlwaysIncludeAuthHeader)
			{
				for (int i = 0; i < request.Parameters.MaxLength; i++)
					await AddAuthToken(request, i);
			}
		}

		/// <summary>
		/// Add auth token to request
		/// </summary>
		/// <param name="request"></param>
		/// <param name="index">Wich request to add to</param>
		/// <returns></returns>
		async Task AddAuthToken(IApiRequest request, int index)
		{
			if (string.IsNullOrEmpty(request.Scope))
				return;

			if (Login is null && Config.AlwaysIncludeAuthHeader)
				AddTokenLocation(request, "");

			if (!Login.TryGetToken(request.GetUser(index), (Scope)request.Scope, out IToken token))
				throw new Exception($"No token with scope '{request.Scope}'");

			AddTokenLocation(request, await token.GetToken());
		}

		/// <summary>
		/// Add auth token to the correct location accoridng to the login config
		/// </summary>
		/// <param name="request"></param>
		/// <param name="token"></param>
		void AddTokenLocation(IApiRequest request, string token)
		{
			switch (Login.LoginSetup.TokenLocation)
			{
				case "header":
					if (request.Parameters.Headers.Exists(a => a.Key == Login.LoginSetup.TokenName))
						return;

					request.SetHeader(Login.LoginSetup.TokenName, token);
					break;
				case "query":
					request.AddToQuery(Login.LoginSetup.TokenName, token);
					break;
				default:
					throw new Exception("Unknown access token location");
			}
		}
	}
}
