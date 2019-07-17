using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	public class LoginConfig : ILoginConfig
	{
		public string AuthenticationEndpoint { get; set; }

		public string TokenEndpoint { get; set; }

		public string JwtKeySetEndpoint { get; set; }
	}
}
