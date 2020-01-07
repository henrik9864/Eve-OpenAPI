using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi
{
	public class HttpHandler : IHttpHandler
	{
		private static readonly HttpClient client;

		static HttpHandler()
		{
			client = new HttpClient();
		}

		public Task<HttpResponseMessage> DeleteAsync(string requestUri)
			=> client.DeleteAsync(requestUri);

		public Task<HttpResponseMessage> GetAsync(string requestUri)
			=> client.GetAsync(requestUri);

		public Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
			=> client.PatchAsync(requestUri, content);

		public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
			=> client.PostAsync(requestUri, content);

		public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
			=> client.PutAsync(requestUri, content);

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
			=> client.SendAsync(request);
	}
}
