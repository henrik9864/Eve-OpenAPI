namespace EveOpenApi.Interfaces
{
	public interface IAddOnlyScope : IReadOnlyScope
	{
		void AddScope(IScope scope);
	}
}
