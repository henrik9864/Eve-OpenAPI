using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
	interface ITokenFactoryAsync<T> where T : IToken
	{
		Task<T> CreateTokenAsync(params object[] context);
	}
}
