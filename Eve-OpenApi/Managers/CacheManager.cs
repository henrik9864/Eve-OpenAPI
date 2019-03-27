using EveOpenApi.Api;
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
		Dictionary<string, ApiResponse> cache = new Dictionary<string, ApiResponse>();
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

		async Task<List<ApiResponse>> ExecuteRequest(ApiRequest request)
		{
			if (API.Config.UseInternalLoop)
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
			if (API.Login != null)
				await AddToken(request);

			if (string.IsNullOrEmpty(API.Config.UserAgent))
				throw new Exception("User-Agent must be set.");

			request.SetHeader("X-User-Agent", API.Config.UserAgent);

			return requestQueue.AddRequest(request);
		}

		/// <summary>
		/// Add access token to the request where it is sepcified to be
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		async Task AddToken(ApiRequest request)
		{
			if (!API.Login.TryGetToken((Scope)request.Scope, out IToken token))
			{
				if (API.Config.AutoRequestScope)
					token = await API.Login.AddToken((Scope)request.Scope);
				else
					throw new Exception($"No token with scope '{request.Scope}'");
			}

			switch (API.Login.Setup.TokenLocation)
			{
				case "header":
					request.SetHeader(API.Login.Setup.TokenName, await token.GetToken());
					break;
				case "query":
					request.AddQuery(API.Login.Setup.TokenName, await token.GetToken());
					break;
				default:
					throw new Exception("Unknwon access token location");
			}
		}

		/// <summary>
		/// Check if request has been cached.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		bool TryHitCache(ApiRequest request, int index, out ApiResponse response)
		{
			string requestUrl = request.GetRequestUrl(index);
			if (cache.TryGetValue(requestUrl, out response) && DateTime.Now < response.Expired)
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
			string requestUrl = request.GetRequestUrl(index);
			if (!cache.TryAdd(requestUrl, response))
				cache[requestUrl] = response;
		}

		/// <summary>
		/// Tries to retrive an eTag from a response. (Will not check if expired)
		/// </summary>
		/// <param name="request"></param>
		/// <param name="index"></param>
		/// <param name="eTag"></param>
		/// <returns></returns>
		bool TryGetETag(ApiRequest request, int index, out string eTag)
		{
			string requestUrl = request.GetRequestUrl(index);
			if (cache.TryGetValue(requestUrl, out ApiResponse response))
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
			{
				if (TryHitCache(request, i, out ApiResponse response))
				{
					esiResponses.Add(response);
					break;
				}

				TryGetETag(request, i, out string eTag);
				request.SetHeader("If-None-Match", eTag);

				response = await API.ResponseManager.GetResponse(request, i);
				if (response.Expired != default && response.CacheControl == "Public" || response.CacheControl == "Private")
					SaveToCache(request, i, response);

				esiResponses.Add(response);
			}

			return esiResponses;
		}
	}
}
