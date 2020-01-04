namespace EveOpenApi.Authentication
{
	internal interface IJwtToken
	{
		string ClientID { get; }

		int Expiery { get; }

		string Issuer { get; }

		string JwtID { get; }

		string KeyIdentifier { get; }

		string Name { get; }

		string Owner { get; }

		string Subject { get; }
	}
}