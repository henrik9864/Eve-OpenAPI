using EveOpenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EveOpenApi
{
	public class Scope : IScope, IEquatable<IScope>
	{
		public string ScopeString
		{
			get
			{
				return string.Join<string>(' ', scopes.ToArray());
			}
		}

		public IReadOnlyList<string> Scopes
		{
			get
			{
				return new ReadOnlyCollection<string>(scopes);
			}
		}

		List<string> scopes;

		private Scope()
		{
			scopes = new List<string>();
		}

		public void AddScope(IScope scope)
		{
			scopes.AddRange(scope.Scopes);
		}

		public void RemoveScope(IScope scope)
		{
			for (int i = 0; i < scope.Scopes.Count; i++)
			{
				int index = scopes.IndexOf(scope.Scopes[i]);

				if (index >= 0)
					scopes.RemoveAt(index);
				else
					throw new Exception($"Scope does not contain scope '{scope.Scopes[i]}' to remove");
			}
		}

		public bool IsSubset(IScope scope)
		{
			if (string.IsNullOrWhiteSpace(scope.ScopeString))
				return true;

			for (int i = 0; i < scope.Scopes.Count; i++)
			{
				if (scopes.IndexOf(scope.Scopes[i]) < 0)
					return false;
			}

			return true;
		}

		public bool Equals(IScope other)
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

		public static Scope Parse(string scope)
		{
			Scope esiScope = new Scope();
			string[] scopes = scope.Split(' ');

			for (int i = 0; i < scopes.Length; i++)
				esiScope.scopes.Add(scopes[i]);

			return esiScope;
		}

		#region Ovverides

		public override string ToString()
		{
			return ScopeString;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			if (ReferenceEquals(obj, null))
				return false;

			return obj.GetType() == typeof(IScope) && Equals((IScope)obj);
		}

		#endregion

		#region Operators

		public static explicit operator string(Scope scope)
		{
			return scope.ToString();
		}

		public static implicit operator Scope(string scope)
		{
			return Parse(scope);
		}

		public static bool operator ==(Scope obj1, Scope obj2)
		{
			if (ReferenceEquals(obj1, obj2))
				return true;

			if (ReferenceEquals(obj1, null) || ReferenceEquals(null, obj2))
				return false;

			return obj1.Equals(obj2);
		}

		public static bool operator !=(Scope obj1, Scope obj2)
		{
			if (ReferenceEquals(obj1, obj2))
				return false;

			if (ReferenceEquals(obj1, null) || ReferenceEquals(null, obj2))
				return true;

			return !obj1.Equals(obj2);
		}

		#endregion
	}
}
