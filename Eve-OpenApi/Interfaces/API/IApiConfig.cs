using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
    public interface IApiConfig
	{
		/// <summary>
		/// Use an internal background loop to regulate requests.
		/// </summary>
		bool UseRequestQueue { get; }

		/// <summary>
		/// Wether you allow for the backround loop to use events.
		/// </summary>
		bool EnableEventQueue { get; }

		/// <summary>
		/// Whether to throttle requests or throw exception when rate limit has been reached.
		/// </summary>
		bool RateLimitThrotle { get; }

		/// <summary>
		/// Whether yo use build in cache or not.
		/// </summary>
		bool UseCache { get; }

		/// <summary>
		/// If you want to include the auth header even if no auth is required
		/// </summary>
		bool AlwaysIncludeAuthHeader { get; }

		/// <summary>
		/// User agent to send to ESI.
		/// </summary>
		string UserAgent { get; }

		/// <summary>
		/// Default user to user for tokens.
		/// </summary>
		string DefaultUser { get; }

		/// <summary>
		/// Url to swager spec for the api, usually set by the IApiConfig
		/// </summary>
		string SpecURL { get; }
	}
}
