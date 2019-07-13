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

		protected IAPI API { get; }

		protected IManagerContainer Managers { get; }

		protected IApiConfig Config { get; }

		public BaseManager(HttpClient client, IAPI api, IManagerContainer managerContainer, IApiConfig config)
		{
			Client = client;
			API = api;
			Managers = managerContainer;
			Config = config;
		}
	}
}
