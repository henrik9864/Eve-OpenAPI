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
		public bool UseRequestQueue { get; set; } = true;

		/// <summary>
		/// Wether you allow for the backround loop to use events.
		/// </summary>
		public bool EnableEventQueue { get; set; } = true;

		/// <summary>
		/// Whether to throttle requests or throw exception when rate limit has been reached.
		/// </summary>
		public bool RateLimitThrotle { get; set; } = true;

		/// <summary>
		/// Whether yo use build in cache or not.
		/// </summary>
		public bool UseCache { get; set; } = true;

		/// <summary>
		/// User agent to send to ESI.
		/// </summary>
		public string UserAgent { get; set; }

		/// <summary>
		/// Default user to user for tokens.
		/// </summary>
		public string DefaultUser { get; set; }

		/// <summary>
		/// Url to swager spec for the api, usually set by the IApiConfig
		/// </summary>
		public virtual string SpecURL { get; set; }
	}
}
