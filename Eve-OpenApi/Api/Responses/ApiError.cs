using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiError : ApiResponse
	{
		public string Error { get; }

		public HttpStatusCode StatusCode { get; }

		internal ApiError(string eTag, IEnumerable<string> response, DateTime expired, string cacheControl, HttpStatusCode statusCode)
			: base(eTag, response, expired, cacheControl)
		{
			if (response.First().Length > 0 && response.First()[0] == '{')
			{
				dynamic jObj = JsonConvert.DeserializeObject(base.FirstPage);
				Error = jObj.error;
			}
			else
			{
				Error = response.First();
			}

			StatusCode = statusCode;
		}

		public override IApiResponse<T> ToType<T>()
		{
			throw new Exception($"ApiError cannot be casted. Error: {Error}");
		}
	}
}
