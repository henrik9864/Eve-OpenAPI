using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EveOpenApi.Api;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Managers
{
	internal interface ITokenManager
	{
		/// <summary>
		/// Add authentication token from login to ApiRequest
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		void AddAuthTokens(IApiRequest request);
	}
}