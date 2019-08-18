using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace EveOpenApi.Api
{
	public interface IApiPath
	{
		/// <summary>
		/// List of users to use as default :)
		/// </summary>
		List<string> DefaultUsers { get; }

		/// <summary>
		/// Path after the domain
		/// </summary>
		string Path { get; }

		Task<IApiResponse> Delete(params (string, object)[] parameters);

		Task<IApiResponse<T>> Delete<T>(params (string, object)[] parameters);

		Task<IEnumerable<IApiResponse>> DeleteBatch(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> DeleteBatch<T>(params (string, List<object>)[] parameters);

		Task<IApiResponse> Get(params (string, object)[] parameters);

		Task<IApiResponse<T>> Get<T>(params (string, object)[] parameters);

		Task<IEnumerable<IApiResponse>> GetBatch(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> GetBatch<T>(params (string, List<object>)[] parameters);

		Task<IApiResponse> Post(params (string, object)[] parameters);

		Task<IApiResponse<T>> Post<T>(params (string, object)[] parameters);

		Task<IEnumerable<IApiResponse>> PostBatch(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> PostBatch<T>(params (string, List<object>)[] parameters);

		Task<IApiResponse> Put(params (string, object)[] parameters);

		Task<IApiResponse<T>> Put<T>(string user, params (string, object)[] parameters);

		Task<IEnumerable<IApiResponse>> PutBatch(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> PutBatch<T>(params (string, List<object>)[] parameters);

		Task<IApiResponse> Run(OperationType type, params (string, object)[] parameters);

		Task<IApiResponse<T>> Run<T>(OperationType type, params (string, object)[] parameters);

		Task<IEnumerable<IApiResponse>> RunBatch(OperationType type, params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> RunBatch<T>(OperationType type, params (string, List<object>)[] parameters);

		/// <summary>
		/// Assign new default users to this path.
		/// </summary>
		/// <param name="users"></param>
		/// <returns></returns>
		IApiPath SetUsers(params string[] users);
	}
}