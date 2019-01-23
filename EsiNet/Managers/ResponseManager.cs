using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EsiNet.Managers
{
	internal class ResponseManager : BaseManager
	{
		public event Action<int, EsiResponse> RequestFinished;

		int errorRemain = 100;
		DateTime errorReset;

		public ResponseManager(HttpClient client, EsiNet esiNet) : base(client, esiNet)
		{
		}

		public async Task<EsiResponse> GetResponse(EsiRequest request, int index)
		{
			HttpResponseMessage response = await GetHttpResponse(request, index);
			EsiResponse esiResponse = await GetEsiResponse(response);
			CheckLimit(response);
			return esiResponse;
		}

		public async Task<EsiResponse<T>> GetResponse<T>(EsiRequest request, int index)
		{
			EsiResponse esiResponse = await GetResponse(request, index);
			return new EsiResponse<T>(esiResponse);
		}

		public async Task<HttpResponseMessage> GetHttpResponse(EsiRequest request, int index)
		{
			Uri requestUri = new Uri(request.GetRequestUrl(index));
			HttpRequestMessage requestMessage = new HttpRequestMessage(request.Method, requestUri);

			foreach (var item in request.Parameters.Headers)
				requestMessage.Headers.Add(item.Key, item.Value);

			// Throttle requests if users send too many.
			if (errorRemain == 0 && errorReset > DateTime.Now)
				await Task.Delay(errorReset - DateTime.Now);

			return await Client.SendAsync(requestMessage);
		}

		async Task<EsiResponse> GetEsiResponse(HttpResponseMessage response)
		{
			string eTag = TryGetHeaderValue(response.Headers, "etag");
			string expires = TryGetHeaderValue(response.Content.Headers, "expires");
			string json = await response.Content.ReadAsStringAsync();

			DateTime parsedExpiery = DateTime.ParseExact(expires, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);
			return new EsiResponse(eTag, json, parsedExpiery);
		}

		void CheckLimit(HttpResponseMessage response)
		{
			string errorRemainString = TryGetHeaderValue(response.Headers, "x-esi-error-limit-remain");
			string errorResetString = TryGetHeaderValue(response.Headers, "x-esi-error-limit-reset");

			int.TryParse(errorRemainString, out errorRemain);
			int.TryParse(errorResetString, out int errorResetTime);
			errorReset = DateTime.Now + new TimeSpan(0, 0, errorResetTime);
		}

		string TryGetHeaderValue(HttpHeaders header, string name)
		{
			if (header.TryGetValues(name, out IEnumerable<string> list))
				return list.FirstOrDefault();

			return "";
		}
	}
}
