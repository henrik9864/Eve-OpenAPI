using EveOpenApi.Authentication.Interfaces;
using System.Text.Json;

namespace EveOpenApi.Authentication
{
	internal class OauthTokenFactory : IOauthTokenFactory
	{
		public IOauthToken Create(string accessToken, string refreshToken, uint expires, string tokenType, IScope scope)
		{
			OauthTokenSave token = new OauthTokenSave(accessToken, refreshToken, expires, tokenType);
			return new OauthToken(token, scope);
		}

		public IOauthToken FromJson(string json, IScope scope)
		{
			OauthTokenSave token = JsonSerializer.Deserialize<OauthTokenSave>(json);
			return new OauthToken(token, scope);
		}
	}
}
