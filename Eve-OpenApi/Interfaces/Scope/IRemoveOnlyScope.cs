namespace EveOpenApi.Authentication
{
	public interface IRemoveOnlyScope : IReadOnlyScope
	{
		void RemoveScope(IScope scope);
	}
}
