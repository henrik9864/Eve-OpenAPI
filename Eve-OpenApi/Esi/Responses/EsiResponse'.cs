using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Esi
{
	public class EsiResponse<T> : EsiResponse
	{
		public new T Response { get; }

		internal EsiResponse(string eTag, string response, DateTime expired, string cacheControl)
			: base(eTag, response, expired, cacheControl)
		{
			Response = JsonConvert.DeserializeObject<T>(base.Response);
		}

		public override EsiResponse<T> ToType<T>()
		{
			throw new Exception("EsiResponse alreaedy casted.");
		}
	}
}
