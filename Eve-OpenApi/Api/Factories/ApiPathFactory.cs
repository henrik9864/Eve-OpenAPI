using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Factories
{
	class ApiPathFactory : IFactory<IApiPath>
	{
		IRequestManager requestManager;

		public ApiPathFactory(IRequestManager requestManager)
		{
			this.requestManager = requestManager;
		}

		public IApiPath Create(params object[] context)
		{
			string path = (string)context[0];
			string defaultUser = (string)context[1];
			OpenApiPathItem pathItem = (OpenApiPathItem)context[2];

			return new ApiPath(path, defaultUser, pathItem, requestManager);
		}
	}
}
