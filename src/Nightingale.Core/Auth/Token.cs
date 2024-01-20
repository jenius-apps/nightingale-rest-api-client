using Newtonsoft.Json;

namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Represents an authentication token.
    /// Usually used in OAuth 2.0 scenarios.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The access token.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Describes the token type.
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// The time in seconds until access token expires.
        /// </summary>
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// A refresh token.
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
