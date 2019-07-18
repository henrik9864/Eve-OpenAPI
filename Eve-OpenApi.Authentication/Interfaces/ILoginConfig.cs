namespace EveOpenApi.Authentication
{
	public interface ILoginConfig
	{
		string AuthenticationEndpoint { get; set; }

		string TokenEndpoint { get; set; }

		string JwtKeySetEndpoint { get; set; }

		string AuthType { get; set; }
	}
}