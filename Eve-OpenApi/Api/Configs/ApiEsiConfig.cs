using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Configs
{
	public class EsiConfig : ApiConfig, IEsiConfig
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

		public override string TokenLocation
		{
			get
			{
				return "query";
			}
		}

		public override string TokenName
		{
			get
			{
				return "token";
			}
		}

		public override string RateLimitRemainHeader
		{
			get
			{
				return "X-Esi-Error-Limit-Remain";
			}
		}

		public override string RateLimitResetHeader
		{
			get
			{
				return "X-Esi-Error-Limit-Reset";
			}
		}

		public override string PageHeader
		{
			get
			{
				return "X-Pages";
			}
		}
	}
}
