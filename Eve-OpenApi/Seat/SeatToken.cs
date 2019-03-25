using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Seat
{
	public class SeatToken : IToken
	{
		public string AccessToken { get; }

		public IRemoveOnlyScope Scope { get; }

		public DateTime Expiery { get; }

		public async Task<string> GetToken()
		{
			await Task.CompletedTask;
			return AccessToken;
		}

		public async Task RefreshToken(IScope subset)
		{
			await Task.CompletedTask;
		}

		public SeatToken(string token)
		{
			AccessToken = token;
			Scope = (Scope)"";
			Expiery = DateTime.MaxValue;
		}
	}
}
