using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Seat
{
    class SeatInterfaceSetup : IInterfaceSetup
	{
		public string TokenLocation { get; } = "header";

		public string TokenName { get; } = "X-Token";

		public string RateLimitHeader { get; } = "";

		public string RateLimitRemainHeader { get; } = "";

		public string RateLimitResetHeader { get; } = "";
	}
}
