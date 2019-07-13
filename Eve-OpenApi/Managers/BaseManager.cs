using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi.Managers
{
	internal abstract class BaseManager : IBaseManager
	{
		protected HttpClient Client { get; }

		protected ILogin Login { get; }

		protected IApiConfig Config { get; }

		public BaseManager(HttpClient client, ILogin login, IApiConfig config)
		{
			Client = client;
			Login = login;
			Config = config;
		}
	}
}
