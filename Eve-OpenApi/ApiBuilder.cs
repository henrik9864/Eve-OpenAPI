using EveOpenApi.Api;
using EveOpenApi.Api.Factories;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;

namespace EveOpenApi
{
	public class ApiBuilder
	{
		public IApiConfig Config { get; private set; }

		public ILogin Login { get; private set; }

		IManagerContainer managers;

		IFactory<IApiPath> pathFacotry;
		IFactory<IApiEventMethod> eventMethodFactory;
		IFactory<IApiEventPath> eventPathFactory;

		public ApiBuilder()
		{
			pathFacotry = new ApiPathFactory(managers);
			eventMethodFactory = new ApiEventMethodFactory(managers);
			eventPathFactory = new ApiEventPathFactory(managers, eventMethodFactory);
		}

		public ApiBuilder(IApiConfig config) : this()
		{
			Config = config;
		}

		public ApiBuilder(ILogin login) : this()
		{
			Login = login;
		}

		public ApiBuilder(IApiConfig config, ILogin login) : this()
		{
			Config = config;
			Login = login;
		}

		public ApiBuilder WithConfig(IApiConfig config)
		{
			Config = config;
			return this;
		}

		public ApiBuilder WithLogin(ILogin login)
		{
			Login = login;
			return this;
		}

		public IAPI Build()
		{
			return Create(Login, Config);
		}

		IAPI Create(ILogin login, IApiConfig config)
		{
			return new API(login, config, pathFacotry, eventPathFactory);
		}
	}
}
