using EveOpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Factories
{
	class ApiPathFactory : IFactory<IApiPath>
	{
		IManagerContainer managers;

		public ApiPathFactory(IManagerContainer managers)
		{
			this.managers = managers;
		}

		public IApiPath Create(params object[] context)
		{
			string path = (string)context[0];
			string defaultUser = (string)context[1];
			OpenApiPathItem pathItem = (OpenApiPathItem)context[2];

			return new ApiPath(managers, path, defaultUser, pathItem);
		}
	}
}
