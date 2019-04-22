using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiError : ApiResponse
	{
		public string Error { get; }

		public HttpStatusCode StatusCode { get; }

		internal ApiError(string eTag, string response, DateTime expired, string cacheControl, HttpStatusCode statusCode)
			: base(eTag, response, expired, cacheControl)
		{
			if (response.Length > 0 && response[0] == '{')
			{
				dynamic jObj = JsonConvert.DeserializeObject(base.Response);
				Error = jObj.error;
			}
			else
			{
				Error = response;
			}

			StatusCode = statusCode;
		}

		public override ApiResponse<T> ToType<T>()
		{
			throw new Exception($"ApiError cannot be casted. Error: {Error}");
		}
	}
}
