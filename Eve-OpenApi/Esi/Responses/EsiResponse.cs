using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Esi
{
	public class EsiResponse
	{
		public string ETag { get; }

		public string Response { get; }

		public DateTime Expired { get; }

		internal string CacheControl { get; }

		internal EsiResponse(string eTag, string response, DateTime expired, string cacheControl)
		{
			ETag = eTag;
			Response = response;
			Expired = expired;
			CacheControl = cacheControl;
		}

		public virtual EsiResponse<T> ToType<T>()
		{
			return new EsiResponse<T>(ETag, Response, Expired, CacheControl);
		}
	}
}
