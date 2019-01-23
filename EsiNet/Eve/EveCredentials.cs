using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsiNet.Eve
{
    internal class EveCredentials
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; private set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; private set; }

        [JsonProperty("token_type")]
        public string TokenType { get; private set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; private set; }
    }
}
