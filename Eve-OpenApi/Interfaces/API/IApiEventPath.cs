using System.Collections.Generic;

namespace EveOpenApi.Api
{
	public interface IApiEventPath
	{
		IList<string> DefaultUsers { get; }

		string Path { get; }

		IApiEventMethod Get(params (string, object)[] parameters);

		IApiEventMethod GetBatch(params (string, List<object>)[] parameters);

		IApiEventPath SetUsers(params string[] users);
	}
}