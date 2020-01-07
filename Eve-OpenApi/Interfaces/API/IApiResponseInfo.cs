using EveOpenApi.Interfaces;
using EveOpenApi.Managers.CacheControl;
using System;

namespace EveOpenApi.Api
{
	public interface IApiResponseInfo
	{
		/// <summary>
		/// Return E-Tag from the API
		/// </summary>
		string ETag { get; }

		/// <summary>
		/// When this response is stale
		/// </summary>
		DateTime Expired { get; internal set; }

		/// <summary>
		/// Information about how this response should be cached
		/// </summary>
		ICacheControl CacheControl { get; }
	}
}