using System.Collections.Generic;
using System.Net.Http;

namespace EveOpenApi.Api
{
	internal interface IApiRequestCollection : IEnumerable<IApiRequest>
	{
		/// <summary>
		/// Domain URI to the API
		/// </summary>
		string BaseUrl { get; }

		/// <summary>
		/// What method to use on the endpoint
		/// </summary>
		HttpMethod Method { get; }

		/// <summary>
		/// Relative path to the endpoint.
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Scopes for this requrest.
		/// </summary>
		string Scope { get; }

		/// <summary>
		/// List of parameters to supply the API
		/// </summary>
		ParsedParameters Parameters { get; }

		/// <summary>
		/// Amount of requests in this collection
		/// </summary>
		int Requests { get; }

		/// <summary>
		/// Get use for request at index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		string GetUser(int index);

		int GetHashCode();

		int GetHashCode(int index);
	}
}