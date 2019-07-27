using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Authentication.Managers
{
	internal interface ITokenManager
	{
		AuthUrl GenerateAuthUrl(IScope scope);

		Task<(IToken token, string owner)> GetToken(IScope scope);

		Task<(IToken token, string owner)> ListenForResponse(IScope scope, AuthUrl authUrl);
	}
}