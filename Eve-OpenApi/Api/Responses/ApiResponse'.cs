using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiResponse<T> : ApiResponse
	{
		public new T Response { get; }

		internal ApiResponse(string eTag, string response, DateTime expired, string cacheControl)
			: base(eTag, response, expired, cacheControl)
		{
			Response = JsonConvert.DeserializeObject<T>(base.Response);
		}

		public override ApiResponse<T> ToType<T>()
		{
			throw new Exception("EsiResponse alreaedy casted.");
		}
	}
}
