using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EveOpenApi.Api
{
	internal interface IApiRequest
	{
		public Uri RequestUri { get; }

		public HttpMethod HttpMethod { get; }

		public string User { get; }

		public string Scope { get; }

		public IDictionary<string, string> Headers { get; }

		/// <summary>
		/// Add a parameter to the query of this request.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void SetParameter(string name, string value);

		/// <summary>
		/// Set a header to a value for this request.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void SetHeader(string name, string value);
	}
}