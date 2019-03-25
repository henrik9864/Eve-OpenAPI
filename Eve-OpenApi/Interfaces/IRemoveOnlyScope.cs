namespace EveOpenApi.Interfaces
{
	public interface IRemoveOnlyScope : IReadOnlyScope
	{
		void RemoveScope(IScope scope);
	}
}
