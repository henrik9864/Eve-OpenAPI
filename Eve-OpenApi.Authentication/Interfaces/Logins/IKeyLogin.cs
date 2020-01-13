using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication.Interfaces
{
	public interface IKeyLogin : ILogin
	{
		void AddKey(string key, string user, IScope scope);
	}
}
