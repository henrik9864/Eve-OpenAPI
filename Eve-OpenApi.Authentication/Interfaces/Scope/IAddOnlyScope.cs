namespace EveOpenApi.Authentication
{
	public interface IAddOnlyScope : IReadOnlyScope
	{
		void AddScope(IScope scope);
	}
}
