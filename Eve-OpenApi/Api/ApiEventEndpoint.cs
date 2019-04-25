using EveOpenApi.Enums;
using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api
{
    public class ApiEventMethod
	{
		public string Path { get; }

		public OperationType Operation { get; }

		public Dictionary<string, List<object>> Parameters { get; }

		public List<string> Users { get; }

		OpenApiPathItem pathItem;
		API parent;

		public ApiEventMethod(string path, OperationType operation, Dictionary<string, List<object>> parameters, List<string> users, OpenApiPathItem pathItem, API parent)
		{
			Path = path;
			Operation = operation;
			Parameters = parameters;
			Users = users;
			this.pathItem = pathItem;
			this.parent = parent;
		}

		/// <summary>
		/// Called whenever the data expire.
		/// </summary>
		public event ApiUpdate OnChange
		{
			add
			{
				ApiRequest request = GetRequest(EventType.Change);
				for (int i = 0; i < request.Parameters.MaxLength; i++)
				{
					parent.EventManager.Events[(request.GetHashCode(i), EventType.Change)] = parent.EventManager.Events[(request.GetHashCode(i), EventType.Change)] + value;
				}
			}
			remove
			{
				ApiRequest request = GetRequest(EventType.Change);
				for (int i = 0; i < request.Parameters.MaxLength; i++)
				{
					parent.EventManager.Events[(request.GetHashCode(i), EventType.Change)] = parent.EventManager.Events[(request.GetHashCode(i), EventType.Change)] - value;
				}
			}
		}

		/// <summary>
		/// Called whenever the data on an endpoint change.
		/// </summary>
		public event ApiUpdate OnUpdate
		{
			add
			{
				ApiRequest request = GetRequest(EventType.Update);
				for (int i = 0; i < request.Parameters.MaxLength; i++)
				{
					parent.EventManager.Events[(request.GetHashCode(i), EventType.Update)] = parent.EventManager.Events[(request.GetHashCode(i), EventType.Update)] + value;
				}
			}
			remove
			{
				ApiRequest request = GetRequest(EventType.Update);
				for (int i = 0; i < request.Parameters.MaxLength; i++)
				{
					parent.EventManager.Events[(request.GetHashCode(i), EventType.Update)] = parent.EventManager.Events[(request.GetHashCode(i), EventType.Update)] - value;
				}
			}
		}

		ApiRequest GetRequest(EventType type)
		{
			return parent.EventManager.GetRequest(Operation, type, Path, Parameters, Users, GetOperation(Operation));
		}

		OpenApiOperation GetOperation(OperationType type)
		{
			if (!pathItem.Operations.TryGetValue(type, out OpenApiOperation operation))
				throw new Exception($"This path does not have '{type}' operation specified.");

			return operation;
		}
	}
}
