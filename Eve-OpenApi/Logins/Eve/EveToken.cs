using EveOpenApi.Eve;
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

namespace EveOpenApi
{
	public class EveToken : IToken
	{
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

		public DateTime Expiers { get; private set; }

		public IRemoveOnlyScope Scope { get; private set; }

		internal EveCredentials Credential { get; private set; }

		internal JwtToken Token { get; }

		internal EveToken(EveCredentials credential, JwtToken token, IScope scope)
		{
			Scope = scope;
			Token = token;
			Credential = credential;

			string[] subjectArray = Token.Subject.Split(':');
			Expiers = DateTime.Now + new TimeSpan(0, 0, int.Parse(credential.ExpiresIn));
			UserID = int.Parse(subjectArray[2]);
		}

		/// <summary>
		/// Refresh access token.
		/// </summary>
		/// <param name="subset"></param>
		/// <returns></returns>
		public async Task RefreshToken(IScope subset = default)
		{
			if (subset == default)
				subset = (Scope)Scope;
			else
				Scope = subset;

			Credential = await EveAuthentication.RefreshToken(subset, Credential.RefreshToken, Token.ClientID);
			Expiers = DateTime.Now + new TimeSpan(0, 0, int.Parse(Credential.ExpiresIn));
		}

		/// <summary>
		/// Retrive token and automaticly refresh it if expired.
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetToken()
		{
			if (DateTime.Now > Expiers)
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
	}
}
