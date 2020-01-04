using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
	/// <summary>
	/// I know you can just cast to get around this but it servers as a warning
	/// </summary>
	public interface IReadOnlyScope
	{
		string ScopeString { get; }

		IReadOnlyList<string> Scopes { get; }

		/// <summary>
		/// Check if <paramref name="scope"/> is a subset of this scope.
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		bool IsSubset(IReadOnlyScope scope);
	}
}
