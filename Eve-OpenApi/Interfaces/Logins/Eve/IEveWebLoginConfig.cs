namespace EveOpenApi.Eve
{
	public interface IEveWebLoginConfig
	{
		string Callback { get; set; }
		string ClientID { get; set; }
		string ClientSecret { get; set; }
	}
}