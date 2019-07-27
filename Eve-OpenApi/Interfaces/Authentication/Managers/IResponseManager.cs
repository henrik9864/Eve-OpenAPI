using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Authentication.Managers
{
	internal interface IResponseManager
	{
		Task<AuthResponse> GetResponse(string authUrl, int timeout);

		Task<AuthResponse> AwaitResponse(int timeout);
	}
}