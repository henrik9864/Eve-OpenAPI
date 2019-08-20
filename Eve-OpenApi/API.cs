using EveOpenApi.Api;
using EveOpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Net;
using EveOpenApi.Authentication;

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
				return pathFacotry.Create(path, DefaultUser, pathItem);//new ApiPath(this, path, DefaultUser, pathItem);
			else
				throw new Exception($"The spec does not contain path '{path}'");
		}

		public IApiEventPath PathEvent(string path)
		{
			if (Spec.Paths.TryGetValue(path, out OpenApiPathItem pathItem))
				return eventPathFacotry.Create(path, DefaultUser, pathItem);//new ApiEventPath(this, path, DefaultUser, pathItem);
			else
				throw new Exception($"The spec does not contain path '{path}'");
		}

		#region Static methods

		// Deprecated
		/*

		/// <summary>
		/// Create a new EsiNet with specification.
		/// </summary>
		/// <param name="version">Spec version.</param>
		/// <param name="datasource">Spe datasource.</param>
		/// <param name="login">EveLogin.</param>
		/// <param name="client">Optional HttpClient.</param>
		/// <param name="config">Optional Config.</param>
		/// <returns></returns>
		public static API CreateEsi(EsiVersion version, Datasource datasource, ILogin login, HttpClient client = default, ApiConfig config = default)
		{
			string baseUrl = "https://esi.evetech.net/";
			string specUrl = $"{baseUrl}{version}/swagger.json?datasource={datasource}".ToLower();
			return Create(specUrl, login, client, config);
		}

		/// <summary>
		/// Create a new API interface for ESI with versioned specification.
		/// </summary>
		/// <param name="version">Spec version.</param>
		/// <param name="datasource">Spe datasource.</param>
		/// <param name="login">EveLogin.</param>
		/// <param name="client">Optional HttpClient.</param>
		/// <param name="config">Optional Config.</param>
		/// <returns></returns>
		public static API CreateEsiVersioned(EsiVersion version, Datasource datasource, ILogin login, HttpClient client = default, ApiConfig config = default)
		{
			string baseUrl = "https://esi.evetech.net/";
			string specUrl = $"{baseUrl}_{version}/swagger.json?datasource={datasource}".ToLower();
			return Create(specUrl, login, client, config);
		}

		/// <summary>
		/// Create an API interface for any OpenAPI with supported login.
		/// </summary>
		/// <param name="specUrl"></param>
		/// <param name="login"></param>
		/// <param name="client"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static API Create(string specUrl, ILogin login = default, HttpClient client = default, ApiConfig config = default)
		{
			if (Client == default && client != default)
				Client = client;
			else
				Client = new HttpClient();

			if (config is null)
				config = new ApiConfig();

			OpenApiDocument document = SpecFromUrl(specUrl);
			return new API(login, document, config);
		}

		/// <summary>
		/// Download the ESI swagger spec from the url. This is not async so it can be used in constructores
		/// </summary>
		/// <param name="specUrl"></param>
		/// <returns></returns>
		static OpenApiDocument SpecFromUrl(string specUrl)
		{
			string specString;
			using (WebClient webClient = new WebClient())
			{
				specString = webClient.DownloadString(specUrl);
			}

			return new OpenApiStringReader().Read(specString, out OpenApiDiagnostic diagnostic);
		}

		*/

		#endregion
	}
}
