using EveOpenApi.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication.Interfaces
{
	public interface IOauthLogin : ILogin
	{
		ILoginConfig Config { get; }

		ILoginCredentials Credentials { get; }

		Task<IOauthToken> RefreshToken(IOauthToken token);

		Task<IOauthToken> AddToken(IScope scope);

		Task<IOauthToken> AddToken(string refreshToken, IScope scope);

		Task<string> GetAuthUrl(IScope scope);

		void SaveToFileEncrypted(string path, bool @override);

		string ToEncrypted();
	}
}
