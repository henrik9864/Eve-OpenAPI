using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Interfaces
{
	internal interface IAuthResponse
	{
		string Code { get; }

		string State { get; }
	}
}
