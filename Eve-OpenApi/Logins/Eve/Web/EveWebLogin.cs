using EveOpenApi.Eve;
using EveOpenApi.Interfaces;
using Microsoft.Extensions.Options;
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
    public class EveWebLogin : ILogin
	{
		private static HttpClient Client { get; set; }

		public IInterfaceSetup Setup { get; }

		public IToken this[string user, string scope]
		{
			get
			{
				return GetToken(user, (Scope)scope);
			}
		}

		public string ClientID { get; }

		public string ClientSecret { get; }

		public string Callback { get; }

		Dictionary<string, List<IToken>> userTokens;

		public EveWebLogin(IOptions<EveWebConfig> options, HttpClient client)
			: this(options.Value.ClientID, options.Value.ClientSecret, options.Value.CallbackUrl, client)
		{

		}

		public EveWebLogin(string clientID, string clientSecret, string callback, HttpClient client = default)
		{
			if (Client == default && client != default)
				Client = client;
			else if (Client == default)
				Client = new HttpClient();

			ClientID = clientID;
			Callback = callback;
			ClientSecret = clientSecret;
			Setup = new EveInterfaceSetup();
			userTokens = new Dictionary<string, List<IToken>>();
		}

		public (string authURl, string state) GetAuthUrl(IScope scope)
		{
			return EveAuthentication.GetAuthUrl(scope, ClientID, Callback, Client);
		}

		public async Task<IToken> AddToken(IScope scope, string code)
		{
			EveToken token = await EveAuthentication.GetWebToken(scope, code, ClientID, ClientSecret);
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

		public List<string> GetUsers()
		{
			var dicList = userTokens.ToList();
			return dicList.ConvertAll(a => a.Key);
		}

		void AddToken(EveToken token)
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
