using System.Threading.Tasks;

namespace EveOpenApi.Authentication.Managers
{
	internal interface ITokenManager
	{
		(string authUrl, string state) GenerateAuthUrl(IScope scope);

		Task<(IToken token, string owner)> GetToken(IScope scope);

		Task<(IToken token, string owner)> ListenForResponse(IScope scope, string state);
	}
}