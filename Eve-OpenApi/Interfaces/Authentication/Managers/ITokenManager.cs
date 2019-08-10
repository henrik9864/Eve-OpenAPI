using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Authentication.Managers
{
	internal interface ITokenManager
	{
		/// <summary>
		/// Just generates the auth url, give this to the user and call the ListenForResponse method.
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		AuthUrl GenerateAuthUrl(IScope scope);

		/// <summary>
		/// Will listen for a response on the callback for a new token.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="authUrl"></param>
		/// <returns></returns>
		Task<(IToken token, string owner)> ListenForResponse(IScope scope, AuthUrl authUrl);

		/// <summary>
		/// Will automatily promt the user with a page requesting the token.
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		Task<(IToken token, string owner)> GetToken(IScope scope);

		Task<(IToken token, string owner)> RefreshToken(string refreshToken, IScope scope);
	}
}