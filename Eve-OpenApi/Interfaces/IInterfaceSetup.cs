using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
    public interface IInterfaceSetup
	{
		string TokenLocation { get; }

		string TokenName { get; }

		string RateLimitHeader { get; }

		string RateLimitRemainHeader { get; }

		string RateLimitResetHeader { get; }
	}
}
