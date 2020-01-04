using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	public class LoginBuilder
	{
		/// <summary>
		/// Standard oauth login.
		/// </summary>
		public OAuthLoginBuilder OAuth
		{
			get
			{
				return new OAuthLoginBuilder();
			}
		}

		public KeyLoginBuilder Key
		{
			get
			{
				return new KeyLoginBuilder();
			}
		}

		/// <summary>
		/// OAuth login preconfigured for eve tranquility sso
		/// </summary>
		public OAuthLoginBuilder Eve
		{
			get
			{
				return new OAuthLoginBuilder()
					.WithConfig(OauthLoginConfig.Tranquility);
			}
		}
	}
}
