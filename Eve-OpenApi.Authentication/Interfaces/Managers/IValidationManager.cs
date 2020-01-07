using EveOpenApi.Authentication.Interfaces;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Authentication.Managers
{
	internal interface IValidationManager
	{
		Task<IJwtToken> ValidateTokenAsync(IOauthToken token);
	}
}