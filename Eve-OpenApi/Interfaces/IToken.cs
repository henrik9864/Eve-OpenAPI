using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
    public interface IToken
	{
		string AccessToken { get; }

		IRemoveOnlyScope Scope { get; }

		DateTime Expiery { get; }

		Task RefreshToken(IScope subset);

		Task<string> GetToken();
	}
}
