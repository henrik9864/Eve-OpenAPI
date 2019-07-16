using EveOpenApi.Eve;
using EveOpenApi.Interfaces;
using EveOpenApi.Seat;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi
{
	/// <summary>
	/// ESI authentication for web applications.
	/// </summary>
    public class EveWebLogin : IWebLogin
	{
		private static HttpClient Client { get; set; }

		public ILoginSetup LoginSetup { get; }

		public IEveWebLoginConfig LoginConfig { get; }

		public IToken this[string user, string scope]
		{
			get
			{
				return GetToken(user, (Scope)scope);
			}
		}

		Dictionary<string, List<IToken>> userTokens;
		ITokenFactoryAsync<EveToken> tokenFactory;

		internal EveWebLogin(IEveWebLoginConfig loginConfig, ILoginSetup loginSetup, ITokenFactoryAsync<EveToken> tokenFactory, HttpClient client)
		{
			LoginSetup = loginSetup;
			LoginConfig = loginConfig;
			this.tokenFactory = tokenFactory;
			userTokens = new Dictionary<string, List<IToken>>();
		}

		public (string authURl, string state) GetAuthUrl(IScope scope)
		{
			return EveAuthentication.GetAuthUrl(scope, LoginConfig.ClientID, LoginConfig.Callback, Client);
		}

		public async Task<IToken> AddToken(IScope scope, string code)
		{
			IToken token = await tokenFactory.CreateTokenAsync(scope, code, LoginConfig.ClientID, LoginConfig.ClientSecret);
			AddToken(token);

			return token;
		}

		public bool TryGetToken(string user, IScope scope, out IToken token)
		{
			userTokens.TryGetValue(user, out List<IToken> tokens);

			token = tokens?.Find(a => a.Scope.IsSubset(scope));
			return token != null;
		}

		public IToken GetToken(string user, IScope scope)
		{
			IToken token = userTokens[user].Find(a => a == scope);

			if (token == null)
				throw new Exception($"No token with scope '{scope}' found");

			return token;
		}

		IList<string> ILogin.GetUsers()
		{
			return userTokens.ToList()
				.ConvertAll(a => a.Key);
		}

		public IList<IToken> GetTokens(string user)
		{
			return userTokens[user];
		}

		void AddToken(IToken token)
		{
			if (userTokens.TryGetValue(token.Name, out List<IToken> list))
				list.Add(token);
			else
			{
				list = new List<IToken>();
				list.Add(token);

				userTokens.Add(token.Name, list);
			}
		}
	}
}
