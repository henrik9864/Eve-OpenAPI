using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiEventMethod : IApiEventMethod
	{
		public string Path { get; }

		public OperationType Operation { get; }

		public Dictionary<string, List<object>> Parameters { get; }

		public List<string> Users { get; }

		OpenApiPathItem pathItem;

		IManagerContainer managers;

		internal ApiEventMethod(OpenApiPathItem pathItem, IManagerContainer managerContainer, string path, OperationType operation, Dictionary<string, List<object>> parameters, List<string> users)
		{
			Path = path;
			Operation = operation;
			Parameters = parameters;
			Users = users;
			this.pathItem = pathItem;
			this.managers = managerContainer;
		}

		/// <summary>
		/// Called whenever the data expire.
		/// </summary>
		public event ApiUpdate OnChange
		{
			add
			{
				IEventManager eventManager = managers.EventManager;
				IApiRequest request = GetRequest(EventType.Change);

				for (int i = 0; i < request.Parameters.MaxLength; i++)
					eventManager.Events[(request.GetHashCode(i), EventType.Change)] = eventManager.Events[(request.GetHashCode(i), EventType.Change)] + value;
			}
			remove
			{
				IEventManager eventManager = managers.EventManager;
				IApiRequest request = GetRequest(EventType.Change);

				for (int i = 0; i < request.Parameters.MaxLength; i++)
					eventManager.Events[(request.GetHashCode(i), EventType.Change)] = eventManager.Events[(request.GetHashCode(i), EventType.Change)] - value;
			}
		}

		/// <summary>
		/// Called whenever the data on an endpoint change.
		/// </summary>
		public event ApiUpdate OnUpdate
		{
			add
			{
				IEventManager eventManager = managers.EventManager;
				IApiRequest request = GetRequest(EventType.Update);

				for (int i = 0; i < request.Parameters.MaxLength; i++)
					eventManager.Events[(request.GetHashCode(i), EventType.Update)] = eventManager.Events[(request.GetHashCode(i), EventType.Update)] + value;
			}
			remove
			{
				IEventManager eventManager = managers.EventManager;
				IApiRequest request = GetRequest(EventType.Update);

				for (int i = 0; i < request.Parameters.MaxLength; i++)
					eventManager.Events[(request.GetHashCode(i), EventType.Update)] = eventManager.Events[(request.GetHashCode(i), EventType.Update)] - value;
			}
		}

		IApiRequest GetRequest(EventType type)
		{
			return managers.EventManager.GetRequest(Operation, type, Path, Parameters, Users, GetOperation(Operation));
		}

		OpenApiOperation GetOperation(OperationType type)
		{
			if (!pathItem.Operations.TryGetValue(type, out OpenApiOperation operation))
				throw new Exception($"This path does not have '{type}' operation specified.");

			return operation;
		}
	}
}
