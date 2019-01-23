namespace EsiNet.Interfaces
{
	public interface IRemoveOnlyScope : IReadOnlyScope
	{
		void RemoveScope(Scope scope);
	}
}
