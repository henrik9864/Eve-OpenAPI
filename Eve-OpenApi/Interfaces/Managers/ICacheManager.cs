using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EveOpenApi.Api;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Managers
{
	internal interface ICacheManager
	{
		/// <summary>
		/// Get all responses from an ApiRequest
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task<IList<IApiResponse>> GetResponse(IApiRequest request);

		/// <summary>
		/// Get a response from a ApiRequest
		/// </summary>
		/// <param name="request"></param>
		/// <param name="index">Wich request to execute</param>
		/// <returns></returns>
		Task<IApiResponse> GetResponse(IApiRequest request, int index);

		/// <summary>
		/// Get all responsen from a ApiRequest of type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		Task<IList<IApiResponse<T>>> GetResponse<T>(IApiRequest request);

		/// <summary>
		/// Get a response from a ApiRequest of type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <param name="index">Wich request to execute</param>
		/// <returns></returns>
		Task<IApiResponse<T>> GetResponse<T>(IApiRequest request, int index);

		bool TryHitCache(IApiRequest request, int index, bool validateTime, out IApiResponse response);
	}
}