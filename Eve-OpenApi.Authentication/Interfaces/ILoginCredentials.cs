namespace EveOpenApi.Authentication
{
	public interface ILoginCredentials
	{
		string Callback { get; set; }

		string ClientID { get; set; }

		string ClientSecret { get; set; }

		string AuthType { get; set; }
	}
}