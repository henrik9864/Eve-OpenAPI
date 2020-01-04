using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Authentication
{
	public class KeyLogin : ILogin
	{
		public IToken this[string user, string scope]
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ILoginConfig Config
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Task<IToken> AddToken(IScope scope)
		{
			throw new NotImplementedException();
		}

		public Task<IToken> AddToken(string refreshToken, IScope scope)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetAuthUrl(IScope scope)
		{
			throw new NotImplementedException();
		}

		public IToken GetToken(string user, IScope scope)
		{
			throw new NotImplementedException();
		}

		public IList<IToken> GetTokens(string user)
		{
			throw new NotImplementedException();
		}

		public IList<string> GetUsers()
		{
			throw new NotImplementedException();
		}

		public Task<IToken> RefreshToken(IToken token)
		{
			throw new NotImplementedException();
		}

		public void SaveToFile(string path, bool @override)
		{
			throw new NotImplementedException();
		}

		public string ToEncrypted()
		{
			throw new NotImplementedException();
		}

		public string ToJson()
		{
			throw new NotImplementedException();
		}

		public bool TryGetToken(string user, IScope scope, out IToken token)
		{
			throw new NotImplementedException();
		}
	}
}
