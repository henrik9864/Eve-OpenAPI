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

		#region Delete

		Task<IApiResponse> Delete(params (string, object)[] parameters);

		Task<IApiResponse> Delete(params object[] parameters);

		Task<IApiResponse<T>> Delete<T>(params (string, object)[] parameters);

		Task<IApiResponse<T>> Delete<T>(params object[] parameters);

		Task<IEnumerable<IApiResponse>> DeleteBatch(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse>> DeleteBatch(params List<object>[] parameters);

		Task<IEnumerable<IApiResponse<T>>> DeleteBatch<T>(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> DeleteBatch<T>(params List<object>[] parameters);

		#endregion

		#region Get

		Task<IApiResponse> Get(params (string, object)[] parameters);

		Task<IApiResponse> Get(params object[] parameters);

		Task<IApiResponse<T>> Get<T>(params (string, object)[] parameters);

		Task<IApiResponse<T>> Get<T>(params object[] parameters);

		Task<IEnumerable<IApiResponse>> GetBatch(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse>> GetBatch(params List<object>[] parameters);

		Task<IEnumerable<IApiResponse<T>>> GetBatch<T>(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> GetBatch<T>(params List<object>[] parameters);

		#endregion

		#region Post

		Task<IApiResponse> Post(params (string, object)[] parameters);

		Task<IApiResponse> Post(params object[] parameters);

		Task<IApiResponse<T>> Post<T>(params (string, object)[] parameters);

		Task<IApiResponse<T>> Post<T>(params object[] parameters);

		Task<IEnumerable<IApiResponse>> PostBatch(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse>> PostBatch(params List<object>[] parameters);

		Task<IEnumerable<IApiResponse<T>>> PostBatch<T>(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> PostBatch<T>(params List<object>[] parameters);

		#endregion

		#region Put

		Task<IApiResponse> Put(params (string, object)[] parameters);

		Task<IApiResponse> Put(params object[] parameters);

		Task<IApiResponse<T>> Put<T>(params (string, object)[] parameters);

		Task<IApiResponse<T>> Put<T>(params object[] parameters);

		Task<IEnumerable<IApiResponse>> PutBatch(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse>> PutBatch(params List<object>[] parameters);

		Task<IEnumerable<IApiResponse<T>>> PutBatch<T>(params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> PutBatch<T>(params List<object>[] parameters);

		#endregion

		#region Run

		Task<IApiResponse> Run(OperationType type, params (string, object)[] parameters);

		Task<IApiResponse> Run(OperationType type, params object[] parameters);

		Task<IApiResponse<T>> Run<T>(OperationType type, params (string, object)[] parameters);

		Task<IApiResponse<T>> Run<T>(OperationType type, params object[] parameters);

		Task<IEnumerable<IApiResponse>> RunBatch(OperationType type, params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse>> RunBatch(OperationType type, params List<object>[] parameters);

		Task<IEnumerable<IApiResponse<T>>> RunBatch<T>(OperationType type, params (string, List<object>)[] parameters);

		Task<IEnumerable<IApiResponse<T>>> RunBatch<T>(OperationType type, params List<object>[] parameters);

		#endregion

		/// <summary>
		/// Assign new default users to this path.
		/// </summary>
		/// <param name="users"></param>
		/// <returns></returns>
		IApiPath SetUsers(params string[] users);
	}
}