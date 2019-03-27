using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Interfaces
{
    public interface IScope : IReadOnlyScope, IRemoveOnlyScope, IAddOnlyScope
	{
		
	}
}
