namespace EveOpenApi.Api
{
	public interface IApiResponse : IPaginatedResponse<string>, IApiResponseInfo
	{
		int GetHashCode();

		/// <summary>
		/// Json deserialize the response to type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IApiResponse<T> ToType<T>();
	}
}