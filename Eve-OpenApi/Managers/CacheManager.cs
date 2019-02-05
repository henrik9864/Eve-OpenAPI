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
		Dictionary<string, EsiResponse> cache = new Dictionary<string, EsiResponse>();
		RequestQueueAsync<EsiRequest, List<EsiResponse>> requestQueue;

		public CacheManager(HttpClient client, ESI esiNet) : base(client, esiNet)
		{
			requestQueue = new RequestQueueAsync<EsiRequest, List<EsiResponse>>(ProcessResponse);
		}

		/// <summary>
		/// Get response from ESI.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task<List<EsiResponse>> GetResponse(EsiRequest request)
		{
			int id = await AddToRequestQueue(request);
			return await requestQueue.AwaitResponse(id);
		}

		public async Task<List<EsiResponse<T>>> GetResponse<T>(EsiRequest request)
		{
			int id = await AddToRequestQueue(request);
			List<EsiResponse> responses = await requestQueue.AwaitResponse(id);

			List<EsiResponse<T>> returnResponses = new List<EsiResponse<T>>();
			for (int i = 0; i < responses.Count; i++)
				returnResponses.Add(new EsiResponse<T>(responses[i]));

			return returnResponses;
		}

		async Task<int> AddToRequestQueue(EsiRequest request)
		{
			if (!string.IsNullOrEmpty(request.Scope))
			{
				if (!EsiNet.Login.TryGetToken(request.User, request.Scope, out EveToken token))
				{
					if (EsiNet.Config.AutoRequestScope)
						token = await EsiNet.Login.AddToken(request.Scope);
					else
						throw new Exception($"No token with scope '{request.Scope}'");
				}

				request.AddQuery("token", await token.GetToken());
			}

			return requestQueue.AddRequest(request);
		}

		/// <summary>
		/// Check if request has been cached.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		public bool TryHitCache(EsiRequest request, int index, out EsiResponse response)
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
		void SaveToCache(EsiRequest request, int index, EsiResponse response)
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
		bool TryGetETag(EsiRequest request, int index, out string eTag)
		{
			string requestUrl = request.GetRequestUrl(index);
			if (cache.TryGetValue(requestUrl, out EsiResponse response))
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
		async Task<List<EsiResponse>> ProcessResponse(EsiRequest request)
		{
			List<EsiResponse> esiResponses = new List<EsiResponse>();

			for (int i = 0; i < request.Parameters.MaxLength; i++)
			{
				if (TryHitCache(request, i, out EsiResponse response))
				{
					esiResponses.Add(response);
					break;
				}

				TryGetETag(request, i, out string eTag);
				request.SetHeader("If-None-Match", eTag);

				response = await EsiNet.ResponseManager.GetResponse(request, i);
				if (response.Expired != default)
					SaveToCache(request, i, response);

				esiResponses.Add(response);
			}

			return esiResponses;
		}
	}
}
