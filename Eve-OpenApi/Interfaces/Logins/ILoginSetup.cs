using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
    public interface ILoginSetup
	{
		/// <summary>
		/// Location where the access token will be set.
		/// </summary>
		string TokenLocation { get; }

		/// <summary>
		/// Parameter name for the access token.
		/// </summary>
		string TokenName { get; }

		/// <summary>
		/// Name of rate-limit header
		/// </summary>
		string RateLimitHeader { get; }

		string RateLimitRemainHeader { get; }

		string RateLimitResetHeader { get; }
	}
}
