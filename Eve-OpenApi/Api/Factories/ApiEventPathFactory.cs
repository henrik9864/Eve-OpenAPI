using EveOpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Factories
{
	class ApiEventPathFactory : IFactory<IApiEventPath>
	{
		IManagerContainer managers;
		IFactory<IApiEventMethod> eventMethodFactory;

		public ApiEventPathFactory(IManagerContainer managers, IFactory<IApiEventMethod> eventMethodFactory)
		{
			this.managers = managers;
			this.eventMethodFactory = eventMethodFactory;
		}

		public IApiEventPath Create(params object[] context)
		{
			string path = (string)context[0];
			string defaultUser = (string)context[1];
			OpenApiPathItem pathItem = (OpenApiPathItem)context[2];

			return new ApiEventPath(managers, eventMethodFactory, path, defaultUser, pathItem);
		}
	}
}
