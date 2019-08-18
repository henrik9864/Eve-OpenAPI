using EveOpenApi.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi.Api.Factories
{
	internal class ApiRequestFactory : IFactory<IApiRequest>
	{
		public ApiRequestFactory()
		{
		}

		public IApiRequest Create(params object[] context)
		{
			Uri uri = (Uri)context[0];
			string user = (string)context[1];
			string scope = (string)context[2];
			IDictionary<string,string> headers = (IDictionary<string, string>)context[3];
			HttpMethod method = (HttpMethod)context[4];
			return new ApiRequest(uri, user, scope, headers, method);
		}
	}
}
