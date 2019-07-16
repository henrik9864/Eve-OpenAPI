using EveOpenApi.Eve;
using EveOpenApi.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi
{
	/// <summary>
	/// ESI authentication for client programs.
	/// TODO: Add AddResponse to DI
	/// </summary>
	public class EveLogin : IClientLogin
	{
		private static HttpClient Client { get; set; }

		public ILoginSetup LoginSetup { get; }

		public IEveLoginConfig LoginConfig { get; }

		public IToken this[string user, string scope]
		{
			get
			{
				return GetToken(user, (Scope)scope);
			}
		}

		Dictionary<string, List<IToken>> userTokens;
		ITokenFactoryAsync<EveToken> tokenFactory;

		internal EveLogin(IEveLoginConfig loginConfig, ILoginSetup loginSetup, ITokenFactoryAsync<EveToken> tokenFactory, HttpClient client)
		{
			Client = client;
			LoginSetup = loginSetup;
			LoginConfig = loginConfig;
			this.tokenFactory = tokenFactory;

			userTokens = new Dictionary<string, List<IToken>>();
		}

		/// <summary>
		/// Create a new token with scope. (Auto opens URL in browser)
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		public async Task<IToken> AddToken(IScope scope)
		{
			//IToken token = await EveAuthentication.CreateToken(scope, LoginConfig.ClientID, LoginConfig.Callback, Client);
			IToken token = await tokenFactory.CreateTokenAsync(scope, LoginConfig.ClientID, LoginConfig.Callback, Client);
			AddToken(token);

			return token;
		}

		/// <summary>
		/// Create a new token with scope. (URL must be manually given to user)
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		public async Task<string> GetAuthURL(IScope scope)
		{
			var auth = EveAuthentication.Authenticate(scope, LoginConfig.ClientID, LoginConfig.Callback);
			AddResponse(scope, auth.state, auth.verifier);

			await Task.CompletedTask;
			return auth.authUrl;
		}

		/// <summary>
		/// Try to get a token with scope.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="scope"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public bool TryGetToken(string user, IScope scope, out IToken token)
		{
			userTokens.TryGetValue(user, out List<IToken> tokens);

			token = tokens?.Find(a => a.Scope.IsSubset(scope));
			return token != null;
		}

		public IToken GetToken(string user, IScope scope)
		{
			IToken token = userTokens[user].Find(a => a.Scope.IsSubset(scope));

			if (token == null)
				throw new Exception($"No token with scope '{scope}' found");

			return token;
		}

		/// <summary>
		/// Get all users with token.
		/// </summary>
		/// <returns></returns>
		public IList<string> GetUsers()
		{
			var dicList = userTokens.ToList();
			return dicList.ConvertAll(a => a.Key);
		}

		/// <summary>
		/// Get all scopes for a user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public IList<IToken> GetTokens(string user)
		{
			return userTokens[user];
		}

		/// <summary>
		/// Get response from auth url and add the token.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="state"></param>
		/// <param name="verfier"></param>
		async void AddResponse(IScope scope, string state, string verfier)
		{
			IToken token = await EveAuthentication.ValidateResponse(scope, LoginConfig.Callback, state, verfier, LoginConfig.ClientID);
			AddToken(token);
		}

		/// <summary>
		/// Add token to dictionary
		/// </summary>
		/// <param name="token"></param>
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

		#region Static methods

		/*

		/// <summary>
		/// Create a new EveLogin and add a token with scope.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <param name="client"></param>
		/// <returns></returns>
		public static async Task<EveLogin> Login(Scope scope, string clientID, string callback, HttpClient client = default)
		{
			EveLogin login = new EveLogin(clientID, callback, client);
			await login.AddToken(scope);

			return login;
		}

		/// <summary>
		/// Load EveLogin from file.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="client"></param>
		/// <returns></returns>
		public static async Task<EveLogin> FromFile(string filePath, HttpClient client = default)
		{
			(Dictionary<string, List<string>> eveLoginSave, string clientID, string callback) loaded;
			using (StreamReader reader = new StreamReader(filePath))
			{
				string json = await reader.ReadToEndAsync();
				loaded = JsonConvert.DeserializeObject<(Dictionary<string, List<string>>, string, string)>(json);
			}

			EveLogin login = new EveLogin(loaded.clientID, loaded.callback, client);
			foreach (var user in loaded.eveLoginSave)
			{
				List<IToken> tokens = new List<IToken>();

				foreach (var token in user.Value)
					tokens.Add(await EveAuthentication.FromJson(token, Client));

				login.userTokens.Add(user.Key, tokens);
			}

			return login;
		}

		*/

		#endregion
	}
}
