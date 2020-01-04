using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Interfaces
{
	internal interface IFactory<T>
	{
		T Create(params object[] context);
	}
}
