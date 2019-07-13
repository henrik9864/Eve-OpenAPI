using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Interfaces
{
	internal interface IFactory<T>
	{
		T Create(params object[] context);
	}
}
