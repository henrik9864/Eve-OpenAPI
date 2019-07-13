using System.Collections.Generic;
using EveOpenApi.Api;
using EveOpenApi.Enums;
using Microsoft.OpenApi.Models;

namespace EveOpenApi.Managers
{
	internal interface IEventManager
	{
		/// <summary>
		/// Dictinary of all events currently active.
		/// </summary>
		Dictionary<(int, EventType), ApiUpdate> Events { get; }

		/// <summary>
		/// Prep a path for an event subscription.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="eventType"></param>
		/// <param name="path"></param>
		/// <param name="parameters"></param>
		/// <param name="users"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		IApiRequest GetRequest(OperationType type, EventType eventType, string path, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation);
	}
}