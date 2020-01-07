using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication
{
    public interface IScope : IReadOnlyScope, IRemoveOnlyScope, IAddOnlyScope
	{
		
	}
}
