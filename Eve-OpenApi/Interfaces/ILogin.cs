using System.Collections.Generic;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public interface ILogin
	{
		Task<IToken> GetToken(string user, IScope scope);

		IList<string> GetUsers();

		IList<IToken> GetTokens(string user);

		void SaveToFile(string path, bool @override);

		string ToJson();
	}
}