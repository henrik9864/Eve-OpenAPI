using EveOpenApi.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Interfaces
{
	public interface ICacheControl
	{
		Cacheability Cacheability { get; }

		uint TimeUntillStale { get; }

		Validation Validation { get; }
	}
}
