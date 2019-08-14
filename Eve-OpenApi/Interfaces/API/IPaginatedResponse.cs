using System.Collections.Generic;

namespace EveOpenApi.Api
{
	public interface IPaginatedResponse<T> : IEnumerable<T>
	{
		/// <summary>
		/// First page of the API response
		/// </summary>
		T FirstPage { get; }

		int MaxPages { get; }
	}
}