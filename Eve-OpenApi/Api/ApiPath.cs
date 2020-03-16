using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using System.Collections;

namespace EveOpenApi.Api
{
	public class ApiPath : IApiPath
	{
		public string Path { get; }

		public List<string> DefaultUsers { get; private set; }

		OpenApiPathItem pathItem;

		IRequestManager requestManager;

		internal ApiPath(string path, string defaultUser, OpenApiPathItem pathItem, IRequestManager requestManager)
		{
			Path = path;
			this.pathItem = pathItem;
			this.requestManager = requestManager;
			DefaultUsers = new List<string>() { defaultUser };
		}

		public IApiPath SetUsers(params string[] users)
		{
			if (users.Any(x => string.IsNullOrEmpty(x)))
				throw new Exception("Users cannot be null or empty");

			DefaultUsers = new List<string>();
			DefaultUsers.AddRange(users);

			return this;
		}

		#region Get

		/// <summary>
		/// Send Get request to the API.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Get(params (string, object)[] parameters)
		{
			return Run(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send Get request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Get(params object[] parameters)
		{
			return Run(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send batch Get request to the API.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> GetBatch(params (string, List<object>)[] parameters)
		{
			return RunBatch(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send batch Get request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> GetBatch(params List<object>[] parameters)
		{
			return RunBatch(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send Get request to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Get<T>(params (string, object)[] parameters)
		{
			return Run<T>(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send Get request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Get<T>(params object[] parameters)
		{
			return Run<T>(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send batch Get request to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> GetBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send batch Get request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> GetBatch<T>(params List<object>[] parameters)
		{
			return RunBatch<T>(OperationType.Get, parameters);
		}

		#endregion

		#region Post

		/// <summary>
		/// Send Post request to the API.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Post(params (string, object)[] parameters)
		{
			return Run(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send Post request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Post(params object[] parameters)
		{
			return Run(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send batch Post request to the API.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> PostBatch(params (string, List<object>)[] parameters)
		{
			return RunBatch(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send batch Post request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> PostBatch(params List<object>[] parameters)
		{
			return RunBatch(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send Post request to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Post<T>(params (string, object)[] parameters)
		{
			return Run<T>(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send Post request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Post<T>(params object[] parameters)
		{
			return Run<T>(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send batch Post request to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> PostBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send batch Post request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> PostBatch<T>(params List<object>[] parameters)
		{
			return RunBatch<T>(OperationType.Post, parameters);
		}

		#endregion

		#region Put

		/// <summary>
		/// Send Put request to the API.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Put(params (string, object)[] parameters)
		{
			return Run(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send Put request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Put(params object[] parameters)
		{
			return Run(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send batch Put request to the API.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> PutBatch(params (string, List<object>)[] parameters)
		{
			return RunBatch(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send batch Put request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> PutBatch(params List<object>[] parameters)
		{
			return RunBatch(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send Put request to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Put<T>(params (string, object)[] parameters)
		{
			return Run<T>(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send Put request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Put<T>(params object[] parameters)
		{
			return Run<T>(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send batch Put request to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> PutBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send batch Put request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> PutBatch<T>(params List<object>[] parameters)
		{
			return RunBatch<T>(OperationType.Put, parameters);
		}

		#endregion

		#region Delete

		/// <summary>
		/// Send Delete request to the API.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Delete(params (string, object)[] parameters)
		{
			return Run(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send Delete request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Delete(params object[] parameters)
		{
			return Run(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send batch Delete request to the API.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> DeleteBatch(params (string, List<object>)[] parameters)
		{
			return RunBatch(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send batch Delete request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> DeleteBatch(params List<object>[] parameters)
		{
			return RunBatch(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send Delete request to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Delete<T>(params (string, object)[] parameters)
		{
			return Run<T>(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send Delete request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Delete<T>(params object[] parameters)
		{
			return Run<T>(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send batch Delete request to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> DeleteBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send batch Delete request to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> DeleteBatch<T>(params List<object>[] parameters)
		{
			return RunBatch<T>(OperationType.Delete, parameters);
		}

		#endregion

		#region Run

		/// <summary>
		/// Send a batch of queries to the API.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> RunBatch(OperationType type, params (string, List<object>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2);
			return RunBatch(type, convertedParameters);
		}

		/// <summary>
		/// Send a batch of queries to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> RunBatch(OperationType type, params List<object>[] parameters)
		{
			var convertedParameters = parameters.ToList();
			return RunBatch(type, convertedParameters);
		}

		/// <summary>
		/// Send one query to the API.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<IApiResponse> Run(OperationType type, params (string, object)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<object> { a.Item2 });
			return (await RunBatch(type, convertedParameters)).FirstOrDefault();
		}

		/// <summary>
		/// Send one query to the API. Parameters will be matched based on order.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<IApiResponse> Run(OperationType type, params object[] parameters)
		{
			var convertedParameters = parameters.Select(x => new List<object>() { x }).ToList();
			return (await RunBatch(type, convertedParameters)).FirstOrDefault();

		}

		/// <summary>
		/// Send a batch of queries to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> RunBatch<T>(OperationType type, params (string, List<object>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2);
			return RunBatch<T>(type, convertedParameters);
		}

		/// <summary>
		/// Send a batch of queries to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse<T>>> RunBatch<T>(OperationType type, params List<object>[] parameters)
		{
			var convertedParameters = parameters.ToList();
			return RunBatch<T>(type, convertedParameters);
		}

		/// <summary>
		/// Send one query to the API.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<IApiResponse<T>> Run<T>(OperationType type, params (string, object)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<object> { a.Item2 });
			return (await RunBatch<T>(type, convertedParameters)).FirstOrDefault();
		}

		/// <summary>
		/// Send one query to the API. Parameters will be matched based on order.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="type"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<IApiResponse<T>> Run<T>(OperationType type, params object[] parameters)
		{
			var convertedParameters = parameters.Select(x => new List<object>() { x }).ToList();
			return (await RunBatch<T>(type, convertedParameters)).FirstOrDefault();
		}

		#endregion

		Task<IEnumerable<IApiResponse>> RunBatch(OperationType type, Dictionary<string, List<object>> parameters)
		{
			OpenApiOperation operation = GetOperation(type);
			return requestManager.RequestBatch(Path, type, parameters, DefaultUsers, operation);
		}

		Task<IEnumerable<IApiResponse>> RunBatch(OperationType type, List<List<object>> parameters)
		{
			Dictionary<string, List<object>> namedParameters = ResolveParameterName(type, parameters);
			return RunBatch(type, namedParameters);
		}

		Task<IEnumerable<IApiResponse<T>>> RunBatch<T>(OperationType type, Dictionary<string, List<object>> parameters)
		{
			OpenApiOperation operation = GetOperation(type);
			return requestManager.RequestBatch<T>(Path, type, parameters, DefaultUsers, operation);
		}

		Task<IEnumerable<IApiResponse<T>>> RunBatch<T>(OperationType type, List<List<object>> parameters)
		{
			Dictionary<string, List<object>> namedParameters = ResolveParameterName(type, parameters);
			return RunBatch<T>(type, namedParameters);
		}

		OpenApiOperation GetOperation(OperationType type)
		{
			if (!pathItem.Operations.TryGetValue(type, out OpenApiOperation operation))
				throw new Exception($"This path does not have '{type}' operation specified.");

			return operation;
		}

		Dictionary<string, List<object>> ResolveParameterName(OperationType type, List<List<object>> param)
		{
			OpenApiOperation operation = GetOperation(type);
			List<string> paramNames = GetParameterNames(operation);

			if (paramNames.Count > param.Count)
				throw new Exception($"Not enough parameters for {paramNames.Count} expected but got {param.Count}");
			else if (paramNames.Count < param.Count)
				throw new Exception($"Too many parameters for {paramNames.Count} expected but got {param.Count}");

			Dictionary<string, List<object>> parameters = new Dictionary<string, List<object>>();
			for (int i = 0; i < paramNames.Count; i++)
				parameters.Add(paramNames[i], param[i]);

			return parameters;
		}

		List<string> GetParameterNames(OpenApiOperation operation)
		{
			return operation.Parameters
				.Select(x => x.Name)																							// Select the names
				.Where(x => x != "datasource" && x != "If-None-Match" && x != "token" && x != "user_agent" && x != "page")		// Filter our parameters that are automaticly filled
				.ToList();
		}
	}
}
