using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	internal struct KeyTokenSave
	{
		public string Key { get; }

		public string User { get; }

		public string Scope { get; }

		public KeyTokenSave(string key, string user, string scope)
		{
			this.Key = key;
			this.User = user;
			this.Scope = scope;
		}
	}
}
