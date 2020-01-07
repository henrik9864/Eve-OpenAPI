using EveOpenApi.Interfaces;
using EveOpenApi.Managers.CacheControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace EveOpenApi.Api
{
	public class ApiResponse<T> : IApiResponse<T>
	{
		public T FirstPage { get; }

		public int MaxPages { get; }

		public string ETag { get; }

		public ICacheControl CacheControl { get; }

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

		internal ApiResponse(string eTag, IEnumerable<string> response, DateTime expired, ICacheControl cacheControl)
		{
			ETag = eTag;
			Expired = expired;
			CacheControl = cacheControl;

			this.response = response.Select(x => JsonSerializer.Deserialize<T>(x));
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
