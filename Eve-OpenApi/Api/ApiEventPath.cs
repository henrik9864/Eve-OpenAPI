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

		OpenApiPathItem pathItem;
		API parent;

		public ApiEventPath(API parent, string path, OpenApiPathItem pathItem)
		{
			Path = path;
			this.pathItem = pathItem;
			this.parent = parent;
		}

		public ApiEventMethod Get(params (string, object)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => new List<object> { a.Item2 });
			return new ApiEventMethod(Path, OperationType.Get, convertedParameters, pathItem, parent);
		}

		public ApiEventMethod GetBatch(params (string, List<object>)[] parameters)
		{
			var convertedParameters = parameters.ToDictionary(a => a.Item1, a => a.Item2);
			return new ApiEventMethod(Path, OperationType.Get, convertedParameters, pathItem, parent);
		}
	}
}
