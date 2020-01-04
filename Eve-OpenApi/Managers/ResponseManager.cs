using EveOpenApi.Api;
using EveOpenApi.Authentication;
using EveOpenApi.Authentication.Managers;
using EveOpenApi.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Managers
{
	internal class ResponseManager : BaseManager, IResponseManager
	{
		int errorRemain = 100;
		DateTime errorReset;

		IFactory<ICacheControl> cacheControlFactory;

		public ResponseManager(IHttpHandler client, IApiConfig config, ILogin login, IFactory<ICacheControl> cacheControlFactory) : base(client, login, config)
		{
			this.cacheControlFactory = cacheControlFactory;
		}

		public async Task<IApiResponse<T>> GetResponse<T>(IApiRequest request)
		{
			IApiResponse esiResponse = await GetResponse(request);
			return esiResponse.ToType<T>();
		}

		public async Task<IApiResponse> GetResponse(IApiRequest request)
		{
			return await GetAllPages(request);
		}

		async Task<IApiResponse> GetAllPages(IApiRequest request)
		{
			// Get response from api and check rate limit
			var httpResponse = await GetHttpRequest(request);
			await CheckRateLimit(httpResponse);

			// Get page info from header and save response as first page
			int pages = GetPages(httpResponse);
			string[] responseArr = new string[pages];
			responseArr[0] = await httpResponse.Content.ReadAsStringAsync();

			// Get headers to use later
			string eTag = TryGetHeaderValue(httpResponse.Headers, "etag");
			string expireString = TryGetHeaderValue(httpResponse.Content.Headers, "Expires");
			string nowString = TryGetHeaderValue(httpResponse.Headers, "Date");
			string cacheControlString = TryGetHeaderValue(httpResponse.Content.Headers, "cache-control");

			ICacheControl cacheControl = cacheControlFactory.Create(cacheControlString);

			// Return an error if the request failed
			DateTime expired = ParseDateTime(expireString, nowString);
			if (!httpResponse.IsSuccessStatusCode)
				return new ApiError(eTag, responseArr, expired, cacheControl, httpResponse.StatusCode);

			for (int i = 1; i < pages; i++)
				responseArr[i] = await GetPage(request, i);

			return new ApiResponse(eTag, responseArr, expired, cacheControl);
		}

		async Task<HttpResponseMessage> GetHttpRequest(IApiRequest request)
		{
			HttpRequestMessage requestMessage = new HttpRequestMessage(request.HttpMethod, request.RequestUri);
			foreach (var item in request.Headers)
				requestMessage.Headers.TryAddWithoutValidation(item.Key, item.Value);

			return await Client.SendAsync(requestMessage);
		}

		async Task<string> GetPage(IApiRequest request, int page)
		{
			request.SetParameter("page", (page + 1).ToString());

			var httpResponse = await GetHttpRequest(request);
			await CheckRateLimit(httpResponse);

			// Throw an exception if server errors in the middle of fetching pages
			if (!httpResponse.IsSuccessStatusCode)
				throw new Exception($"Error fetching pages for '{request.RequestUri.AbsolutePath}' CODE {httpResponse.StatusCode}: {await httpResponse.Content.ReadAsStringAsync()}");

			return await httpResponse.Content.ReadAsStringAsync();
		}

		async Task CheckRateLimit(HttpResponseMessage response)
		{
			string errorRemainString = TryGetHeaderValue(response.Headers, Config.RateLimitRemainHeader);
			string errorResetString = TryGetHeaderValue(response.Headers, Config.RateLimitResetHeader);

			int.TryParse(errorRemainString, out errorRemain);
			int.TryParse(errorResetString, out int errorResetTime);
			errorReset = DateTime.Now + new TimeSpan(0, 0, errorResetTime);

			// Throttle requests if users send too many errors.
			if (errorRemain <= 0 && errorReset > DateTime.Now)
			{
				Console.WriteLine((errorReset - DateTime.Now).Seconds);
				if (Config.RateLimitThrotle)
					await Task.Delay(errorReset - DateTime.Now);
				else
					throw new Exception("Rate limit reached.");
			}
		}

		int GetPages(HttpResponseMessage response)
		{
			string pageString = TryGetHeaderValue(response.Headers, Config.PageHeader);

			if (string.IsNullOrEmpty(pageString))
				return 1;

			return int.Parse(pageString);
		}

		string TryGetHeaderValue(HttpHeaders header, string name)
		{
			if (header.TryGetValues(name, out IEnumerable<string> list))
				return list.FirstOrDefault();

			return "";
		}

		DateTime ParseDateTime(string dateTime, string nowString)
		{
			if (string.IsNullOrEmpty(dateTime) || string.IsNullOrEmpty(nowString))
				return default;

			DateTime parsedExpiery = DateTime.ParseExact(dateTime, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);
			DateTime parsedDate = DateTime.ParseExact(nowString, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);

			return DateTime.Now + (parsedExpiery - parsedDate);
		}
	}
}
