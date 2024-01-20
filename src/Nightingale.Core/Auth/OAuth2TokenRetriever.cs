using Newtonsoft.Json;
using Nightingale.Core.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Class for retrieving access tokens for OAuth 2.0.
    /// </summary>
    public class OAuth2TokenRetriever : IOAuth2TokenRetriever
    {
        public readonly HttpClient _client;

        public OAuth2TokenRetriever(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory?.GetClient()
                ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <inheritdoc/>
        public async Task<Token> RefreshAccessToken(
            string tokenUrl,
            string refreshToken,
            string scope = null,
            IDictionary<string, string> otherParameters = null)
        {
            if (string.IsNullOrWhiteSpace(tokenUrl) || !Uri.IsWellFormedUriString(tokenUrl, UriKind.Absolute))
                throw new ArgumentException("Token URL is invalid: " + tokenUrl);
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentNullException(nameof(refreshToken));

            // Ref: https://tools.ietf.org/html/rfc6749#section-6
            var formEncodedContent = new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
            };

            if (!string.IsNullOrWhiteSpace(scope)) 
                formEncodedContent.Add("scope", Uri.EscapeDataString(scope));

            if (otherParameters != null && otherParameters.Count > 0)
            {
                foreach (var pair in otherParameters) 
                    formEncodedContent.Add(pair.Key, Uri.EscapeDataString(pair.Value));
            }

            using (var msg = new HttpRequestMessage(HttpMethod.Post, tokenUrl))
            {
                msg.Content = new FormUrlEncodedContent(formEncodedContent);
                HttpResponseMessage response = await _client.SendAsync(msg);
                string json = await response.Content.ReadAsStringAsync();

                if (response == null || !response.IsSuccessStatusCode) 
                    throw new HttpRequestException(json);

                Token token = JsonConvert.DeserializeObject<Token>(json);
                return token;
            }
        }
    }
}
