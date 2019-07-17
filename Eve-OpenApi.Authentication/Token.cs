using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EveOpenApi.Authentication
{
	internal struct Token : IToken
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonPropertyName("expires_in")]
		public uint Expires { get; set; }

		[JsonPropertyName("token_type")]
		public string TokenType { get; set; }
	}
}
