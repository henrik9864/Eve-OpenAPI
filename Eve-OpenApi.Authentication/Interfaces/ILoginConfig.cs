namespace EveOpenApi.Authentication
{
	public interface ILoginConfig
	{
		string AuthenticationEndpoint { get; set; }

		string TokenEndpoint { get; set; }

		string JwtKeySetEndpoint { get; set; }

		string AuthType { get; set; }

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