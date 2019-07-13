using EveOpenApi;
using EveOpenApi.Api;
using EveOpenApi.Api.Configs;
using EveOpenApi.DependencyInjection;
using EveOpenApi.Eve;
using EveOpenApi.Interfaces;
using EveOpenApi.Seat;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AspNet
	{
		public static IServiceCollection AddEsi(this IServiceCollection services, IConfiguration config)
		{
			services.AddEve(config);

			services.AddScoped<IApiConfig, EsiConfig>(a =>
			{
				EsiConfig esiConfig = new EsiConfig();
				config.Bind("EsiConfig", esiConfig);

				return esiConfig;

			});
			services.AddScoped<API>();

			return services;
		}

		public static IServiceCollection AddEve(this IServiceCollection services, IConfiguration config)
		{
			services.AddSingleton<HttpClient>();
			services.Configure<EveWebConfig>(config.GetSection("EveConfig"));
			services.AddSingleton<ILogin, EveWebLoginExtension>();

			return services;
		}

		public static IServiceCollection AddSeat(this IServiceCollection services, IConfiguration config)
		{
			services.AddSingleton<HttpClient>();
			services.Configure<SeatConfig>(config.GetSection("SeatConfig"));
			services.AddSingleton<ILogin, SeatLogin>();

			return services;
		}

		public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config)
		{
			services.AddSingleton<HttpClient>();
			services.AddScoped<IApiConfig, ApiConfig>(a =>
			{
				ApiConfig apiConfig = new ApiConfig();
				config.Bind("ApiConfig", apiConfig);

				return apiConfig;

			});
			services.AddScoped<API>();

			return services;
		}
	}
}
