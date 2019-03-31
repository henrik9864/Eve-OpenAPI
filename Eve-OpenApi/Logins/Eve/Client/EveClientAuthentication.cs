using EveOpenApi.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Eve
{
    static partial class EveAuthentication
	{
		/// <summary>
		/// Create a new EveToken
		/// </summary>
		/// <param name="scope">Scope for this token.</param>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		internal static async Task<EveToken> CreateToken(IScope scope, string clientID, string callback, HttpClient httpClient = default)
		{
			if (httpClient != default && Client == null)
				Client = httpClient;

			var auth = Authenticate(scope, clientID, callback);
			OpenUrl(auth.authUrl);

			return await ValidateResponse(scope, callback, auth.state, auth.verifier, clientID);
		}

		/// <summary>
		/// Generate authentication url, state and codeverifier
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		internal static (string authUrl, string state, string verifier) Authenticate(IScope scope, string clientID, string callback)
		{
			string state = RandomString(8);
			string codeVerifier = GetCodeVerifier();
			string codeChallenge = GenerateCodeChallenge(codeVerifier);

			string authUrl = GetAuthURL(scope, codeChallenge, state, callback, clientID);
			return (authUrl, state, codeVerifier);
		}

		/// <summary>
		/// Wait for auth response after user har logged in.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <param name="codeVerifier"></param>
		/// <param name="clientID"></param>
		/// <returns></returns>
		internal static async Task<EveToken> ValidateResponse(IScope scope, string callback, string state, string codeVerifier, string clientID)
		{
			var response = await GetAuthResponse(callback);
			if (state != response.state)
				throw new Exception("Response state not matching sent state.");

			EveCredentials credential = await RetriveClientCredentials(response.code, codeVerifier, clientID);
			JwtToken token = await ValidateCredentials(credential);

			return new EveToken(credential, token, scope);
		}

		/// <summary>
		/// Create auth URL for client programs.
		/// </summary>
		/// <param name="scope">Scope that will be requested from user.</param>
		/// <param name="codeChallenge">Code challenge to verifi identity</param>
		/// <param name="state">Randomly generated state.</param>
		/// <param name="callback">Where the auth request will send the user.</param>
		/// <param name="clientID">Id of application</param>
		static string GetAuthURL(IScope scope, string codeChallenge, string state, string callback, string clientID)
		{
			return $"https://login.eveonline.com/v2/oauth/authorize/?" +
				$"response_type=code" +
				$"&redirect_uri={callback}" +
				$"&client_id={clientID}" +
				$"&scope={scope.ScopeString.Replace(" ", "%20")}" +
				$"&code_challenge={codeChallenge}" +
				$"&code_challenge_method=S256" +
				$"&state={state}";
		}

		/// <summary>
		/// Get credentials from the auth code.
		/// </summary>
		/// <param name="code">Authentication code retrived from the callback.</param>
		/// <param name="codeVerifier">Code to verifi its was sent by the proper source.</param>
		/// <param name="clientID">Id of application</param>
		/// <returns></returns>
		static async Task<EveCredentials> RetriveClientCredentials(string code, string codeVerifier, string clientID)
		{
			string loginUrl = "https://login.eveonline.com/v2/oauth/token";
			KeyValuePair<string, string>[] data = new[]
			{
				new KeyValuePair<string, string>("grant_type", "authorization_code"),
				new KeyValuePair<string, string>("code", code),
				new KeyValuePair<string, string>("client_id", clientID),
				new KeyValuePair<string, string>("code_verifier", codeVerifier),
			};

			EveCredentials credentials;
			using (HttpResponseMessage response = await Client.PostAsync(loginUrl, new FormUrlEncodedContent(data)))
			{
				string json = await response.Content.ReadAsStringAsync();
				credentials = JsonConvert.DeserializeObject<EveCredentials>(json);
			}

			return credentials;
		}
	}
}
