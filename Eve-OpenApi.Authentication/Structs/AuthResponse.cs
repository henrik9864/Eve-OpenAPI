
using EveOpenApi.Interfaces;

namespace EveOpenApi.Authentication.Managers
{
	internal struct AuthResponse : IAuthResponse
	{
		public string Code { get; set; }

		public string State { get; set; }
	}
}