using EveOpenApi.Api;
using EveOpenApi.Authentication;
using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Managers
{
	internal class CacheManager : BaseManager, ICacheManager
	{
		IMemoryCache cache;
		RequestQueueAsync<IApiRequest, IApiResponse> requestQueue;

		ITokenManager tokenManager;
		IResponseManager responseManager;

		public CacheManager(IHttpHandler client, IApiConfig config, ILogin login, IMemoryCache memoryCache, ITokenManager tokenManager, IResponseManager responseManager) : base(client, login, config)
		{
			requestQueue = new RequestQueueAsync<IApiRequest, IApiResponse>(ProcessResponse);
			cache = memoryCache;
			this.tokenManager = tokenManager;
			this.responseManager = responseManager;
		}

		/// <summary>
		/// Get response from ESI.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task<IEnumerable<IApiResponse>> GetResponse(IEnumerable<IApiRequest> request)
		{
			return ExecuteRequest(request);
		}

		public async Task<IEnumerable<IApiResponse<T>>> GetResponse<T>(IEnumerable<IApiRequest> request)
		{
			IEnumerable<IApiResponse> responses = await GetResponse(request);
			return responses.Select(x => x.ToType<T>());
		}

		public Task<IApiResponse> GetResponse(IApiRequest request)
		{
			return ExecuteRequest(request);
		}

		public async Task<IApiResponse<T>> GetResponse<T>(IApiRequest request)
		{
			IApiResponse response = await GetResponse(request);
			return response.ToType<T>();
		}

		async Task<IApiResponse> ExecuteRequest(IApiRequest request)
		{
			if (Config.UseRequestQueue)
			{
				int id = await AddToRequestQueue(request);
				return await requestQueue.AwaitResponse(id);
			}
			else
			{
				return await ProcessResponse(request);
			}
		}

		async Task<IEnumerable<IApiResponse>> ExecuteRequest(IEnumerable<IApiRequest> requests)
		{
			List<int> ids = new List<int>();
			foreach (IApiRequest request in requests)
				ids.Add(await AddToRequestQueue(request));

			IApiResponse[] responses = new IApiResponse[ids.Count];
			for (int i = 0; i < ids.Count; i++)
				responses[i] = await requestQueue.AwaitResponse(ids[i]);

			return responses;
		}

		async Task<int> AddToRequestQueue(IApiRequest request)
		{
			await tokenManager.AddAuthTokens(request);

			if (string.IsNullOrEmpty(Config.UserAgent))
				throw new Exception("User-Agent must be set.");

			request.SetHeader("User-Agent", Config.UserAgent);

			return requestQueue.AddRequest(request);
		}

		/// <summary>
		/// Check if request has been cached. And not expired
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		public bool TryHitCache(IApiRequest request, bool validateTime, out IApiResponse response)
		{
			return cache.TryGetValue(request.GetHashCode(), out response) && (!validateTime || DateTime.UtcNow < response.Expired);
		}

		/// <summary>
		/// Save response from url to cache.
		/// </summary>
		/// <param name="requestUrl"></param>
		/// <param name="response"></param>
		void SaveToCache(IApiRequest request, IApiResponse response)
		{
			cache.Set(request.GetHashCode(), response, response.Expired);
		}

		/// <summary>
		/// Tries to retrive an eTag from a response.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="index"></param>
		/// <param name="eTag"></param>
		/// <returns></returns>
		bool TryGetETag(IApiRequest request, out string eTag)
		{
			if (cache.TryGetValue(request.GetHashCode(), out IApiResponse response))
			{
				eTag = response.ETag;
				return true;
			}

			eTag = "";
			return false;
		}

		/// <summary>
		/// Execute EsiRequest.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		/*async Task<IApiResponse> ProcessResponse(IApiRequest request)
		{
			IList<IApiResponse> esiResponses = new List<IApiResponse>();
			for (int i = 0; i < request.Parameters.MaxLength; i++)
				esiResponses.Add(await ProcessResponse(request, i));

			return esiResponses;
		}*/

		async Task<IApiResponse> ProcessResponse(IApiRequest request)
		{
			if (Config.UseCache && TryHitCache(request, true, out IApiResponse response))
				return response;

			TryGetETag(request, out string eTag);
			request.SetHeader("If-None-Match", eTag);

			response = await responseManager.GetResponse(request);
			if (response is ApiError)
			{
				ApiError error = response as ApiError;
				if (error.StatusCode == System.Net.HttpStatusCode.NotModified)
				{
					TryHitCache(request, false, out response);
					response.Expired = error.Expired;
				}
			}

			if (response.Expired != default && (Cacheability.Public | Cacheability.Private).HasFlag(response.CacheControl.Cacheability))
				SaveToCache(request, response);

			return response;
		}
	}
}
