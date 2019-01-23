using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi.Managers
{
	internal class BaseManager
	{
		protected HttpClient Client { get; }

		protected ESI EsiNet { get; }

		public BaseManager(HttpClient client, ESI esiNet)
		{
			Client = client;
			EsiNet = esiNet;
		}
	}
}
