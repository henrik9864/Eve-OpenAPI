using System.Threading.Tasks;

namespace EveOpenApi.Authentication.Managers
{
	internal interface ITokenManager
	{
		AuthUrl GenerateAuthUrl(IScope scope);

		Task<(IToken token, string owner)> GetToken(IScope scope);

		Task<(IToken token, string owner)> ListenForResponse(IScope scope, AuthUrl authUrl);
	}
}