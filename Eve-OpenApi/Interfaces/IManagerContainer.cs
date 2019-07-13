using EveOpenApi.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Interfaces
{
	/// <summary>
	/// This class contains all managers used in the API
	/// </summary>
	internal interface IManagerContainer
	{
		IRequestManager RequestManager { get; }

		ICacheManager CacheManager { get; }

		IResponseManager ResponseManager { get; }

		IEventManager EventManager { get; }

		ITokenManager TokenManager { get; }
	}
}
