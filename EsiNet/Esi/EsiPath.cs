using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace EsiNet
{
	public class EsiPath
	{
		public string Path { get; }

		OpenApiPathItem pathItem;
		EsiNet parent;

		public EsiPath(EsiNet parent, string path, OpenApiPathItem pathItem)
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
		public Task<EsiResponse> Get(string user, params (string, string)[] parameters)
		{
			return Run(OperationType.Get, user, parameters);
		}

		/// <summary>
		/// Send batch Get request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse>> GetBatch(string user, params (string, List<string>)[] parameters)
		{
			return RunBatch(OperationType.Get, user, parameters);
		}

		/// <summary>
		/// Send Get request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<EsiResponse<T>> Get<T>(string user, params (string, string)[] parameters)
		{
			return Run<T>(OperationType.Get, user, parameters);
		}

		/// <summary>
		/// Send batch Get request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse<T>>> GetBatch<T>(string user, params (string, List<string>)[] parameters)
		{
			return RunBatch<T>(OperationType.Get, user, parameters);
		}

		#endregion

		#region Post

		/// <summary>
		/// Send Post request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<EsiResponse> Post(string user, params (string, string)[] parameters)
		{
			return Run(OperationType.Post, user, parameters);
		}

		/// <summary>
		/// Send batch Post request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse>> PostBatch(string user, params (string, List<string>)[] parameters)
		{
			return RunBatch(OperationType.Post, user, parameters);
		}

		/// <summary>
		/// Send Post request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<EsiResponse<T>> Post<T>(string user, params (string, string)[] parameters)
		{
			return Run<T>(OperationType.Post, user, parameters);
		}

		/// <summary>
		/// Send batch Post request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse<T>>> PostBatch<T>(string user, params (string, List<string>)[] parameters)
		{
			return RunBatch<T>(OperationType.Post, user, parameters);
		}

		#endregion

		#region Put

		/// <summary>
		/// Send Put request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<EsiResponse> Put(string user, params (string, string)[] parameters)
		{
			return Run(OperationType.Put, user, parameters);
		}

		/// <summary>
		/// Send batch Put request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse>> PutBatch(string user, params (string, List<string>)[] parameters)
		{
			return RunBatch(OperationType.Put, user, parameters);
		}

		/// <summary>
		/// Send Put request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<EsiResponse<T>> Put<T>(string user, params (string, string)[] parameters)
		{
			return Run<T>(OperationType.Put, user, parameters);
		}

		/// <summary>
		/// Send batch Put request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse<T>>> PutBatch<T>(string user, params (string, List<string>)[] parameters)
		{
			return RunBatch<T>(OperationType.Put, user, parameters);
		}

		#endregion

		#region Delete

		/// <summary>
		/// Send Delete request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<EsiResponse> Delete(string user, params (string, string)[] parameters)
		{
			return Run(OperationType.Delete, user, parameters);
		}

		/// <summary>
		/// Send batch Delete request to ESI.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse>> DeleteBatch(string user, params (string, List<string>)[] parameters)
		{
			return RunBatch(OperationType.Delete, user, parameters);
		}

		/// <summary>
		/// Send Delete request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<EsiResponse<T>> Delete<T>(string user, params (string, string)[] parameters)
		{
			return Run<T>(OperationType.Delete, user, parameters);
		}

		/// <summary>
		/// Send batch Delete request to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse<T>>> DeleteBatch<T>(string user, params (string, List<string>)[] parameters)
		{
			return RunBatch<T>(OperationType.Delete, user, parameters);
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
		public Task<List<EsiResponse>> RunBatch(OperationType type, string user, params (string, List<string>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2 );
			return RunBatch(type, user, convertedParameters);
		}

		/// <summary>
		/// Send one query to esi.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<EsiResponse> Run(OperationType type, string user, params (string, string)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<string> { a.Item2 });
			return (await RunBatch(type, user, convertedParameters))[0];
		}

		/// <summary>
		/// Send a batch of queries to ESI.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="type"></param>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Task<List<EsiResponse<T>>> RunBatch<T>(OperationType type, string user, params (string, List<string>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2);
			return RunBatch<T>(type, user, convertedParameters);
		}

		/// <summary>
		/// Send one query to esi.
		/// </summary>
		/// <typeparam name="T">Esi response return type.</typeparam>
		/// <param name="type"></param>
		/// <param name="user"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<EsiResponse<T>> Run<T>(OperationType type, string user, params (string, string)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<string> { a.Item2 });
			return (await RunBatch<T>(type, user, convertedParameters))[0];
		}

		#endregion

		async Task<List<EsiResponse>> RunBatch(OperationType type, string user, Dictionary<string, List<string>> parameters)
		{
			OpenApiOperation operation = GetOperation(type);
			return await parent.RequestManager.RequestBatch(Path, user, type, parameters, operation);
		}

		async Task<List<EsiResponse<T>>> RunBatch<T>(OperationType type, string user, Dictionary<string, List<string>> parameters)
		{
			OpenApiOperation operation = GetOperation(type);
			return await parent.RequestManager.RequestBatch<T>(Path, user, type, parameters, operation);
		}

		OpenApiOperation GetOperation(OperationType type)
		{
			if (!pathItem.Operations.TryGetValue(type, out OpenApiOperation operation))
				throw new Exception($"This path does not have '{type}' operation specified.");

			return operation;
		}
	}
}
