using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EveOpenApi.Api;
using Microsoft.OpenApi.Models;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Managers
{
	internal interface IRequestManager
	{
		/// <summary>
		/// Create and execute a ApiRequest and return all responses.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <param name="users"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		Task<IEnumerable<IApiResponse>> RequestBatch(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation);

		/// <summary>
		/// Create and execute a ApiRequest of type and return all responses.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <param name="users"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		Task<IEnumerable<IApiResponse<T>>> RequestBatch<T>(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation);

		IEnumerable<IApiRequest> GetRequest(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation);
	}
}