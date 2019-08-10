using EveOpenApi.Authentication.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public class OauthLogin : ILogin
	{
		public ILoginConfig Config { get; }

		public ILoginCredentials Credentials { get; }

		public IToken this[string user, string scope]
		{
			get
			{
				return GetToken(user, (Scope)scope);
			}
		}

		ITokenManager tokenManager;

		Dictionary<string, List<IToken>> userTokens;

		internal OauthLogin(ILoginConfig config, ILoginCredentials credentials, ITokenManager tokenManager)
		{
			Config = config;
			Credentials = credentials;
			this.tokenManager = tokenManager;

			userTokens = new Dictionary<string, List<IToken>>();
		}

		public async Task<IToken> AddToken(IScope scope)
		{
			var result = await tokenManager.GetToken(scope);
			AddToken(result.owner, result.token);

			return result.token;
		}

		public async Task<IToken> AddToken(string refreshToken, IScope scope)
		{
			var result = await tokenManager.RefreshToken(refreshToken, scope);
			AddToken(result.owner, result.token);

			return result.token;
		}

		public Task<IToken> RefreshToken(IToken token)
		{
			return AddToken(token.RefreshToken, token.Scope);
		}

		public async Task<string> GetAuthUrl(IScope scope)
		{
			var authUrl = tokenManager.GenerateAuthUrl(scope);

			await Task.Factory.StartNew(async () =>
			{
				var response = await tokenManager.ListenForResponse(scope, authUrl);
				AddToken(response.owner, response.token);
			});

			return authUrl.Url;
		}

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

		public IList<string> GetUsers()
		{
			var dicList = userTokens.ToList();
			return dicList.ConvertAll(a => a.Key);
		}

		public IList<IToken> GetTokens(string user)
		{
			return userTokens[user];
		}

		public void SaveToFile(string path, bool @override)
		{
			if (File.Exists(path) && !@override)
				throw new Exception("File already exists, enable override to override it.");

			List<TokenSave> tokens = userTokens.Values
				.SelectMany(x => x)
				.ToList()
				.ConvertAll(a => new TokenSave(a.RefreshToken, a.Scope.ScopeString));

			string json = JsonSerializer.Serialize(tokens);

			// Use client secret if supplied
			string passPhrase = string.IsNullOrEmpty(Credentials.ClientSecret) ? Credentials.ClientID : Credentials.ClientSecret;
			string encryptedJson = StringCipher.Encrypt(json, passPhrase);

			File.WriteAllText(path, encryptedJson);
		}

		void AddToken(string owner, IToken token)
		{
			if (!userTokens.TryGetValue(owner, out List<IToken> tokens))
			{
				tokens = new List<IToken>();
				userTokens[owner] = tokens;
			}

			tokens.Add(token);
		}
	}
}
