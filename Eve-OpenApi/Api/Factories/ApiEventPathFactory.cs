using EveOpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Api.Factories
{
	class ApiEventPathFactory : IFactory<IApiEventPath>
	{
		IFactory<IApiEventMethod> eventMethodFactory;

		public ApiEventPathFactory(IFactory<IApiEventMethod> eventMethodFactory)
		{
			this.eventMethodFactory = eventMethodFactory;
		}

		public IApiEventPath Create(params object[] context)
		{
			string path = (string)context[0];
			string defaultUser = (string)context[1];
			OpenApiPathItem pathItem = (OpenApiPathItem)context[2];

			return new ApiEventPath(eventMethodFactory, path, defaultUser, pathItem);
		}
	}
}
