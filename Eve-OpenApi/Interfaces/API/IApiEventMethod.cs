using System.Collections.Generic;
using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;

namespace EveOpenApi.Api
{
	public interface IApiEventMethod
	{
		OperationType Operation { get; }

		Dictionary<string, List<object>> Parameters { get; }

		string Path { get; }

		List<string> Users { get; }

		event ApiUpdate OnChange;

		event ApiUpdate OnUpdate;
	}
}