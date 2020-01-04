using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EveOpenApi.Authentication
{
	internal struct OauthTokenSave
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonPropertyName("expires_in")]
		public uint Expires { get; set; }

		[JsonPropertyName("token_type")]
		public string TokenType { get; set; }

		public OauthTokenSave(string accessToken, string refreshToken, uint expires, string tokenType)
		{
			AccessToken = accessToken;
			RefreshToken = refreshToken;
			Expires = expires;
			TokenType = tokenType;
		}
	}
}
