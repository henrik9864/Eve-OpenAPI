using EveOpenApi.Eve;
using EveOpenApi.Interfaces;
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

		public IToken this[string scope]
		{
			get
			{
				return GetToken((Scope)scope);
			}
		}

		public string ClientID { get; }

		public string ClientSecret { get; }

		public string Callback { get; }

		public string CurrentUser { get; private set; }

		Dictionary<string, List<IToken>> userTokens;

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
			return EveAuthentication.GetAuthUrl(scope, ClientID, Callback);
		}

		public Task<IToken> AddToken(string code)
		{

		}

		public bool TryGetToken(IScope scope, out IToken token)
		{
			userTokens.TryGetValue(CurrentUser, out List<IToken> tokens);

			token = tokens?.Find(a => a.Scope.IsSubset(scope));
			return token != null;
		}

		public IToken GetToken(IScope scope)
		{
			IToken token = userTokens[CurrentUser].Find(a => a == scope);

			if (token == null)
				throw new Exception($"No token with scope '{scope}' found");

			return token;
		}

		public List<string> GetUsers()
		{
			var dicList = userTokens.ToList();
			return dicList.ConvertAll(a => a.Key);
		}
	}
}
