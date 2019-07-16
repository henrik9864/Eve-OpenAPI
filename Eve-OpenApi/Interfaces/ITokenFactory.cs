using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
	interface ITokenFactory<T> where T : IToken
	{
		T CreateToken(params object[] context);
	}
}
