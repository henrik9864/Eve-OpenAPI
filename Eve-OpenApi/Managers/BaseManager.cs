using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi.Managers
{
	internal class BaseManager
	{
		protected HttpClient Client { get; }

		protected API EsiNet { get; }

		public BaseManager(HttpClient client, API esiNet)
		{
			Client = client;
			EsiNet = esiNet;
		}
	}
}
