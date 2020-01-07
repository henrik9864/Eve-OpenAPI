using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	public class OauthLoginConfig : ILoginConfig
	{
		public string AuthenticationEndpoint { get; set; }

		public string TokenEndpoint { get; set; }

		public string JwtKeySetEndpoint { get; set; }

		public string AuthType { get; set; }

		public string TokenLocation { get; set; }

		public string TokenName { get; set; }

		public static OauthLoginConfig Tranquility
		{
			get
			{
				return new OauthLoginConfig()
				{
					AuthenticationEndpoint = "https://login.eveonline.com/v2/oauth/authorize/",
					TokenEndpoint = "https://login.eveonline.com/v2/oauth/token/",
					JwtKeySetEndpoint = "https://login.eveonline.com/oauth/jwks",
					AuthType = "basic",
				};
			}
		}

		public static OauthLoginConfig Singularity
		{
			get
			{
				return new OauthLoginConfig()
				{
					AuthenticationEndpoint = "https://sisilogin.testeveonline.com/oauth/authorize",
					TokenEndpoint = "https://sisilogin.testeveonline.com/oauth/token",
					JwtKeySetEndpoint = "https://sisilogin.testeveonline.com/oauth/verify",
					AuthType = "basic",
				};
			}
		}
	}
}
