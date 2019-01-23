using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Interfaces
{
	/// <summary>
	/// I know you can just cast to get around this but it servers as a warning
	/// </summary>
	public interface IReadOnlyScope
	{
		string ScopeString { get; }

		IReadOnlyList<string> Scopes { get; }

		bool HasScope(Scope scope);
	}
}
