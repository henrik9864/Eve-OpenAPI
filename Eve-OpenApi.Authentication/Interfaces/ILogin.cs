using System.Collections.Generic;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public interface ILogin
	{
		//ILoginSetup LoginSetup { get; }

		IToken this[string user, string scope] { get; }

		bool TryGetToken(string user, string scope, out IToken token);

		Task<IToken> AddToken(string scope);

		IToken GetToken(string user, string scope);

		IList<string> GetUsers();

		IList<IToken> GetTokens(string user);
	}
}