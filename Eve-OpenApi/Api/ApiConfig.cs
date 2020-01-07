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
		public virtual bool UseRequestQueue { get; set; } = true;

		/// <summary>
		/// Wether you allow for the backround loop to use events.
		/// </summary>
		public virtual bool EnableEventQueue { get; set; } = true;

		/// <summary>
		/// Whether to throttle requests or throw exception when rate limit has been reached.
		/// </summary>
		public virtual bool RateLimitThrotle { get; set; } = true;

		/// <summary>
		/// Whether yo use build in cache or not.
		/// </summary>
		public virtual bool UseCache { get; set; } = true;

		/// <summary>
		/// If you want to include the auth header even if no auth is required
		/// </summary>
		public virtual bool AlwaysIncludeAuthHeader { get; set; } = false;

		/// <summary>
		/// User agent to send to ESI.
		/// </summary>
		public virtual string UserAgent { get; set; }

		/// <summary>
		/// Default user to user for tokens.
		/// </summary>
		public virtual string DefaultUser { get; set; }

		/// <summary>
		/// Url to swager spec for the api, usually set by the IApiConfig
		/// </summary>
		public virtual string SpecURL { get; set; }

		public virtual string TokenLocation { get; set; } = "header";

		public virtual string TokenName { get; set; } = "token";

		public virtual string RateLimitHeader { get; }

		public virtual string RateLimitRemainHeader { get; }

		public virtual string RateLimitResetHeader { get; }

		public virtual string PageHeader { get; }
	}
}
