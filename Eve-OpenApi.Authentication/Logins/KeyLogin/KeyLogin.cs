using EveOpenApi.Authentication.Interfaces;
using EveOpenApi.Authentication.Structs;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public class KeyLogin : IKeyLogin
	{
		Dictionary<string, List<IToken>> keys;

		IFactory<IToken> tokenFactory;

		internal KeyLogin(IFactory<IToken> tokenFactory)
		{
			this.tokenFactory = tokenFactory;
			keys = new Dictionary<string, List<IToken>>();
		}

		public void AddKey(string key, string user, IScope scope)
		{
			IToken token = tokenFactory.Create(key, scope);
			if (!this.keys.TryGetValue(user, out List<IToken> tokens))
			{
				tokens = new List<IToken>();
				keys[user] = tokens;
			}

			tokens.Add(token);
		}

		public async Task<IToken> GetToken(string user, IScope scope)
		{
			await Task.CompletedTask;
			return keys[user].Find(a => a.Scope.IsSubset(scope));
		}

		public IList<IToken> GetTokens(string user)
		{
			return keys[user];
		}

		public IList<string> GetUsers()
		{
			return keys.ToList().Select(x => x.Key).ToList();
		}

		public void SaveToFile(string path, bool @override)
		{
			if (File.Exists(path) && !@override)
				throw new Exception("File already exists, enable override to override it.");

			string json = ToJson();
			File.WriteAllText(path, json);
		}

		public string ToJson()
		{
			List<KeyTokenSave> keys = this.keys
				.SelectMany(x => x.Value.ConvertAll(a => new KeyTokenSave(a.GetToken(), x.Key, a.Scope.ScopeString)))
				.ToList();

			return JsonSerializer.Serialize(new LoginSave("Key", JsonSerializer.Serialize(keys)));
		}
	}
}
