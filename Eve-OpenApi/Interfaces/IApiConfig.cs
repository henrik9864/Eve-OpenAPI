using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
    public interface IApiConfig
	{
		bool UseRequestQueue { get; set; }

		bool RateLimitThrotle { get; set; }

		string UserAgent { get; set; }

		string SpecURL { get; }
	}
}
