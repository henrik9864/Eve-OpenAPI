using System.Collections.Generic;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public interface ILogin
	{
		//ILoginSetup LoginSetup { get; }

		ILoginConfig Config { get; }

		IToken this[string user, string scope] { get; }

		Task<IToken> AddToken(IScope scope);

		Task<IToken> AddToken(string refreshToken, IScope scope);

		Task<IToken> RefreshToken(IToken token);

		Task<string> GetAuthUrl(IScope scope);

		bool TryGetToken(string user, IScope scope, out IToken token);

		IToken GetToken(string user, IScope scope);

		IList<string> GetUsers();

		IList<IToken> GetTokens(string user);

		void SaveToFile(string path, bool @override);

		string ToJson();

		string ToEncrypted();
	}
}