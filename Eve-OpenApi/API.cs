using EveOpenApi.Api;
using EveOpenApi.Authentication;
using EveOpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Net;

namespace EveOpenApi
{
	public class API : IAPI
	{
		public ILogin Login { get; private set; }

		public OpenApiDocument Spec { get; private set; }

		public string DefaultUser { get; private set; }

		internal IApiConfig Config { get; }

		IFactory<IApiPath> pathFacotry;
		IFactory<IApiEventPath> eventPathFacotry;

		internal API(ILogin login, IApiConfig config, IFactory<IApiPath> pathFacotry, OpenApiDocument spec, IFactory<IApiEventPath> eventPathFacotry)
		{
			Login = login;
			this.pathFacotry = pathFacotry;
			this.eventPathFacotry = eventPathFacotry;
			Spec = spec;
			Config = config;
			DefaultUser = Config.DefaultUser;
		}

		public void ChangeLogin(ILogin login)
		{
			if (login is null)
				throw new NullReferenceException("Cannot change login to null.");

			Login = login;
		}

		public IApiPath Path(string path)
		{
			if (Spec.Paths.TryGetValue(path, out OpenApiPathItem pathItem))
				return pathFacotry.Create(path, DefaultUser, pathItem);
			else
				throw new Exception($"The spec does not contain path '{path}'");
		}

		public IApiEventPath PathEvent(string path)
		{
			if (Spec.Paths.TryGetValue(path, out OpenApiPathItem pathItem))
				return eventPathFacotry.Create(path, DefaultUser, pathItem);
			else
				throw new Exception($"The spec does not contain path '{path}'");
		}
	}
}
