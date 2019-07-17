using EveOpenApi.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Eve
{
    static partial class EveAuthentication
	{
		/// <summary>
		/// Generate a auth URL and a state for web applications.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		internal static (string, string) GetAuthUrl(IScope scope, string clientID, string callback)
		{
			string state = RandomString(8);
			return (GetAuthURL(scope, clientID, callback, state), state);
		}

		internal static async Task<EveToken> GetWebToken(IScope scope, string code, string clientID, string clientSecret, IHttpHandler client)
		{
			EveCredentials credentials = await RetriveWebCredentials(code, clientID, clientSecret, client);
			JwtToken jwtToken = await ValidateCredentials(credentials, client);

			return new EveToken(credentials, jwtToken, scope);
		}

		/// <summary>
		/// Create a auth URL for web applications.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		static string GetAuthURL(IScope scope, string clientID, string callback, string state)
		{
			return $"https://login.eveonline.com/v2/oauth/authorize/?" +
				$"response_type=code" +
				$"&redirect_uri={callback}" +
				$"&client_id={clientID}" +
				$"&scope={scope.ScopeString.Replace(" ", "%20")}" +
				$"&state={state}";
		}

		static async Task<EveCredentials> RetriveWebCredentials(string code, string clientID, string clientSecret, IHttpHandler client)
		{
			string loginUrl = "https://login.eveonline.com/v2/oauth/token";
			KeyValuePair<string, string>[] data = new[]
			{
				new KeyValuePair<string, string>("grant_type", "authorization_code"),
				new KeyValuePair<string, string>("code", code),
			};

			string authString = $"{clientID}:{clientSecret}";
			string urlSafe = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));

			Console.WriteLine(authString);
			Console.WriteLine(urlSafe);
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
			request.Content = new FormUrlEncodedContent(data);

			request.Headers.TryAddWithoutValidation("Authorization", "Basic " + urlSafe);


			EveCredentials credentials;
			using (HttpResponseMessage response = await client.SendAsync(request))
			{
				Console.WriteLine(await response.Content.ReadAsStringAsync());
				string json = await response.Content.ReadAsStringAsync();
				credentials = JsonConvert.DeserializeObject<EveCredentials>(json);
			}

			return credentials;
		}
	}
}
