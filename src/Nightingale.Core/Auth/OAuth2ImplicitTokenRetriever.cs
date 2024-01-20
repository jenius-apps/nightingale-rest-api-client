using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Class for retrieving an OAuth 2.0 implicit token.
    /// </summary>
    public class OAuth2ImplicitTokenRetriever : IImplicitTokenRetriver
    {
        private const string AccessTokenKey = "access_token";
        private readonly IWebAuthBroker _webAuthBroker;

        public OAuth2ImplicitTokenRetriever(
            IWebAuthBroker webAuthBroker)
        {
            _webAuthBroker = webAuthBroker 
                ?? throw new ArgumentNullException(nameof(webAuthBroker));
        }

        /// <inheritdoc/>
        public async Task<string> GetImplicitAccessToken(
            string accessTokenUrl,
            string clientId,
            string callbackUrl,
            string scope,
            string state)
        {
            // Ensure URLs are valid
            if (!Uri.IsWellFormedUriString(callbackUrl, UriKind.Absolute))
            {
                throw new UriFormatException("A valid callback is required for this operation");
            }
            if (!Uri.IsWellFormedUriString(accessTokenUrl, UriKind.Absolute))
            {
                throw new UriFormatException("A valid access token URL is required for this operation");
            }

            // Query should look like this:
            // /authorize?response_type=token&client_id=s6BhdRkqt3&state=xyz
            // &redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Fcb
            // Ref: https://tools.ietf.org/html/rfc6749#section-4.2.1
            var queries = new List<string>
            {
                "response_type=token",
                "client_id=" + Uri.EscapeDataString(clientId),
                "redirect_uri=" + Uri.EscapeDataString(callbackUrl),
            };
            if (!string.IsNullOrWhiteSpace(scope))
            {
                queries.Add("scope=" + Uri.EscapeDataString(scope));
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                queries.Add("state=" + Uri.EscapeDataString(state));
            }

            // Build request and trigger UI pop up broker.
            var url = $"{accessTokenUrl}?{string.Join("&", queries)}";
            string result = await _webAuthBroker.GetAuthorizationResultAsync(url, callbackUrl);
            if (string.IsNullOrWhiteSpace(result) || !Uri.IsWellFormedUriString(result, UriKind.Absolute))
            {
                throw new Exception("The return value after user authentication was empty or not a well formed URL.");
            }

            // The access code is in the #fragment of the returned URL.
            string fragment = (new Uri(result)).Fragment?.Trim('#');
            if (string.IsNullOrWhiteSpace(fragment) || !fragment.Contains(AccessTokenKey))
            {
                throw new Exception("Could not find access_token parameter in returned authentication URL: " + result);
            }

            var fragments = HttpUtility.ParseQueryString(fragment);
            return fragments[AccessTokenKey];
        }
    }
}
