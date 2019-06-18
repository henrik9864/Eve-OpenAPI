using EveOpenApi.Api;
using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
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
	internal class CacheManager : BaseManager
	{
		Dictionary<int, ApiResponse> cache = new Dictionary<int, ApiResponse>();
		RequestQueueAsync<ApiRequest, List<ApiResponse>> requestQueue;

		public CacheManager(HttpClient client, API api) : base(client, api)
		{
			requestQueue = new RequestQueueAsync<ApiRequest, List<ApiResponse>>(ProcessResponse);
		}

		/// <summary>
		/// Get response from ESI.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task<List<ApiResponse>> GetResponse(ApiRequest request)
		{
			return ExecuteRequest(request);
		}

		public async Task<List<ApiResponse<T>>> GetResponse<T>(ApiRequest request)
		{
			List<ApiResponse> responses = await ExecuteRequest(request);

			List<ApiResponse<T>> returnResponses = new List<ApiResponse<T>>();
			for (int i = 0; i < responses.Count; i++)
				returnResponses.Add(responses[i].ToType<T>());

			return returnResponses;
		}

		public async Task<ApiResponse> GetResponse(ApiRequest request, int index)
		{
			await API.TokenManager.AddAuthTokens(request);
			return await ProcessResponse(request, index);
		}

		public async Task<ApiResponse<T>> GetResponse<T>(ApiRequest request, int index)
		{
			await API.TokenManager.AddAuthTokens(request);
			ApiResponse response = await ProcessResponse(request, index);
			return response.ToType<T>();
		}

		async Task<List<ApiResponse>> ExecuteRequest(ApiRequest request)
		{
			if (API.Config.UseRequestQueue)
			{
				int id = await AddToRequestQueue(request);
				return await requestQueue.AwaitResponse(id);
			}
			else
			{
				return await ProcessResponse(request);
			}
		}

		async Task<int> AddToRequestQueue(ApiRequest request)
		{
			await API.TokenManager.AddAuthTokens(request);

			if (string.IsNullOrEmpty(API.Config.UserAgent))
				throw new Exception("User-Agent must be set.");

			request.SetHeader("User-Agent", API.Config.UserAgent);

			return requestQueue.AddRequest(request);
		}

		/// <summary>
		/// Check if request has been cached.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		public bool TryHitCache(ApiRequest request, int index, bool validateTime, out ApiResponse response)
		{
			if (cache.TryGetValue(request.GetHashCode(index), out response) && (!validateTime || DateTime.UtcNow < response.Expired))
				return true;

			return false;
		}

		/// <summary>
		/// Save response from url to cache.
		/// </summary>
		/// <param name="requestUrl"></param>
		/// <param name="response"></param>
		void SaveToCache(ApiRequest request, int index, ApiResponse response)
		{
			if (!cache.TryAdd(request.GetHashCode(), response))
				cache[request.GetHashCode(index)] = response;
		}

		/// <summary>
		/// Tries to retrive an eTag from a response.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="index"></param>
		/// <param name="eTag"></param>
		/// <returns></returns>
		bool TryGetETag(ApiRequest request, int index, out string eTag)
		{
			if (cache.TryGetValue(request.GetHashCode(index), out ApiResponse response))
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
		async Task<List<ApiResponse>> ProcessResponse(ApiRequest request)
		{
			List<ApiResponse> esiResponses = new List<ApiResponse>();
			for (int i = 0; i < request.Parameters.MaxLength; i++)
				esiResponses.Add(await ProcessResponse(request, i));

			return esiResponses;
		}

		async Task<ApiResponse> ProcessResponse(ApiRequest request, int index)
		{
			if (API.Config.UseCache && TryHitCache(request, index, true, out ApiResponse response))
				return response;

			TryGetETag(request, index, out string eTag);
			request.SetHeader("If-None-Match", eTag);

			response = await API.ResponseManager.GetResponse(request, index);
			if (response is ApiError)
			{
				ApiError error = response as ApiError;
				if (error.StatusCode == System.Net.HttpStatusCode.NotModified)
				{
					TryHitCache(request, index, false, out response);
					response.UpdateExpiery(error);
				}
			}
			
			if (response.Expired != default && (Cacheability.Public | Cacheability.Private).HasFlag(response.CacheControl.Cacheability))
				SaveToCache(request, index, response);

			return response;
		}
	}
}
