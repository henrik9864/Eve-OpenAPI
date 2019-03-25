using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace EveOpenApi.Esi
{
	public class OpenApiPath
	{
		public string Path { get; }

		OpenApiPathItem pathItem;
		OpenApiInterface parent;

		public OpenApiPath(OpenApiInterface parent, string path, OpenApiPathItem pathItem)
		{
			Path = path;
			this.pathItem = pathItem;
			this.parent = parent;
		}

		#region Get

		/// <summary>
		/// Send Get request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<EsiResponse> Get(params (string, object)[] parameters)
		{
			return Run(OperationType.Get, parameters);
		}

		/// <summary>
		/// Send batch Get request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse>> GetBatch(params (string, List<object>)[] parameters)
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
		public Task<EsiResponse<T>> Get<T>(params (string, object)[] parameters)
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
		public Task<List<EsiResponse<T>>> GetBatch<T>(params (string, List<object>)[] parameters)
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
		public Task<EsiResponse> Post(params (string, object)[] parameters)
		{
			return Run(OperationType.Post, parameters);
		}

		/// <summary>
		/// Send batch Post request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse>> PostBatch(params (string, List<object>)[] parameters)
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
		public Task<EsiResponse<T>> Post<T>(params (string, object)[] parameters)
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
		public Task<List<EsiResponse<T>>> PostBatch<T>(params (string, List<object>)[] parameters)
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
		public Task<EsiResponse> Put(params (string, object)[] parameters)
		{
			return Run(OperationType.Put,parameters);
		}

		/// <summary>
		/// Send batch Put request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse>> PutBatch(params (string, List<object>)[] parameters)
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
		public Task<EsiResponse<T>> Put<T>(string user, params (string, object)[] parameters)
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
		public Task<List<EsiResponse<T>>> PutBatch<T>( params (string, List<object>)[] parameters)
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
		public Task<EsiResponse> Delete(params (string, object)[] parameters)
		{
			return Run(OperationType.Delete, parameters);
		}

		/// <summary>
		/// Send batch Delete request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse>> DeleteBatch(params (string, List<object>)[] parameters)
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
		public Task<EsiResponse<T>> Delete<T>(params (string, object)[] parameters)
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
		public Task<List<EsiResponse<T>>> DeleteBatch<T>(params (string, List<object>)[] parameters)
		{
			return RunBatch<T>(OperationType.Delete,parameters);
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
		public Task<List<EsiResponse>> RunBatch(OperationType type, params (string, List<object>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2 );
			return RunBatch(type, convertedParameters);
		}

		/// <summary>
		/// Send one query to esi.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<EsiResponse> Run(OperationType type, params (string, object)[] parameters)
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
		public Task<List<EsiResponse<T>>> RunBatch<T>(OperationType type, params (string, List<object>)[] parameters)
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
		public async Task<EsiResponse<T>> Run<T>(OperationType type, params (string, object)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<object> { a.Item2 });
			return (await RunBatch<T>(type, convertedParameters))[0];
		}

		#endregion

		async Task<List<EsiResponse>> RunBatch(OperationType type, Dictionary<string, List<object>> parameters)
		{
			OpenApiOperation operation = GetOperation(type);
			return await parent.RequestManager.RequestBatch(Path, type, parameters, operation);
		}

		async Task<List<EsiResponse<T>>> RunBatch<T>(OperationType type, Dictionary<string, List<object>> parameters)
		{
			OpenApiOperation operation = GetOperation(type);
			return await parent.RequestManager.RequestBatch<T>(Path, type, parameters, operation);
		}

		OpenApiOperation GetOperation(OperationType type)
		{
			if (!pathItem.Operations.TryGetValue(type, out OpenApiOperation operation))
				throw new Exception($"This path does not have '{type}' operation specified.");

			return operation;
		}
	}
}
