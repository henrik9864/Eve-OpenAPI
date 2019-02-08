using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace EveOpenApi.Esi
{
	public class EsiError : EsiResponse
	{
		public string Error { get; }

		public HttpStatusCode StatusCode { get; }

		internal EsiError(string eTag, string response, DateTime expired, string cacheControl, HttpStatusCode statusCode)
			: base(eTag, response, expired, cacheControl)
		{
			dynamic jObj = JsonConvert.DeserializeObject(base.Response);

			Error = jObj.error;
			StatusCode = statusCode;
		}

		public override EsiResponse<T> ToType<T>()
		{
			throw new Exception("EsiError cannot be casted.");
		}
	}
}
