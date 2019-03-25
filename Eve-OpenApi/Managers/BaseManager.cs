using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi.Managers
{
	internal class BaseManager
	{
		protected HttpClient Client { get; }

		protected OpenApiInterface EsiNet { get; }

		public BaseManager(HttpClient client, OpenApiInterface esiNet)
		{
			Client = client;
			EsiNet = esiNet;
		}
	}
}
