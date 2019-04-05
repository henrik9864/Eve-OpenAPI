using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Configs
{
	public class ApiEsiConfig : ApiConfig
	{
		public string BaseUrl { get; set; } = "https://esi.evetech.net/";

		public EsiVersion Version { get; set; } = EsiVersion.Latest;

		public Datasource Datasource { get; set; } = Datasource.Tranquility;

		public override string SpecURL
		{
			get
			{
				return $"{BaseUrl}{Version}/swagger.json?datasource={Datasource}".ToLower();
			}
		}
	}
}
