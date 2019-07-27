using EveOpenApi.Api;
using EveOpenApi.Authentication;
using EveOpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Managers
{
	internal class RequestManager : BaseManager, IRequestManager
	{
		ICacheManager cacheManager;
		OpenApiDocument spec;

		public RequestManager(IHttpHandler client, IApiConfig config, ILogin login, ICacheManager cacheManager, OpenApiDocument spec) : base(client, login, config)
		{
			this.cacheManager = cacheManager;
			this.spec = spec;
		}

		/// <summary>
		/// Request multiple queries for the same path.
		/// </summary>
		/// <param name="path">Esi path</param>
		/// <param name="user">User preforming this query.</param>
		/// <param name="type">Operation Type.</param>
		/// <param name="parameters">Parameters supplide by the user.</param>
		/// <param name="operation">OpenAPI operation for this path.</param>
		/// <returns></returns>
		public async Task<IList<IApiResponse>> RequestBatch(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation)
		{
			ApiRequest request = GetRequest(path, type, parameters, users, operation);
			return await cacheManager.GetResponse(request);
		}

		/// <summary>
		/// Request multiple queries for the same path.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="path">Esi path</param>
		/// <param name="user">User preforming this query.</param>
		/// <param name="type">Operation Type.</param>
		/// <param name="parameters">Parameters supplide by the user.</param>
		/// <param name="operation">OpenAPI operation for this path.</param>
		/// <returns></returns>
		public async Task<IList<IApiResponse<T>>> RequestBatch<T>(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation)
		{
			ApiRequest request = GetRequest(path, type, parameters, users, operation);
			return await cacheManager.GetResponse<T>(request);
		}

		public ApiRequest GetRequest(string path, OperationType type, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation)
		{
			var parsed = ParseParameters(operation, parameters, users);
			string baseUrl = $"{spec.Servers[0].Url}";
			string scope = GetScope(operation);
			HttpMethod httpMethod = OperationToMethod(type);

			return new ApiRequest(baseUrl, path, scope, httpMethod, parsed);
		}

		/// <summary>
		/// Sort parameters into their respective group.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		ParsedParameters ParseParameters(OpenApiOperation operation, Dictionary<string, List<object>> parameters, List<string> users)
		{
			int maxLength = 1;
			var queries = new List<KeyValuePair<string, List<string>>>();
			var headers = new List<KeyValuePair<string, List<string>>>();
			var pathParameters = new List<KeyValuePair<string, List<string>>>();

			foreach (var item in operation.Parameters)
			{
				bool found = parameters.TryGetValue(item.Name, out List<object> value);

				if (found)
				{
					if (maxLength == 1 && value.Count > maxLength)
						maxLength = value.Count;
					else if (maxLength > 1 && value.Count != maxLength)
						throw new Exception("Every batch parameter must have 1 or the same count");

					var kvp = new KeyValuePair<string, List<string>>(item.Name, value.Select(a => a.ToString()).ToList());
					switch (item.In)
					{
						case ParameterLocation.Query:
							queries.Add(kvp);
							break;
						case ParameterLocation.Path:
							pathParameters.Add(kvp);
							break;
						case ParameterLocation.Header:
							headers.Add(kvp);
							break;
						default:
							break;
					}
				}
				else if (item.Required && Login?.Config.TokenLocation != "query" && Login?.Config.TokenName == item.Name)
					throw new Exception($"Required parameter '{item.Name}' not supplied.");
			}

			if (users.Count != 1 && users.Count != maxLength)
				throw new Exception("Number of users must be 1 or same count as batch parameters");

			return new ParsedParameters(maxLength, queries, headers, pathParameters, users);
		}

		/// <summary>
		/// Try get scope from operation.
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		string GetScope(OpenApiOperation operation)
		{
			List<string> scopes = operation.Security?.FirstOrDefault()?.FirstOrDefault().Value as List<string>;

			if (scopes != null && scopes.Count > 0)
				return scopes[0];

			return "";
		}

		/// <summary>
		/// Convert OperationType to HttpMethod
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		HttpMethod OperationToMethod(OperationType operation)
		{
			switch (operation)
			{
				case OperationType.Get:
					return HttpMethod.Get;
				case OperationType.Put:
					return HttpMethod.Put;
				case OperationType.Post:
					return HttpMethod.Post;
				case OperationType.Delete:
					return HttpMethod.Delete;
				case OperationType.Options:
					return HttpMethod.Options;
				case OperationType.Head:
					return HttpMethod.Head;
				case OperationType.Patch:
					return HttpMethod.Patch;
				case OperationType.Trace:
					return HttpMethod.Trace;
				default:
					throw new Exception("Dafuq");
			}
		}
	}
}
