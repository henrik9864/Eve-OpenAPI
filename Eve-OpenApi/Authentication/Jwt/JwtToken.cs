using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EveOpenApi.Authentication
{
	internal struct JwtToken : IJwtToken
	{
		[JsonProperty("jti")]
		public string JwtID { get; set; }

		[JsonProperty("kid")]
		public string KeyIdentifier { get; set; }

		[JsonProperty("sub")]
		public string Subject { get; set; }

		[JsonProperty("azp")]
		public string ClientID { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("owner")]
		public string Owner { get; set; }

		[JsonProperty("exp")]
		public int Expiery { get; set; }

		[JsonProperty("iss")]
		public string Issuer { get; set; }
	}
}
