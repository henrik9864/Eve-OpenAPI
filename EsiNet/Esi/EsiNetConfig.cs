using System;
using System.Collections.Generic;
using System.Text;

namespace EsiNet
{
	public class EsiNetConfig
	{
		/// <summary>
		/// Automaticly ask for the needed scope if it has not already been granted.
		/// </summary>
		public bool AutoRequestScope { get; set; } = false;
	}
}
