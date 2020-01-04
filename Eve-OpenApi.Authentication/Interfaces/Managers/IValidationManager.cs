using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Authentication.Managers
{
	internal interface IValidationManager
	{
		Task<IJwtToken> ValidateTokenAsync(IToken token);
	}
}