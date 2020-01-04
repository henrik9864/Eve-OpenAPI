using EveOpenApi.Interfaces;
using EveOpenApi.Managers.CacheControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Factories
{
	class CacheControlFactory : IFactory<ICacheControl>
	{
		public ICacheControl Create(params object[] context)
		{
			string cacheControlString = context[0].ToString();
			return new CacheControl(cacheControlString);
		}
	}
}
