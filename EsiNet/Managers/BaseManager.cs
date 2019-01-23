using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EsiNet.Managers
{
	internal class BaseManager
	{
		protected HttpClient Client { get; }

		protected EsiNet EsiNet { get; }

		public BaseManager(HttpClient client, EsiNet esiNet)
		{
			Client = client;
			EsiNet = esiNet;
		}
	}
}
