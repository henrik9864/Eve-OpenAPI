using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Eve
{
	class EveInterfaceSetup : IInterfaceSetup
	{
		public string TokenLocation { get; } = "query";

		public string TokenName { get; } = "token";

		public string RateLimitHeader { get; } = "";

		public string RateLimitRemainHeader { get; } = "x-esi-error-limit-remain";

		public string RateLimitResetHeader { get; } = "x-esi-error-limit-reset";
	}
}
