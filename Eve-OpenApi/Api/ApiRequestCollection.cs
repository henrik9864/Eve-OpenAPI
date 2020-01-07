using System.Collections;
using System.Collections.Generic;
using System.Net.Http;

namespace EveOpenApi.Api
{
	internal class ApiRequestCollection : IApiRequestCollection
	{
		public string BaseUrl { get; }

		public HttpMethod Method { get; }

		public string Path { get; }

		public string Scope { get; }

		public ParsedParameters Parameters { get; }

		public int Requests
		{
			get
			{
				return Parameters.MaxLength;
			}
		}

		public ApiRequestCollection(string baseUrl, HttpMethod method, string path, string scope, ParsedParameters parameters)
		{
			this.BaseUrl = baseUrl;
			this.Method = method;
			this.Path = path;
			this.Scope = scope;
			this.Parameters = parameters;
		}

		public IEnumerator<IApiRequest> GetEnumerator()
		{
			throw new System.NotImplementedException();
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

		public string GetUser(int index)
		{
			return FirstOrIndex(Parameters.Users, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// If the list only has one item always use the first item.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		T FirstOrIndex<T>(List<T> list, int index)
		{
			if (list.Count == 1)
				return list[0];

			return list[index];
		}
	}
}
