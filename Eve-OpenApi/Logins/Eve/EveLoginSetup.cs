using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Eve
{
	class EveLoginSetup : ILoginSetup
	{
		public string TokenLocation { get; set; } = "query";

		public string TokenName { get; set; } = "token";

		public string RateLimitHeader { get; set; } = "";

		public string RateLimitRemainHeader { get; set; } = "x-esi-error-limit-remain";

		public string RateLimitResetHeader { get; set; } = "x-esi-error-limit-reset";
	}
}
