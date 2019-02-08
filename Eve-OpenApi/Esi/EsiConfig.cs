using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Esi
{
	public class EsiConfig
	{
		/// <summary>
		/// Automaticly ask for the needed scope if it has not already been granted.
		/// </summary>
		public bool AutoRequestScope { get; set; } = false;

		/// <summary>
		/// User agent to send to ESI.
		/// </summary>
		public string UserAgent { get; set; }
	}
}
