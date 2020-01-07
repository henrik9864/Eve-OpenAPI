using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections;
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
		IEventManager eventManager;

		internal ApiEventMethod(OpenApiPathItem pathItem, string path, OperationType operation, Dictionary<string, List<object>> parameters, List<string> users, IEventManager eventManager)
		{
			Path = path;
			Operation = operation;
			Parameters = parameters;
			Users = users;
			this.pathItem = pathItem;
			this.eventManager = eventManager;
		}

		/// <summary>
		/// Called whenever the data expire.
		/// </summary>
		public event ApiUpdate OnChange
		{
			add
			{
				IEnumerable<IApiRequest> requests = GetRequest(EventType.Change);
				foreach (IApiRequest request in requests)
					eventManager.Events[(request.GetHashCode(), EventType.Change)] = eventManager.Events[(request.GetHashCode(), EventType.Change)] + value;

				//for (int i = 0; i < request.Parameters.MaxLength; i++)
				//eventManager.Events[(request.GetHashCode(), EventType.Change)] = eventManager.Events[(request.GetHashCode(), EventType.Change)] + value;
			}
			remove
			{
				IEnumerable<IApiRequest> requests = GetRequest(EventType.Change);
				foreach (IApiRequest request in requests)
					eventManager.Events[(request.GetHashCode(), EventType.Change)] = eventManager.Events[(request.GetHashCode(), EventType.Change)] - value;

				//for (int i = 0; i < request.Parameters.MaxLength; i++)
				//eventManager.Events[(request.GetHashCode(), EventType.Change)] = eventManager.Events[(request.GetHashCode(), EventType.Change)] - value;
			}
		}

		/// <summary>
		/// Called whenever the data on an endpoint change.
		/// </summary>
		public event ApiUpdate OnExpire
		{
			add
			{
				IEnumerable<IApiRequest> requests = GetRequest(EventType.Update);
				foreach (IApiRequest request in requests)
					eventManager.Events[(request.GetHashCode(), EventType.Update)] = eventManager.Events[(request.GetHashCode(), EventType.Update)] + value;

				//for (int i = 0; i < request.Parameters.MaxLength; i++)
				//eventManager.Events[(request.GetHashCode(), EventType.Update)] = eventManager.Events[(request.GetHashCode(), EventType.Update)] + value;
			}
			remove
			{
				IEnumerable<IApiRequest> requests = GetRequest(EventType.Update);
				foreach (IApiRequest request in requests)
					eventManager.Events[(request.GetHashCode(), EventType.Update)] = eventManager.Events[(request.GetHashCode(), EventType.Update)] - value;

				//for (int i = 0; i < request.Parameters.MaxLength; i++)
				//eventManager.Events[(request.GetHashCode(), EventType.Update)] = eventManager.Events[(request.GetHashCode(), EventType.Update)] - value;
			}
		}

		IEnumerable<IApiRequest> GetRequest(EventType type)
		{
			return eventManager.GetRequest(Operation, type, Path, Parameters, Users, GetOperation(Operation));
		}

		OpenApiOperation GetOperation(OperationType type)
		{
			if (!pathItem.Operations.TryGetValue(type, out OpenApiOperation operation))
				throw new Exception($"This path does not have '{type}' operation specified.");

			return operation;
		}
	}
}
