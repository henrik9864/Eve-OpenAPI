using EveOpenApi.Authentication;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication.Factories
{
	class KeyTokenFactory : IFactory<IToken>
	{
		public IToken Create(params object[] context)
		{
			string key = context[0].ToString();
			IScope scope = (IScope)context[1];

			return new KeyToken(key, scope);
		}
	}
}
