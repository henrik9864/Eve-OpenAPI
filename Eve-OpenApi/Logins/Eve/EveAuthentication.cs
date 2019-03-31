using EveOpenApi.Interfaces;
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

namespace EveOpenApi.Eve
{
    static partial class EveAuthentication
	{
		static HttpClient Client { get; set; }

		#region Authentication

		/// <summary>
		/// Create new credentials that will last 20 min.
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="refreshToken"></param>
		/// <param name="clientID"></param>
		/// <returns></returns>
		internal static async Task<EveCredentials> RefreshToken(IScope scope, string refreshToken, string clientID)
		{
			string refreshUrl = $"https://login.eveonline.com/v2/oauth/token/";

			KeyValuePair<string, string>[] data = new[]
			{
				new KeyValuePair<string, string>("grant_type", "refresh_token"),
				new KeyValuePair<string, string>("refresh_token", refreshToken),
				new KeyValuePair<string, string>("client_id", clientID),
				new KeyValuePair<string, string>("scope", scope.ScopeString),
			};

			using (HttpResponseMessage response = await Client.PostAsync(refreshUrl, new FormUrlEncodedContent(data)))
			{
				string json = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<EveCredentials>(json);
			}
		}

		/// <summary>
		/// Load EveToken from json created by ToJson method.
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		internal static async Task<EveToken> FromJson(string json, HttpClient httpClient = default)
		{
			if (httpClient != default && Client == null)
				Client = httpClient;

			List<string> content = JsonConvert.DeserializeObject<List<string>>(json);

			Scope scope = content[0];
			string refreshToken = content[1];
			string clientID = content[2];

			EveCredentials credentials = await RefreshToken(scope, refreshToken, clientID);
			JwtToken token = await ValidateCredentials(credentials);

			return new EveToken(credentials, token, scope);
		}

		/// <summary>
		/// Listen for a response from eve on the callback.
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		static async Task<(string code, string state)> GetAuthResponse(string callback)
		{
			string htmlResponse = "<html><body style=\"background-color: grey\">You can close this page.</body></html>";

			System.Collections.Specialized.NameValueCollection parameters;
			using (HttpListener listener = new HttpListener())
			{
				listener.Prefixes.Add($"{callback}/");
				listener.Start();

				HttpListenerContext context = await listener.GetContextAsync();
				using (Stream output = context.Response.OutputStream)
				{
					byte[] buffer = Encoding.UTF8.GetBytes(htmlResponse);

					await output.WriteAsync(buffer, 0, buffer.Length);
					await Task.Delay(buffer.Length); // Fix bug where page would not load on chrome :/
				}

				listener.Stop();
				parameters = HttpUtility.ParseQueryString(context.Request.Url.Query);
			}

			return (parameters.Get(0), parameters.Get(1));
		}

		/// <summary>
		/// Validate that the token received has not been tampered with and get additional character information.
		/// </summary>
		/// <param name="credential"></param>
		/// <returns></returns>
		static async Task<JwtToken> ValidateCredentials(EveCredentials credential)
		{
			JwtToken token;
			using (HttpResponseMessage response = await Client.GetAsync("https://login.eveonline.com/oauth/jwks"))
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
