using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	internal class KeyToken : IToken
	{
		public string Key { get; }

		public IScope Scope { get; }

		public KeyToken(string key, IScope scope)
		{
			Key = key;
			Scope = scope;
		}

		public string GetToken()
		{
			return Key;
		}
	}
}
