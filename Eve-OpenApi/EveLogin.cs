using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi
{
	public class EveLogin
	{
		private static HttpClient Client { get; set; }

		public string ClientID { get; }

		public string Callback { get; }

		public IReadOnlyList<EveToken> this[string user]
		{
			get
			{
				return userTokens[user];
			}
		}

		public EveToken this[string user, string scope]
		{
			get
			{
				List<EveToken> tokens = userTokens[user];
				EveToken token = tokens.Find(a => a.Scope.HasScope(scope));

				if (token == null)
					throw new Exception($"No found with: {scope}");

				return token;
			}
		}

		private Dictionary<string, List<EveToken>> userTokens = new Dictionary<string, List<EveToken>>();

		private EveLogin(string clientID, string callback)
		{
			ClientID = clientID;
			Callback = callback;
		}

		/// <summary>
		/// Create a new token with scope. (Auto opens URL in browser)
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		public async Task<EveToken> AddToken(Scope scope)
		{
			EveToken token = await EveToken.Create(scope, ClientID, Callback, Client);
			AddToken(token);

			return token;
		}

		/// <summary>
		/// Create a new token with scope. (URL must be manually given to user)
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		public async Task<string> Authenticate(Scope scope)
		{
			var auth = EveToken.Authenticate(scope, ClientID, Callback);
			AddEveReponse(scope, auth.state, auth.verifier);

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
		public bool TryGetToken(string user, string scope, out EveToken token)
		{
			userTokens.TryGetValue(user, out List<EveToken> tokens);

			token = tokens?.Find(a => a.Scope.HasScope(scope));
			return token != null;
		}

		/// <summary>
		/// Get all users with token.
		/// </summary>
		/// <returns></returns>
		public List<string> GetUsers()
		{
			var dicList = userTokens.ToList();
			return dicList.ConvertAll(a => a.Key);
		}

		/// <summary>
		/// Save all tokens to file.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public async Task SaveToFile(string filePath)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
			{
				string jsonString = ToJson();
				byte[] json = Encoding.UTF8.GetBytes(jsonString);

				await fileStream.WriteAsync(json);
			}
		}

		/// <summary>
		/// Convert EveLogin to Json that can be loaded.
		/// </summary>
		/// <returns></returns>
		public string ToJson()
		{
			Dictionary<string, List<string>> eveLoginSave = new Dictionary<string, List<string>>();
			foreach (var user in userTokens)
			{
				List<string> tokenSaves = new List<string>();
				foreach (var token in user.Value)
					tokenSaves.Add(token.ToJson());

				eveLoginSave.Add(user.Key, tokenSaves);
			}

			return JsonConvert.SerializeObject((eveLoginSave, ClientID, Callback));
		}

		/// <summary>
		/// Get response from auth url and add the token.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="state"></param>
		/// <param name="verfier"></param>
		async void AddEveReponse(Scope scope, string state, string verfier)
		{
			EveToken token = await EveToken.ValidateResponse(scope, Callback, state, verfier, ClientID);
			AddToken(token);
		}

		/// <summary>
		/// Add token to dictionary
		/// </summary>
		/// <param name="token"></param>
		void AddToken(EveToken token)
		{
			if (userTokens.TryGetValue(token.Name, out List<EveToken> list))
				list.Add(token);
			else
			{
				list = new List<EveToken>();
				list.Add(token);

				userTokens.Add(token.Name, list);
			}
		}

		/// <summary>
		/// Create a empty EveLogin with no authenticated users.
		/// </summary>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static async Task<EveLogin> Create(string clientID, string callback)
		{
			await Task.CompletedTask;
			return new EveLogin(clientID, callback);
		}

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
			if (Client == default && client != default)
				Client = client;
			else
				Client = new HttpClient();

			EveLogin login = await Create(clientID, callback);
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
			if (Client == default && client != default)
				Client = client;
			else
				Client = new HttpClient();

			(Dictionary<string, List<string>> eveLoginSave, string clientID, string callback) loaded;
			using (StreamReader reader = new StreamReader(filePath))
			{
				string json = await reader.ReadToEndAsync();
				loaded = JsonConvert.DeserializeObject<(Dictionary<string, List<string>>, string, string)>(json);
			}

			EveLogin login = new EveLogin(loaded.clientID, loaded.callback);
			foreach (var user in loaded.eveLoginSave)
			{
				List<EveToken> tokens = new List<EveToken>();

				foreach (var token in user.Value)
					tokens.Add(await EveToken.FromJson(token, Client));

				login.userTokens.Add(user.Key, tokens);
			}

			return login;
		}
	}
}
