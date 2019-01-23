using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi
{
	public class EsiResponse
	{
		public string ETag { get; }

		public string Response { get; }

		public DateTime Expired { get; }

		internal EsiResponse(string eTag, string response, DateTime expired)
		{
			ETag = eTag;
			Response = response;
			Expired = expired;
		}

		public EsiResponse<T> ToType<T>()
		{
			return new EsiResponse<T>(this);
		}
	}

	public class EsiResponse<T> : EsiResponse
	{
		public new T Response { get; }

		internal EsiResponse(string eTag, string response, DateTime expired) : base(eTag, response, expired)
		{
			Response = JsonConvert.DeserializeObject<T>(base.Response);
		}

		internal EsiResponse(EsiResponse response) : this(response.ETag, response.Response, response.Expired)
		{

		}
	}
}
