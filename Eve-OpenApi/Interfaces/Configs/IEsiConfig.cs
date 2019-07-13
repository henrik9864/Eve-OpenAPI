using EveOpenApi.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Interfaces
{
	public interface IEsiConfig : IApiConfig
	{
		string BaseUrl { get; set; }

		EsiVersion Version { get; set; }

		Datasource Datasource { get; set; }
	}
}
