using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
	public interface IWebLogin : ILogin
	{
		Task<IToken> AddToken(IScope scope, string code);
	}
}
