﻿using EveOpenApi.Api;
using EveOpenApi.Api.Factories;
using EveOpenApi.Authentication;
using EveOpenApi.Interfaces;
using EveOpenApi.Managers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.IO;
using System.Net;

namespace EveOpenApi
{
	public class ApiBuilder
	{
		public IApiConfig Config { get; private set; }

		public ILogin Login { get; private set; }

		private OpenApiDocument Spec { get; set; }

		public ApiBuilder()
		{
			Config = new ApiConfig();
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

		public ApiBuilder SpecFromFile(string path)
		{
			string spec = File.ReadAllText(path);
			Spec = new OpenApiStringReader().Read(spec, out OpenApiDiagnostic diagnostic);
			return this;
		}

		public IAPI Build()
		{
			return Create(Login, Config);
		}

		IAPI Create(ILogin login, IApiConfig config)
		{
			if (config is null)
				throw new Exception("Configuration cannot be null");

			IHttpHandler client = new HttpHandler();
			IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions()
			{
				SizeLimit = 128, // Each ApiRequest is one in size
			});

			if (!string.IsNullOrEmpty(config.SpecURL))
				Spec = SpecFromUrl(config.SpecURL);

			IFactory<IApiRequest> apiRequestFactory = new ApiRequestFactory();
			IFactory<ICacheControl> cacheControlFactory = new CacheControlFactory();

			ITokenManager tokenManager = new TokenManager(client, config, login);
			IResponseManager responseManager = new ResponseManager(client,config, login, cacheControlFactory);
			ICacheManager cacheManager = new CacheManager(client, config, login, memoryCache, tokenManager, responseManager);
			IRequestManager requestManager = new RequestManager(client, config, login, cacheManager, apiRequestFactory, Spec);
			IEventManager eventManager = new EventManager(client, config, login, cacheManager, requestManager);

			IFactory<IApiPath> pathFacotry = new ApiPathFactory(requestManager);
			IFactory<IApiEventMethod> eventMethodFactory = new ApiEventMethodFactory(eventManager);
			IFactory<IApiEventPath> eventPathFactory = new ApiEventPathFactory(eventMethodFactory);

			return new API(login, config, pathFacotry, Spec, eventPathFactory);
		}

		OpenApiDocument SpecFromUrl(string specUrl)
		{
			string specString;
			using (WebClient webClient = new WebClient())
			{
				specString = webClient.DownloadString(specUrl);
			}

			return new OpenApiStringReader().Read(specString, out OpenApiDiagnostic diagnostic);
		}
	}
}
