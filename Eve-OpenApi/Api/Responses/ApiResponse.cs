using EveOpenApi.Managers.CacheControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiResponse : IApiResponse
	{
		public string ETag { get; }

		public string Response { get; }

		public DateTime Expired { get; private set; }

		DateTime IApiResponse.Expired
		{
			get
			{
				return Expired;
			}
			set
			{
				Expired = value;
			}
		}

		public CacheControl CacheControl { get; }

		string cacheControlString;

		internal ApiResponse(string eTag, string response, DateTime expired, string cacheControl)
		{
			ETag = eTag;
			Response = response;
			Expired = expired;
			CacheControl = new CacheControl(cacheControl);
			cacheControlString = cacheControl;
		}

		public void UpdateExpiery(IApiResponse newResponse)
		{
			Expired = newResponse.Expired;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash *= 23 + ETag.GetHashCode();
			hash *= 23 + Response.GetHashCode();
			//hash *= 23 + Expired.GetHashCode();

			return hash;
		}

		public virtual IApiResponse<T> ToType<T>()
		{
			return new ApiResponse<T>(ETag, Response, Expired, cacheControlString);
		}
	}
}
