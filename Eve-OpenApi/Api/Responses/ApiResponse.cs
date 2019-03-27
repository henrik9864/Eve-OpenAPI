using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiResponse
	{
		public string ETag { get; }

		public string Response { get; }

		public DateTime Expired { get; }

		internal string CacheControl { get; }

		internal ApiResponse(string eTag, string response, DateTime expired, string cacheControl)
		{
			ETag = eTag;
			Response = response;
			Expired = expired;
			CacheControl = cacheControl;
		}

		public virtual ApiResponse<T> ToType<T>()
		{
			return new ApiResponse<T>(ETag, Response, Expired, CacheControl);
		}
	}
}
