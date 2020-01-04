using EveOpenApi.Authentication.Factories;
using EveOpenApi.Authentication.Interfaces;
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

		public KeyLoginBuilder FromFile(string path)
		{
			string encryptedJson = File.ReadAllText(path);

			return FromString(encryptedJson);
		}

		public KeyLoginBuilder FromString(string json)
		{
			(string type, List<KeyTokenSave> tokens) = JsonSerializer.Deserialize<(string, List<KeyTokenSave>)>(json);

			if (type != "Key")
				throw new Exception($"Unknown token types '{type}' expected type 'Key'");

			Tokens = tokens;

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
