using System.Threading.Tasks;

namespace EveOpenApi.Authentication.Managers
{
	internal interface ITokenManager
	{
		Task<(IToken token, string owner)> GetToken(string scope);
	}
}