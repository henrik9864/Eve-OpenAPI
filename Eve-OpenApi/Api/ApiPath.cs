using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using EveOpenApi.Interfaces;
using EveOpenApi.Managers;

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
		/// Send Get request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Get(params (string, object)[] parameters)
		{
			return Run(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send batch Get request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse>> GetBatch(params (string, List<object>)[] parameters)
		{
			return RunBatch(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send Get request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Get<T>(params (string, object)[] parameters)
		{
			return Run<T>(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send batch Get request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse<T>>> GetBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Get, parameters);
		}

		#endregion

		#region Post

		/// <summary>
		/// Send Post request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Post(params (string, object)[] parameters)
		{
			return Run(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send batch Post request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse>> PostBatch(params (string, List<object>)[] parameters)
		{
			return RunBatch(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send Post request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Post<T>(params (string, object)[] parameters)
		{
			return Run<T>(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send batch Post request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse<T>>> PostBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Post, parameters);
		}

		#endregion

		#region Put

		/// <summary>
		/// Send Put request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Put(params (string, object)[] parameters)
		{
			return Run(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send batch Put request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse>> PutBatch(params (string, List<object>)[] parameters)
		{
			return RunBatch(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send Put request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Put<T>(string user, params (string, object)[] parameters)
		{
			return Run<T>(OperationType.Put, parameters);
		}

		/// <summary>
		/// Send batch Put request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse<T>>> PutBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Put, parameters);
		}

		#endregion

		#region Delete

		/// <summary>
		/// Send Delete request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse> Delete(params (string, object)[] parameters)
		{
			return Run(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send batch Delete request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse>> DeleteBatch(params (string, List<object>)[] parameters)
		{
			return RunBatch(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send Delete request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IApiResponse<T>> Delete<T>(params (string, object)[] parameters)
		{
			return Run<T>(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send batch Delete request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse<T>>> DeleteBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Delete, parameters);
		}

		#endregion

		#region Run

		/// <summary>
		/// Send a batch of queries to ESI.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse>> RunBatch(OperationType type, params (string, List<object>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2);
			return RunBatch(type, convertedParameters);
		}

		/// <summary>
		/// Send one query to esi.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<IApiResponse> Run(OperationType type, params (string, object)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<object> { a.Item2 });
			return (await RunBatch(type, convertedParameters))[0];
		}

		/// <summary>
		/// Send a batch of queries to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="type"></param>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse<T>>> RunBatch<T>(OperationType type, params (string, List<object>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2);
			return RunBatch<T>(type, convertedParameters);
		}

		/// <summary>
		/// Send one query to esi.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="type"></param>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<IApiResponse<T>> Run<T>(OperationType type, params (string, object)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<object> { a.Item2 });
			return (await RunBatch<T>(type, convertedParameters))[0];
		}

		#endregion

		async Task<IList<IApiResponse>> RunBatch(OperationType type, Dictionary<string, List<object>> parameters)
		{
			OpenApiOperation operation = GetOperation(type);
			return await requestManager.RequestBatch(Path, type, parameters, DefaultUsers, operation);
		}

		async Task<IList<IApiResponse<T>>> RunBatch<T>(OperationType type, Dictionary<string, List<object>> parameters)
		{
			OpenApiOperation operation = GetOperation(type);
			return await requestManager.RequestBatch<T>(Path, type, parameters, DefaultUsers, operation);
		}

		OpenApiOperation GetOperation(OperationType type)
		{
			if (!pathItem.Operations.TryGetValue(type, out OpenApiOperation operation))
				throw new Exception($"This path does not have '{type}' operation specified.");

			return operation;
		}
	}
}
