using EveOpenApi.Api;
using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using SharpYaml.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EveOpenApi.Managers
{
	internal class CacheManager : BaseManager, ICacheManager
	{
		IMemoryCache cache;
		RequestQueueAsync<IApiRequest, IList<IApiResponse>> requestQueue;

		ITokenManager tokenManager;
		IResponseManager responseManager;

		public CacheManager(HttpClient client, IApiConfig config, ILogin login, IMemoryCache memoryCache, ITokenManager tokenManager, IResponseManager responseManager) : base(client, login, config)
		{
			requestQueue = new RequestQueueAsync<IApiRequest, IList<IApiResponse>>(ProcessResponse);
			cache = memoryCache;
			this.tokenManager = tokenManager;
			this.responseManager = responseManager;
		}

		/// <summary>
		/// Get response from ESI.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task<IList<IApiResponse>> GetResponse(IApiRequest request)
		{
			return ExecuteRequest(request);
		}

		public async Task<IList<IApiResponse<T>>> GetResponse<T>(IApiRequest request)
		{
			IList<IApiResponse> responses = await ExecuteRequest(request);

			IList<IApiResponse<T>> returnResponses = new List<IApiResponse<T>>();
			for (int i = 0; i < responses.Count; i++)
				returnResponses.Add(responses[i].ToType<T>());

			return returnResponses;
		}

		public async Task<IApiResponse> GetResponse(IApiRequest request, int index)
		{
			await tokenManager.AddAuthTokens(request);
			return await ProcessResponse(request, index);
		}

		public async Task<IApiResponse<T>> GetResponse<T>(IApiRequest request, int index)
		{
			await tokenManager.AddAuthTokens(request);
			IApiResponse response = await ProcessResponse(request, index);
			return response.ToType<T>();
		}

		async Task<IList<IApiResponse>> ExecuteRequest(IApiRequest request)
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
		public bool TryHitCache(IApiRequest request, int index, bool validateTime, out IApiResponse response)
		{
			return cache.TryGetValue(request.GetHashCode(index), out response) && (!validateTime || DateTime.UtcNow < response.Expired);
		}

		/// <summary>
		/// Save response from url to cache.
		/// </summary>
		/// <param name="requestUrl"></param>
		/// <param name="response"></param>
		void SaveToCache(IApiRequest request, int index, IApiResponse response)
		{
			cache.Set(request.GetHashCode(index), response, response.Expired);
		}

		/// <summary>
		/// Tries to retrive an eTag from a response.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="index"></param>
		/// <param name="eTag"></param>
		/// <returns></returns>
		bool TryGetETag(IApiRequest request, int index, out string eTag)
		{
			if (cache.TryGetValue(request.GetHashCode(index), out IApiResponse response))
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
		async Task<IList<IApiResponse>> ProcessResponse(IApiRequest request)
		{
			IList<IApiResponse> esiResponses = new List<IApiResponse>();
			for (int i = 0; i < request.Parameters.MaxLength; i++)
				esiResponses.Add(await ProcessResponse(request, i));

			return esiResponses;
		}

		async Task<IApiResponse> ProcessResponse(IApiRequest request, int index)
		{
			if (Config.UseCache && TryHitCache(request, index, true, out IApiResponse response))
				return response;

			TryGetETag(request, index, out string eTag);
			request.SetHeader("If-None-Match", eTag);

			response = await responseManager.GetResponse(request, index);
			if (response is ApiError)
			{
				ApiError error = response as ApiError;
				if (error.StatusCode == System.Net.HttpStatusCode.NotModified)
				{
					TryHitCache(request, index, false, out response);
					response.Expired = error.Expired;
				}
			}

			if (response.Expired != default && (Cacheability.Public | Cacheability.Private).HasFlag(response.CacheControl.Cacheability))
				SaveToCache(request, index, response);

			return response;
		}
	}
}
