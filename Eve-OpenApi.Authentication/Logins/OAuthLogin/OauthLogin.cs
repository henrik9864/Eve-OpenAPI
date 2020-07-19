using EveOpenApi.Authentication.Interfaces;
using EveOpenApi.Authentication.Managers;
using EveOpenApi.Authentication.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public class OauthLogin : IOauthLogin
	{
		public ILoginCredentials Credentials { get; }

		public ILoginConfig Config { get; }

		ITokenManager tokenManager;

		Dictionary<string, List<IOauthToken>> userTokens;

		internal OauthLogin(ILoginConfig config, ILoginCredentials credentials, ITokenManager tokenManager)
		{
			Credentials = credentials;
			Config = config;
			this.tokenManager = tokenManager;

			userTokens = new Dictionary<string, List<IOauthToken>>();
		}

		#region ILogin

		public async Task<IToken> GetToken(string user, IScope scope)
		{
			IOauthToken token = userTokens[user].Find(a => a.Scope.IsSubset(scope));

			if (token?.Expires > DateTime.UtcNow)
				token = await RefreshToken(token);

			return token;
		}

		public IList<string> GetUsers()
		{
			var dicList = userTokens.ToList();
			return dicList.ConvertAll(a => a.Key);
		}

		public IList<IToken> GetTokens(string user)
		{
			return userTokens[user].ConvertAll<IToken>(x => x);
		}

		/// <summary>
		/// Saves encrypted json to file
		/// </summary>
		/// <param name="path"></param>
		/// <param name="override"></param>
		public void SaveToFile(string path, bool @override)
		{
			if (File.Exists(path) && !@override)
				throw new Exception("File already exists, enable override to override it.");

			string json = ToJson();
			File.WriteAllText(path, json);
		}

		public void SaveToFileEncrypted(string path, bool @override)
		{
			if (File.Exists(path) && !@override)
				throw new Exception("File already exists, enable override to override it.");

			string encryptedJson = ToEncrypted();
			File.WriteAllText(path, encryptedJson);
		}

		public string ToJson()
		{
			List<TokenSave> tokens = userTokens.Values
				.SelectMany(x => x)
				.ToList()
				.ConvertAll(a => new TokenSave(a.RefreshToken, a.Scope.ScopeString));

			return JsonSerializer.Serialize(new LoginSave("OAuth", JsonSerializer.Serialize(tokens)));
		}

		#endregion

		#region OAuthLogin

		public async Task<IOauthToken> AddToken(IScope scope)
		{
			var result = await tokenManager.GetToken(scope);
			AddToken(result.owner, result.token);

			return result.token;
		}

		public async Task<IOauthToken> AddToken(string refreshToken, IScope scope)
		{
			var result = await tokenManager.RefreshToken(refreshToken, scope);
			AddToken(result.owner, result.token);

			return result.token;
		}

		public async Task<IOauthToken> RefreshToken(IOauthToken token)
		{
			(IOauthToken newToken, string owner) = await tokenManager.RefreshToken(token.RefreshToken, token.Scope);
			UpdateToken(owner, newToken);

			return newToken;
		}

		public async Task<string> GetAuthUrl(IScope scope)
		{
			var authUrl = tokenManager.GenerateAuthUrl(scope);

			await Task.Factory.StartNew(async () =>
			{
				try
				{
					var response = await tokenManager.ListenForResponse(scope, authUrl);
					AddToken(response.owner, response.token);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw e;
				}
			});

			return authUrl.Url;
		}

		public string ToEncrypted()
		{
			string json = ToJson();

			// Use client secret if supplied
			string passPhrase = string.IsNullOrEmpty(Credentials.ClientSecret) ? Credentials.ClientID : Credentials.ClientSecret;
			return StringCipher.Encrypt(json, passPhrase);
		}

		#endregion

		void AddToken(string owner, IOauthToken token)
		{
			if (!userTokens.TryGetValue(owner, out List<IOauthToken> tokens))
			{
				tokens = new List<IOauthToken>();
				userTokens[owner] = tokens;
			}

			tokens.Add(token);
		}

		void UpdateToken(string owner, IOauthToken token)
		{
			List<IOauthToken> tokens = userTokens[owner];
			int tokenIndex = tokens.FindIndex(x => x.RefreshToken == token.RefreshToken);

			tokens[tokenIndex] = token;
		}
	}
}
