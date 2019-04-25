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

		public ApiRequest(string baseUrl, string path, string scope, HttpMethod method, ParsedParameters parameters)
		{
			BaseUrl = baseUrl;
			Path = path;
			Scope = scope;
			Method = method;
			Parameters = parameters;
		}

		public override int GetHashCode()
		{
			return GetHashCode(0);
		}

		public int GetHashCode(int index)
		{
			int hash = 17;
			hash *= 23 + BaseUrl.GetHashCode();
			hash *= 23 + Path.GetHashCode();
			hash *= 23 + Scope.GetHashCode();
			hash *= 23 + Method.GetHashCode();
			hash *= 23 + Parameters.GetHashCode(index);

			return hash;
		}

		public string GetRequestUrl(int index)
		{
			string output = $"{BaseUrl}{Path}?";

			foreach (var item in Parameters.PathParameters)
				output = output.Replace($"{{{item.Key}}}", $"{IndexOrLast(item.Value, index)}");

			foreach (var item in Parameters.Queries)
			{
				Console.WriteLine($"	{item.Key}: {item.Value.Count}");
				output += $"{item.Key}={IndexOrLast(item.Value, index)}&";
			}

			return output.Substring(0, output.Length - 1);
		}

		public string GetUser(int index)
		{
			if (Parameters.Users.Count == 1)
				return Parameters.Users[0];

			return Parameters.Users[index];
		}

		public void AddToQuery(string name, string value)
		{
			int kvpIndex = Parameters.Queries.FindIndex(x => x.Key == name);

			Console.WriteLine($"Added {kvpIndex} {name}");

			if (kvpIndex == -1)
				Parameters.Queries.Add(new KeyValuePair<string, List<string>>(name, new List<string> { value }));
			else
				Parameters.Queries[kvpIndex].Value.Add(value);
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
