using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi
{
	public class Scope : IReadOnlyScope, IRemoveOnlyScope, IAddOnlyScope
	{
		public string ScopeString
		{
			get
			{
				return string.Join(' ', scopes.ToArray());
			}
		}

		public IReadOnlyList<string> Scopes
		{
			get
			{
				return scopes;
			}
		}

		List<string> scopes;

		private Scope()
		{
			scopes = new List<string>();
		}

		public void AddScope(Scope scope)
		{
			scopes.AddRange(scope.scopes);
		}

		public void RemoveScope(Scope scope)
		{
			foreach (string scopeStr in scope.scopes)
			{
				int index = scope.scopes.IndexOf(scopeStr);

				if (index >= 0)
					scope.scopes.RemoveAt(index);
				else
					throw new Exception($"Scope does not already contain scope '{scopeStr}' to remove");
			}
		}

		public bool HasScope(Scope scope)
		{
			for (int i = 0; i < scope.Scopes.Count; i++)
			{
				if (scopes.IndexOf(scope.Scopes[i]) < 0)
					return false;
			}

			return true;
		}

		internal bool Validate(List<string> scopes)
		{
			if (scopes.Count != this.scopes.Count)
				return false;

			for (int i = 0; i < scopes.Count; i++)
			{
				if (scopes[i] != this.scopes[i])
					return false;
			}

			return true;
		}

		public override string ToString()
		{
			return ScopeString;
		}

		public static Scope Parse(string scope)
		{
			string[] tokens = scope.Split(' ');
			Scope esiScope = new Scope();

			for (int i = 0; i < tokens.Length; i++)
				esiScope.scopes.Add(tokens[i]);

			return esiScope;
		}

		public static explicit operator string(Scope scope)
		{
			return scope.ScopeString;
		}

		public static implicit operator Scope(string scope)
		{
			return Parse(scope);
		}
	}
}
