using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
    public interface IToken
	{
		string AccessToken { get; }

		/// <summary>
		/// Name for the character or person this token is connected to
		/// </summary>
		string Name { get; }

		IRemoveOnlyScope Scope { get; }

		DateTime Expiers { get; }

		Task RefreshToken(IScope subset);

		Task<string> GetToken();
	}
}
