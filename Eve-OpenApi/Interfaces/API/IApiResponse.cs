using EveOpenApi.Managers.CacheControl;
using System;

namespace EveOpenApi.Api
{
	public interface IApiResponse
	{
		/// <summary>
		/// Return E-Tag from the API
		/// </summary>
		string ETag { get; }

		/// <summary>
		/// When this response is stale
		/// </summary>
		DateTime Expired { get; internal set; }

		/// <summary>
		/// Raw response from the API
		/// </summary>
		string Response { get; }

		CacheControl CacheControl { get; }

		int GetHashCode();

		/// <summary>
		/// Json deserialize the response to type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IApiResponse<T> ToType<T>();
	}
}