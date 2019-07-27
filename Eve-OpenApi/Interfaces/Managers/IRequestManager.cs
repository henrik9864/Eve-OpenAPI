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
		Task<IList<IApiResponse>> RequestBatch(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation);

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
		Task<IList<IApiResponse<T>>> RequestBatch<T>(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation);

		ApiRequest GetRequest(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation);
	}
}