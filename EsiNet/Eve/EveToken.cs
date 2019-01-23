using EsiNet.Eve;
using EsiNet.Interfaces;
using Jose;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EsiNet
{
	public class EveToken
	{
		static HttpClient client;

		/// <summary>
		/// Access token (May be outdayed, remember to check Expiery or use GetToken)
		/// </summary>
		public string AccessToken
		{
			get
			{
				return Credential.AccessToken;
			}
		}

		public string Name
		{
			get
			{
				return Token.Name;
			}
		}

		public int UserID { get; }

		public DateTime Expiery { get; private set; }

		public IRemoveOnlyScope Scope { get; private set; }

		internal EveCredentials Credential { get; private set; }

		internal JwtToken Token { get; }

		private EveToken(EveCredentials credential, JwtToken token, Scope scope)
		{
			Scope = scope;
			Token = token;
			Credential = credential;

			string[] subjectArray = Token.Subject.Split(':');
			Expiery = DateTime.Now + new TimeSpan(0, 0, int.Parse(credential.ExpiresIn));
			UserID = int.Parse(subjectArray[2]);
		}

		/// <summary>
		/// Refresh access token.
		/// </summary>
		/// <param name="subset"></param>
		/// <returns></returns>
		public async Task RefreshToken(Scope subset = default)
		{
			if (subset == default)
				subset = (Scope)Scope;
			else
				Scope = subset;

			Credential = await RefreshToken(subset, Credential.RefreshToken, Token.ClientID, client);
			Expiery = DateTime.Now + new TimeSpan(0, 0, int.Parse(Credential.ExpiresIn));
		}

		/// <summary>
		/// Retrive token and automaticly refresh it if expired.
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetToken()
		{
			if (DateTime.Now > Expiery)
				await RefreshToken();

			return AccessToken;
		}

		internal string ToJson()
		{
			List<string> jsonList = new List<string>()
			{
				Scope.ScopeString,
				Credential.RefreshToken,
				Token.ClientID
			};

			return JsonConvert.SerializeObject(jsonList);
		}

		#region Authentication

		/// <summary>
		/// Create a new EveToken
		/// </summary>
		/// <param name="scope">Scope for this token.</param>
		/// <param name="clientID"></param>
		/// <param name="callback"></param>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		public static async Task<EveToken> Create(Scope scope, string clientID, string callback, HttpClient httpClient = default)
		{
			if (httpClient != default && client == null)
				client = httpClient;

			string state = RandomString(8);
			string codeVerifier = GetCodeVerifier();
			string codeChallenge = GenerateCodeChallenge(codeVerifier);

			OpenAuthURL(scope, codeChallenge, state, callback, clientID);
			var response = await GetAuthResponse(callback);

			if (state != response.state)
				throw new Exception("Response state not matching sent state.");

			EveCredentials credential = await RetriveCredentials(response.code, codeVerifier, clientID);
			JwtToken token = await ValidateCredentials(credential);

			if (!scope.Validate((List<string>)scope.Scopes))
				throw new Exception("Eve not returning expected scopes.");

			return new EveToken(credential, token, scope);
		}

		/// <summary>
		/// Load EveToken from json created by ToJson method.
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static async Task<EveToken> FromJson(string json, HttpClient httpClient = default)
		{
			if (httpClient != default && client == null)
				client = httpClient;

			List<string> content = JsonConvert.DeserializeObject<List<string>>(json);

			Scope scope = content[0];
			string refreshToken = content[1];
			string clientID = content[2];

			EveCredentials credentials = await RefreshToken(scope, refreshToken, clientID, client);
			JwtToken token = await ValidateCredentials(credentials);

			return new EveToken(credentials, token, scope);
		}

		/// <summary>
		/// Create and open the authentication URL.
		/// </summary>
		/// <param name="scope">Scope that will be requested from user.</param>
		/// <param name="codeChallenge">Code challenge to verifi identity</param>
		/// <param name="state">Randomly generated state.</param>
		/// <param name="callback">Where the auth request will send the user.</param>
		/// <param name="clientID">Id of application</param>
		static void OpenAuthURL(Scope scope, string codeChallenge, string state, string callback, string clientID)
		{
			string authURL = $"https://login.eveonline.com/v2/oauth/authorize/?" +
				$"response_type=code" +
				$"&redirect_uri={callback}" +
				$"&client_id={clientID}" +
				$"&scope={scope.ScopeString.Replace(" ", "%20")}" +
				$"&code_challenge={codeChallenge}" +
				$"&code_challenge_method=S256" +
				$"&state={state}";

			OpenUrl(authURL);
		}

		/// <summary>
		/// Wait and retrive response from EveLogin via the callback.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		static async Task<(string code, string state)> GetAuthResponse(string callback)
		{
			System.Collections.Specialized.NameValueCollection parameters;
			using (HttpListener listener = new HttpListener())
			{
				listener.Prefixes.Add($"{callback}/");
				listener.Start();

				HttpListenerContext context = await listener.GetContextAsync();
				using (Stream output = context.Response.OutputStream)
				using (StreamReader reader = new StreamReader("Html/ResponsePage.html"))
				{
					byte[] buffer = Encoding.UTF8.GetBytes(reader.ReadToEnd());

					await output.WriteAsync(buffer, 0, buffer.Length);
					await Task.Delay(buffer.Length); // Fix bug where page would not load on chrome :/
				}

				listener.Stop();
				parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
			}

			return (parameters.Get(0), parameters.Get(1));
		}

		/// <summary>
		/// Get credentioals from the auth code.
		/// </summary>
		/// <param name="code">Authentication code retrived from the callback.</param>
		/// <param name="codeVerifier">Code to verifi its was sent by the proper source.</param>
		/// <param name="clientID">Id of application</param>
		/// <returns></returns>
		static async Task<EveCredentials> RetriveCredentials(string code, string codeVerifier, string clientID)
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
			using (HttpResponseMessage response = await client.PostAsync(loginUrl, new FormUrlEncodedContent(data)))
			{
				string json = await response.Content.ReadAsStringAsync();
				credentials = JsonConvert.DeserializeObject<EveCredentials>(json);
			}

			return credentials;
		}

		/// <summary>
		/// Validate that the token received has not been tampered with and get additional character information.
		/// </summary>
		/// <param name="credential"></param>
		/// <returns></returns>
		static async Task<JwtToken> ValidateCredentials(EveCredentials credential)
		{
			JwtToken token;
			using (HttpResponseMessage response = client.GetAsync("https://login.eveonline.com/oauth/jwks").GetAwaiter().GetResult())
			{
				string json = await response.Content.ReadAsStringAsync();
				Dictionary<string, JToken> keys = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);

				var headers = JWT.Headers(credential.AccessToken);
				var jwk = keys["keys"];

				RSACryptoServiceProvider key = new RSACryptoServiceProvider();
				key.ImportParameters(new RSAParameters
				{
					Modulus = Base64Url.Decode(jwk[1]["n"].ToString()),
					Exponent = Base64Url.Decode(jwk[1]["e"].ToString())
				});

				token = JWT.Decode<JwtToken>(credential.AccessToken, key);

				if (token.Issuer != "login.eveonline.com")
					throw new Exception("Invalid JWT Token");

				int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				if (token.Expiery < unixTimestamp)
					throw new Exception("Invalid JWT Token");
			}

			return token;
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Create new credentials that will last 20 min.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="refreshToken"></param>
		/// <param name="clientID"></param>
		/// <returns></returns>
		static async Task<EveCredentials> RefreshToken(Scope scope, string refreshToken, string clientID, HttpClient client)
		{
			string refreshUrl = $"https://login.eveonline.com/v2/oauth/token/";

			KeyValuePair<string, string>[] data = new[]
			{
				new KeyValuePair<string, string>("grant_type", "refresh_token"),
				new KeyValuePair<string, string>("refresh_token", refreshToken),
				new KeyValuePair<string, string>("client_id", clientID),
				new KeyValuePair<string, string>("scope", scope.ScopeString),
			};

			using (HttpResponseMessage response = await client.PostAsync(refreshUrl, new FormUrlEncodedContent(data)))
			{
				string json = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<EveCredentials>(json);
			}
		}

		/// <summary>
		/// Generate random string of X length.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		static string RandomString(int length)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
			char[] stringChars = new char[length];
			using (var numberGen = new RNGCryptoServiceProvider())
			{
				for (int i = 0; i < length; i++)
				{
					stringChars[i] = chars[RandomNumber(numberGen, chars.Length)];
				}
			}

			return new string(stringChars);
		}

		/// <summary>
		/// Generate a random number in range.
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="range"></param>
		/// <returns></returns>
		static int RandomNumber(RNGCryptoServiceProvider generator, int range)
		{
			byte[] randomBytes = new byte[2];
			generator.GetBytes(randomBytes);
			int randomNumber = BitConverter.ToUInt16(randomBytes, 0);
			return (int)Math.Floor((randomNumber / 65536f) * range);
		}

		/// <summary>
		/// URL safe encode bytes to base64
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		static string UrlSafeBase64(byte[] bytes)
		{
			char[] padding = { '=' };
			return Convert.ToBase64String(bytes).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
		}

		/// <summary>
		/// https://stackoverflow.com/questions/4580263/how-to-open-in-default-browser-in-c-sharp
		/// </summary>
		/// <param name="url"></param>
		static void OpenUrl(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch
			{
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start("xdg-open", url);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					Process.Start("open", url);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Generate a 32 byte code for Auth0
		/// </summary>
		/// <returns></returns>
		static string GetCodeVerifier()
		{
			string randomString = RandomString(32); // Used to make the code challenge and verifier
			byte[] verifierBytes = System.Text.Encoding.UTF8.GetBytes(randomString);

			return UrlSafeBase64(verifierBytes);
		}

		/// <summary>
		/// Generate a code challenge from the 32 byte code
		/// </summary>
		/// <param name="codeVerifier"></param>
		/// <returns></returns>
		static string GenerateCodeChallenge(string codeVerifier)
		{
			byte[] verifierBytes = System.Text.Encoding.UTF8.GetBytes(codeVerifier);
			byte[] hash;
			using (SHA256 myHashGen = SHA256.Create())
			{
				hash = myHashGen.ComputeHash(verifierBytes);
			}

			return UrlSafeBase64(hash);
		}

		#endregion
	}
}
