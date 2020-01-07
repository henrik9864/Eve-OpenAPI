using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi
{
	public interface IHttpHandler
	{
		Task<HttpResponseMessage> DeleteAsync(string requestUri);
		Task<HttpResponseMessage> GetAsync(string requestUri);
		Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content);
		Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
		Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);
		Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
	}
}