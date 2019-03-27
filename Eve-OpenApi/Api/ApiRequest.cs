using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi.Api
{
	internal class ApiRequest
	{
		public string BaseUrl { get; }

		public string Path { get; }

		public string Scope { get; }

		public HttpMethod Method { get; }

		public ParsedParameters Parameters { get; }

		public ApiRequest(string baseUrl, string path, string scope, HttpMethod method, ParsedParameters parametesr)
		{
			BaseUrl = baseUrl;
			Path = path;
			Scope = scope;
			Method = method;
			Parameters = parametesr;
		}

		public string GetRequestUrl(int index)
		{
			string output = $"{BaseUrl}{Path}?";

			foreach (var item in Parameters.PathParameters)
				output = output.Replace($"{{{item.Key}}}", $"{IndexOrLast(item.Value, index)}");

			foreach (var item in Parameters.Queries)
				output += $"{item.Key}={IndexOrLast(item.Value, index)}&";

			return output.Substring(0, output.Length - 1);
		}

		public void AddQuery(string name, string value)
		{
			Parameters.Queries.Add(new KeyValuePair<string, List<string>>(name, new List<string> { value }));
		}

		public void SetHeader(string name, string value)
		{
			var kvp = new KeyValuePair<string, List<string>>(name, new List<string> { value });
			int kvpIndex = Parameters.Headers.FindIndex(a => a.Key == name);

			if (kvpIndex > -1)
				Parameters.Headers[kvpIndex] = kvp;
			else
				Parameters.Headers.Add(new KeyValuePair<string, List<string>>(name, new List<string> { value }));
		}

		T IndexOrLast<T>(List<T> list, int index)
		{
			if (list.Count <= index)
				return list[list.Count - 1];

			return list[index];
		}
	}
}
