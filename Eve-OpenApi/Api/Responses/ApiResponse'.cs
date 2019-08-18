using EveOpenApi.Managers.CacheControl;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiResponse<T> : IApiResponse<T>
	{
		public T FirstPage { get; }

		public int MaxPages { get; }

		public string ETag { get; }

		public CacheControl CacheControl { get; }

		public DateTime Expired { get; private set; }

		DateTime IApiResponseInfo.Expired
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

		IEnumerable<T> response;

		internal ApiResponse(string eTag, IEnumerable<string> response, DateTime expired, string cacheControl)
		{
			ETag = eTag;
			Expired = expired;
			CacheControl = new CacheControl(cacheControl);

			this.response = response.Select(x => JsonConvert.DeserializeObject<T>(x));
			FirstPage = this.response.FirstOrDefault();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return response.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return response.GetEnumerator();
		}
	}
}
