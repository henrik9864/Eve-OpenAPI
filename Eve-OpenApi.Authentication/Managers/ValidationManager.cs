using EveOpenApi.Authentication.Jwt;
using Jose;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Authentication.Managers
{
	internal class ValidationManager : IValidationManager
	{
		ILoginConfig config;
		IHttpHandler client;

		public ValidationManager(ILoginConfig config, IHttpHandler client)
		{
			this.config = config;
			this.client = client;
		}

		public async Task<IJwtToken> ValidateTokenAsync(IToken token)
		{
			IJwtToken jwtToken;
			using (HttpResponseMessage response = await client.GetAsync(config.JwtKeySetEndpoint))
			{
				Stream stream = await response.Content.ReadAsStreamAsync();
				JwtKeySet keySet = await JsonSerializer.DeserializeAsync<JwtKeySet>(stream);

				var headers = JWT.Headers(token.AccessToken);
				var jwk = keySet.Keys[1];

				RSACryptoServiceProvider key = new RSACryptoServiceProvider();
				key.ImportParameters(new RSAParameters
				{
					Modulus = Base64Url.Decode(jwk.n),
					Exponent = Base64Url.Decode(jwk.e)
				});

				jwtToken = JWT.Decode<JwtToken>(token.AccessToken, key);

				if (jwtToken.Issuer != "login.eveonline.com")
					throw new Exception("Invalid JWT Token!");

				uint unixTimestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				if (jwtToken.Expiery < unixTimestamp)
					throw new Exception("Invalid JWT Token");
			}

			return jwtToken;
		}
	}
}
