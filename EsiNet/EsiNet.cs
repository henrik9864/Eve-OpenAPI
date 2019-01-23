using EsiNet.Enums;
using EsiNet.Managers;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EsiNet
{
	public class EsiNet
	{
		internal static HttpClient Client { get; set; }

		public EveLogin Login { get; private set; }

		public OpenApiDocument Spec { get; private set; }

		public Datasource Datasource { get; set; }

		#region Internal

		internal RequestManager RequestManager { get; }

		internal CacheManager CacheManager { get; }

		internal ResponseManager ResponseManager { get; }

		internal EsiNetConfig Config { get; }

		#endregion

		public EsiNet(EveLogin login, OpenApiDocument spec, EsiNetConfig config)
		{
			Login = login;
			Spec = spec;
			Config = config;

			RequestManager = new RequestManager(Client, this);
			CacheManager = new CacheManager(Client, this);
			ResponseManager = new ResponseManager(Client, this);
		}

		public void ChangeLogin(EveLogin eveLogin)
		{
			if (eveLogin is null)
				throw new NullReferenceException("Login must be non null.");

			Login = eveLogin;
		}

		public EsiPath Path(string path)
		{
			if (Spec.Paths.TryGetValue(path, out OpenApiPathItem pathItem))
			{
				return new EsiPath(this, path, pathItem);
			}
			else
				throw new Exception($"The spec does not contain path '{path}'");
		}

		/// <summary>
		/// Create a new EsiNet with specification.
		/// </summary>
		/// <param name="version">Spec version.</param>
		/// <param name="datasource">Spe datasource.</param>
		/// <param name="login">EveLogin.</param>
		/// <param name="client">Optional HttpClient.</param>
		/// <param name="config">Optional Config.</param>
		/// <returns></returns>
		public static Task<EsiNet> Create(EsiVersion version, Datasource datasource, EveLogin login, HttpClient client = default, EsiNetConfig config = default)
		{
			string baseUrl = "https://esi.evetech.net/";
			string specUrl = $"{baseUrl}{version}/swagger.json?datasource={datasource}".ToLower();
			return Create(specUrl, login, client, config);
		}

		/// <summary>
		/// Create a new EsiNet with versioned specification.
		/// </summary>
		/// <param name="version">Spec version.</param>
		/// <param name="datasource">Spe datasource.</param>
		/// <param name="login">EveLogin.</param>
		/// <param name="client">Optional HttpClient.</param>
		/// <param name="config">Optional Config.</param>
		/// <returns></returns>
		public static Task<EsiNet> CreateVersioned(EsiVersion version, Datasource datasource, EveLogin login, HttpClient client = default, EsiNetConfig config = default)
		{
			string baseUrl = "https://esi.evetech.net/";
			string specUrl = $"{baseUrl}_{version}/swagger.json?datasource={datasource}".ToLower();
			return Create(specUrl, login, client, config);
		}

		static async Task<EsiNet> Create(string specUrl, EveLogin login, HttpClient client = default, EsiNetConfig config = default)
		{
			if (Client == default && client != default)
				Client = client;
			else
				Client = new HttpClient();

			if (config is null)
				config = new EsiNetConfig();

			OpenApiDocument document = await SpecFromUrl(specUrl);
			return new EsiNet(login, document, config);
		}

		/// <summary>
		/// Download the ESI swagger spec from the url.
		/// </summary>
		/// <param name="specUrl"></param>
		/// <returns></returns>
		static async Task<OpenApiDocument> SpecFromUrl(string specUrl)
		{
			Stream specStream = await Client.GetStreamAsync(specUrl);
			return ParseSpec(specStream);
		}

		/// <summary>
		/// Convert a json spec to an OpenApiDocument
		/// </summary>
		/// <param name="specStream"></param>
		/// <returns></returns>
		static OpenApiDocument ParseSpec(Stream specStream)
		{
			return new OpenApiStreamReader().Read(specStream, out OpenApiDiagnostic diagnostic);
		}
	}
}
