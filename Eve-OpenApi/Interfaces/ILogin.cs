using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
	/// <summary>
	/// Interface for obtaining access tokens for swagger API's
	/// </summary>
    public interface ILogin
	{
		IInterfaceSetup Setup { get; }

		IToken this[string user, string scope] { get; }

		bool TryGetToken(string user, IScope scope, out IToken token);

		//Task<IToken> AddToken(IScope scope);

		IToken GetToken(string user, IScope scope);
	}
}
