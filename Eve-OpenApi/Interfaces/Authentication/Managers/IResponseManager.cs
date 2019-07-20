using System.Threading.Tasks;

namespace EveOpenApi.Authentication.Managers
{
	internal interface IResponseManager
	{
		Task<AuthResponse> GetResponse(string authUrl, int timeout);

		Task<AuthResponse> AwaitResponse(int timeout);
	}
}