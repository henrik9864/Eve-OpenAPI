namespace EsiNet.Interfaces
{
	public interface IAddOnlyScope : IReadOnlyScope
	{
		void AddScope(Scope scope);
	}
}
