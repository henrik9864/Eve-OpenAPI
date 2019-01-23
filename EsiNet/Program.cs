using EsiNet.Enums;
using Jose;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EsiNet
{
	class Program
	{
        static string clientID = "a72fcc9ce4424ce3848d0edaa5aebbf7";
        static string callback = "http://localhost:8080";

		static void Main(string[] args)
		{
			new Program().Run().GetAwaiter().GetResult();
		}

		async Task Run()
		{
			HttpClient client = new HttpClient();
			EsiNetConfig config = new EsiNetConfig()
			{
				AutoRequestScope = true,
			};

			Console.WriteLine("Login");
			EveLogin login = await EveLogin.FromFile("Save.json", client);
			Console.WriteLine("ENet");
			EsiNet esiNet = await EsiNet.Create(EsiVersion.Latest, Datasource.Tranquility, login, client, config);
			Console.WriteLine("Response");
			EsiResponse<dynamic> response = await esiNet.Path("/characters/{character_id}/attributes/")
				.Get<dynamic>("Prople Dudlestreis", ("character_id", "96037287"));
			//response = await esiNet.Path("/alliances/").Run(OperationType.Get, "Prople Dudlestreis");

			await login.SaveToFile("Save.json");
			Console.WriteLine($"Response: {response.Response.willpower}");
		}
    }
}
