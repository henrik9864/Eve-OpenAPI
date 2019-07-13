using System.Net.Http;

namespace EveOpenApi.Api
{
	internal interface IApiRequest
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
		/// List of parameters to supply the API
		/// </summary>
		ParsedParameters Parameters { get; }

		/// <summary>
		/// Relative path to the endpoint.
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Scopes for this requrest.
		/// </summary>
		string Scope { get; }

		/// <summary>
		/// Add a parameter to the query of this request.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void AddToQuery(string name, string value);

		int GetHashCode();

		int GetHashCode(int index);

		/// <summary>
		/// Create a request url for a request at index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		string GetRequestUrl(int index);

		/// <summary>
		/// Get use for request at index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		string GetUser(int index);

		/// <summary>
		/// Set a header to a value for this request.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void SetHeader(string name, string value);
	}
}