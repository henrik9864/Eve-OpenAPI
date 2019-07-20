using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	internal struct AuthUrl
	{
		public string Url { get; set; }

		public string State { get; set; }

		public string CodeVerifier { get; set; }

		public AuthUrl(string url, string state, string codeVerifier)
		{
			Url = url;
			State = state;
			CodeVerifier = codeVerifier;
		}
	}
}
