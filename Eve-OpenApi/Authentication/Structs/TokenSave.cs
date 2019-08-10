namespace EveOpenApi.Authentication
{
	internal struct TokenSave
	{
		public string RefreshToken { get; set; }

		public string Scope { get; set; }

		public TokenSave(string refreshToken, string scope)
		{
			this.RefreshToken = refreshToken;
			this.Scope = scope;
		}
	}
}
