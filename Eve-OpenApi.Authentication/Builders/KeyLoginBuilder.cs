using EveOpenApi.Authentication.Factories;
using EveOpenApi.Authentication.Interfaces;
using EveOpenApi.Authentication.Structs;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EveOpenApi.Authentication
{
	public class KeyLoginBuilder
	{
		private List<KeyTokenSave> Tokens { get; set; }

		public KeyLoginBuilder()
		{
			Tokens = new List<KeyTokenSave>();
		}

		public KeyLoginBuilder FromFile(string path)
		{
			string encryptedJson = File.ReadAllText(path);

			return FromString(encryptedJson);
		}

		public KeyLoginBuilder FromString(string json)
		{
			LoginSave save = JsonSerializer.Deserialize<LoginSave>(json);

			if (save.Type != "Key")
				throw new Exception($"Unknown token types '{save.Type}' expected type 'Key'");

			Tokens = JsonSerializer.Deserialize<List<KeyTokenSave>>(save.Data);
			return this;
		}

		public IKeyLogin Build()
		{
			IFactory<IToken> tokenFactory = new KeyTokenFactory();

			IKeyLogin login = new KeyLogin(tokenFactory);
			for (int i = 0; i < Tokens.Count; i++)
				login.AddKey(Tokens[i].Key, Tokens[i].User, (Scope)Tokens[i].Scope);

			return login;
		}
	}
}
