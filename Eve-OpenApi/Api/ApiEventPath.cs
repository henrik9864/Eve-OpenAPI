using EveOpenApi.Managers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveOpenApi.Api
{
    public class ApiEventPath
	{
		public string Path { get; }

		public List<string> DefaultUsers { get; private set; }

		OpenApiPathItem pathItem;
		API parent;

		public ApiEventPath(API parent, string path, string deafultUser, OpenApiPathItem pathItem)
		{
			Path = path;
			this.pathItem = pathItem;
			this.parent = parent;
			DefaultUsers = new List<string>() { deafultUser };
		}

		public ApiEventPath SetUsers(params string[] users)
		{
			if (users.Any(x => string.IsNullOrEmpty(x)))
				throw new Exception("Users cannot be null or empty");

			DefaultUsers = new List<string>();
			DefaultUsers.AddRange(users);

			return this;
		}

		public ApiEventMethod Get(params (string, object)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<object> { a.Item2 });
			return new ApiEventMethod(Path, OperationType.Get, convertedParameters, DefaultUsers, pathItem, parent);
		}

		public ApiEventMethod GetBatch(params (string, List<object>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2);
			return new ApiEventMethod(Path, OperationType.Get, convertedParameters, DefaultUsers, pathItem, parent);
		}
	}
}
