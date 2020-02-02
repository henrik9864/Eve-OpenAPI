using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Authentication.Structs
{
	internal struct LoginSave
	{
		public string Type { get; set; }

		public string Data { get; set; }

		public LoginSave(string type, string data)
		{
			this.Type = type;
			this.Data = data;
		}
	}
}
