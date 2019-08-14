using EveOpenApi.Managers.CacheControl;
using System;

namespace EveOpenApi.Api
{
	public interface IApiResponse<T> : IPaginatedResponse<T>, IApiResponseInfo
	{
	}
}