using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsiNet.Eve
{
	internal class JwtToken
	{
		[JsonProperty("jti")]
		public string JwtID { get; private set; }

		[JsonProperty("kid")]
		public string KeyIdentifier { get; private set; }

		[JsonProperty("sub")]
		public string Subject { get; private set; }

		[JsonProperty("azp")]
		public string ClientID { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public string Owner { get; private set; }

		[JsonProperty("exp")]
		public int Expiery { get; private set; }

		[JsonProperty("iss")]
		public string Issuer { get; private set; }
	}
}
