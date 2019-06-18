using EveOpenApi.Eve;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EveOpenApi.DependencyInjection
{
	public class EveWebLoginExtension : EveWebLogin
	{
		public EveWebLoginExtension(IOptions<EveWebConfig> options, HttpClient client)
			: base(options.Value.ClientID, options.Value.ClientSecret, options.Value.CallbackUrl, client)
		{

		}
	}
}
