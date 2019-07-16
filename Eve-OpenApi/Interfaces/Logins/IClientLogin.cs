using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
	public interface IClientLogin : ILogin
	{
		Task<IToken> AddToken(IScope scope);
	}
}
