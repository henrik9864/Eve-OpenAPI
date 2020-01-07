using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace EveOpenApi.Api
{
	internal class ApiRequest : IApiRequest
	{
		public Uri RequestUri { get; private set; }

		public string User { get; }

		public string Scope { get; }

		public IDictionary<string, string> Headers { get; }

		public HttpMethod HttpMethod { get; }

		int computedHashCode;

		public ApiRequest(Uri requestUri, string user, string scope, IDictionary<string, string> headers, HttpMethod httpMethod)
		{
			this.RequestUri = requestUri;
			this.User = user;
			this.Scope = scope;
			this.Headers = headers;
			this.HttpMethod = httpMethod;

			computedHashCode = ComputeHashCode();
		}

		public override int GetHashCode()
		{
			return computedHashCode;
		}

		int ComputeHashCode()
		{
			int hash = 17;
			hash *= 23 + RequestUri.GetHashCode();
			hash *= 23 + HttpMethod.GetHashCode();
			hash *= 23 + User.GetHashCode();

			foreach (var item in Headers.ToList())
			{
				hash *= 23 + item.Key.GetHashCode();
				hash *= 23 + item.Value.GetHashCode();
			}

			return hash;
		}

		/*public string GetRequestUrl(int index)
		{
			string output = $"{BaseUrl}{Path}?";

			// Replace paremetrs in path with correct value
			foreach (var item in Parameters.PathParameters)
				output = output.Replace($"{{{item.Key}}}", $"{FirstOrIndex(item.Value, index)}");

			foreach (var item in Parameters.Queries)
				output += $"{item.Key}={FirstOrIndex(item.Value, index)}&";

			return output[0..^1]; // Removes last &
		}*/

		public void SetParameter(string name, string value)
		{
			RequestUri = AddParameter(RequestUri, name, value);
		}

		public void SetHeader(string name, string value)
		{
			if (Headers.ContainsKey(name))
				Headers[name] = value;
			else
				Headers.Add(name, value);
		}

		/// <summary>
		/// https://stackoverflow.com/questions/14517798/append-values-to-query-string
		/// Adds the specified parameter to the Query String.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="paramName">Name of the parameter to add.</param>
		/// <param name="paramValue">Value for the parameter to add.</param>
		/// <returns>Url with added parameter.</returns>
		Uri AddParameter(Uri uri, string paramName, string paramValue)
		{
			var uriBuilder = new UriBuilder(uri);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query[paramName] = paramValue;
			uriBuilder.Query = query.ToString();

			return uriBuilder.Uri;
		}
	}
}
