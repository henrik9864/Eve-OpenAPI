using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi.Managers
{
	internal class BaseManager
	{
		protected HttpClient Client { get; }

		protected API API { get; }

		public BaseManager(HttpClient client, API api)
		{
			Client = client;
			API = api;
		}
	}
}
