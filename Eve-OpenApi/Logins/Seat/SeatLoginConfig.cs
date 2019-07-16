using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Seat
{
	public class SeatLoginConfig : ISeatLoginConfig
	{
		public string User { get; set; }

		public string Token { get; set; }

		public SeatLoginConfig(string user, string token)
		{
			User = user;
			Token = token;
		}
	}
}
