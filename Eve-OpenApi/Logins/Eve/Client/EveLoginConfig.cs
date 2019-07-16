using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Eve
{
	public class EveLoginConfig : IEveLoginConfig
	{
		public string ClientID { get; set; }

		public string Callback { get; set; } = "https://localhost:8080/";

		public EveLoginConfig(string clientID)
		{
			ClientID = clientID;
		}

		public EveLoginConfig(string clientID, string callback) : this(clientID)
		{
			Callback = callback;
		}
	}
}
