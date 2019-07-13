using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiEventPath : IApiEventPath
	{
		public string Path { get; }

		public IList<string> DefaultUsers { get; private set; }

		OpenApiPathItem pathItem;

		IFactory<IApiEventMethod> eventMethodFactory;

		internal ApiEventPath(IFactory<IApiEventMethod> eventMethodFactory, string path, string deafultUser, OpenApiPathItem pathItem)
		{
			Path = path;
			this.pathItem = pathItem;
			this.eventMethodFactory = eventMethodFactory;
			DefaultUsers = new List<string>() { deafultUser };
		}

		public IApiEventPath SetUsers(params string[] users)
		{
			if (users.Any(x => string.IsNullOrEmpty(x)))
				throw new Exception("Users cannot be null or empty");

			DefaultUsers = new List<string>();
			((List<string>)DefaultUsers).AddRange(users);

			return this;
		}

		public IApiEventMethod Get(params (string, object)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<object> { a.Item2 });
			return eventMethodFactory.Create(pathItem, Path, OperationType.Get, convertedParameters, DefaultUsers);//new ApiEventMethod(pathItem, parent, managers, Path, OperationType.Get, convertedParameters, DefaultUsers);
		}

		public IApiEventMethod GetBatch(params (string, List<object>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2);
			return eventMethodFactory.Create(pathItem, Path, OperationType.Get, convertedParameters, DefaultUsers);//new ApiEventMethod(pathItem, parent, managers, Path, OperationType.Get, convertedParameters, DefaultUsers);
		}
	}
}
