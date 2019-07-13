using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Factories
{
	class ApiEventMethodFactory : IFactory<IApiEventMethod>
	{
		IEventManager eventManager;

		public ApiEventMethodFactory(IEventManager eventManager)
		{
			this.eventManager = eventManager;
		}

		public IApiEventMethod Create(params object[] context)
		{
			OpenApiPathItem pathItem = (OpenApiPathItem)context[0];
			string path = (string)context[1];
			OperationType operation = (OperationType)context[2];
			Dictionary<string, List<object>> parameters = (Dictionary<string, List<object>>)context[3];
			List<string> users = (List<string>)context[4];

			return new ApiEventMethod(pathItem, path, operation, parameters, users, eventManager);
		}
	}
}
