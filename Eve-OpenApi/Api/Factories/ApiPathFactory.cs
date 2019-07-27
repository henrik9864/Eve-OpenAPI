using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Api.Factories
{
	internal class ApiPathFactory : IFactory<IApiPath>
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
