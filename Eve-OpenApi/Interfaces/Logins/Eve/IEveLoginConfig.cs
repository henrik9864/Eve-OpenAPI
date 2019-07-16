namespace EveOpenApi.Eve
{
	public interface IEveLoginConfig
	{
		string Callback { get; set; }

		string ClientID { get; set; }
	}
}