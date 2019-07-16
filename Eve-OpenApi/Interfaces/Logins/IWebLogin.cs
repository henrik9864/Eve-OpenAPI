using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
	public interface IWebLogin : ILogin
	{
		(string authUrl, string state) GetAuthUrl(IScope scope);

		Task<IToken> AddToken(IScope scope, string code);
	}
}
