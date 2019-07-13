using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi
{
	/// <summary>
	/// Container holding all managers used by this API
	/// </summary>
	internal class ManagerContainer : IManagerContainer
	{
		public IRequestManager RequestManager { get; }

		public ICacheManager CacheManager { get; }

		public IResponseManager ResponseManager { get; }

		public IEventManager EventManager { get; }

		public ITokenManager TokenManager { get; }

		public ManagerContainer(IRequestManager requestManager, ICacheManager cacheManager, IResponseManager responseManager, IEventManager eventManager, ITokenManager tokenManager)
		{
			RequestManager = requestManager;
			CacheManager = cacheManager;
			ResponseManager = responseManager;
			EventManager = eventManager;
			TokenManager = tokenManager;
		}

		/*public ManagerContainer(HttpClient client, IAPI api)
		{
			RequestManager = new RequestManager(client, api);
			CacheManager = new CacheManager(client, api);
			ResponseManager = new ResponseManager(client, api);
			EventManager = new EventManager(client, api);
			TokenManager = new TokenManager(client, api);
		}*/
	}
}
