using EveOpenApi.Api;
using System;

namespace EveOpenApi.Authentication
{
	public interface IToken
	{
		IScope Scope { get; }

		string GetToken();
	}
}