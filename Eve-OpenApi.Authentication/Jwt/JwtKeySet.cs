using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Eve_OpenApi.Authentication.Jwt
{
	internal struct JwtKeySet
	{
		public bool SkipUnresolvedJsonWebKeys { get; set; }

		[JsonPropertyName("keys")]
		public List<JwtKey> Keys { get; set; }
	}
}
