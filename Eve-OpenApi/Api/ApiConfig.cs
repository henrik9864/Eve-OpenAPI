using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api
{
	public class ApiConfig : IApiConfig
	{
		/// <summary>
		/// Automaticly ask for the needed scope if it has not already been granted.
		/// </summary>
		//public bool AutoRequestScope { get; set; } = false;

		/// <summary>
		/// Use an internal background loop to regulate requests.
		/// </summary>
		public bool UseInternalLoop { get; set; } = true;

		/// <summary>
		/// Whether to throttle requests or throw exception when rate limit has been reached.
		/// </summary>
		public bool RateLimitThrotle { get; set; } = true;

		/// <summary>
		/// User agent to send to ESI.
		/// </summary>
		public string UserAgent { get; set; }

		public virtual string SpecURL { get; set; }
	}
}
