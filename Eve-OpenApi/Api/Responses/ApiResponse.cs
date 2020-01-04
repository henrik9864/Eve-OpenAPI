using EveOpenApi.Interfaces;
using EveOpenApi.Managers.CacheControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiResponse : IApiResponse
	{
		public string ETag { get; }

		public string FirstPage { get; }

		public int MaxPages { get; }

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

		public ICacheControl CacheControl { get; }

		IEnumerable<string> response;

		internal ApiResponse(string eTag, IEnumerable<string> response, DateTime expired, ICacheControl cacheControl)
		{
			ETag = eTag;
			Expired = expired;
			CacheControl = cacheControl;
			FirstPage = response.FirstOrDefault();
			this.response = response;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash *= 23 + ETag.GetHashCode();
			hash *= 23 + FirstPage.GetHashCode();

			return hash;
		}

		public virtual IApiResponse<T> ToType<T>()
		{
			return new ApiResponse<T>(ETag, response, Expired, CacheControl);
		}

		public IEnumerator<string> GetEnumerator()
		{
			return response.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return response.GetEnumerator();
		}
	}
}
