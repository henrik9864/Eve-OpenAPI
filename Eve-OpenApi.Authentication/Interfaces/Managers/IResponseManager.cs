using EveOpenApi.Interfaces;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Authentication.Managers
{
	internal interface IResponseManager
	{
		Task<IAuthResponse> GetResponse(string authUrl, int timeout);

		Task<IAuthResponse> AwaitResponse(int timeout);
	}
}