using EveOpenApi.Interfaces.Configs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api.Configs
{
	public class SeATConfig : ApiConfig, ISeATConfig
	{
		public override string TokenLocation
		{
			get
			{
				return "header";
			}
		}

		public override string TokenName
		{
			get
			{
				return "X-Token";
			}
		}

		public override bool AlwaysIncludeAuthHeader
		{
			get
			{
				return true;
			}

			set
			{
				base.AlwaysIncludeAuthHeader = value;
			}
		}
	}
}
