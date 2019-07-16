using EveOpenApi.Interfaces;
using EveOpenApi.Seat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Logins.Seat
{
	internal class SeatTokenFactory : ITokenFactory<SeatToken>
	{
		public SeatToken CreateToken(params object[] context)
		{
			string token = (string)context[0];
			string user = (string)context[1];

			return new SeatToken(token, user);
		}
	}
}
