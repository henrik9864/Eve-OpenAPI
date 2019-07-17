namespace Eve_OpenApi.Authentication.Jwt
{
	internal struct JwtKey
	{
		public string alg { get; set; }

		public string crv { get; set; }

		public string e { get; set; }

		public string kid { get; set; }

		public string kty { get; set; }

		public string n { get; set; }

		public string use { get; set; }

		public string x { get; set; }

		public string y { get; set; }
	}
}