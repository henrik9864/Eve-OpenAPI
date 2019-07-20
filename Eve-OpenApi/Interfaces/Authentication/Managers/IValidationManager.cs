using System.Threading.Tasks;

namespace EveOpenApi.Authentication.Managers
{
	internal interface IValidationManager
	{
		Task<IJwtToken> ValidateTokenAsync(IToken token);
	}
}