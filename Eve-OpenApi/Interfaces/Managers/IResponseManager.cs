using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EveOpenApi.Api;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Managers
{
	internal interface IResponseManager
	{
		/// <summary>
		/// Get a response from the API.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="index">What request index to use.</param>
		/// <returns></returns>
		Task<IApiResponse> GetResponse(IApiRequest request, int index);

		/// <summary>
		/// Get a response from the API of type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <param name="index">What request index to use.</param>
		/// <returns></returns>
		Task<IApiResponse<T>> GetResponse<T>(IApiRequest request, int index);
	}
}