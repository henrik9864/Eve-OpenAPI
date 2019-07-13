namespace EveOpenApi.Api
{
	public interface IApiResponse<T> : IApiResponse
	{
		/// <summary>
		/// Response from the API deserialized into type.
		/// </summary>
		new T Response { get; }
	}
}