using System.Threading.Tasks;
using EveOpenApi.Api;

namespace EveOpenApi.Managers
{
	internal interface ITokenManager
	{
		/// <summary>
		/// Add authentication token from login to ApiRequest
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task AddAuthTokens(IApiRequest request);
	}
}