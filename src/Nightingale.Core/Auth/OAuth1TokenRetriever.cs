using Nightingale.Core.Client.Authenticators;
using Nightingale.Core.Http;
using Nightingale.Core.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Nightingale.Core.Auth
{
    public class OAuth1TokenRetriever : IOAuth1TokenRetriever
    {
        private readonly HttpClient _client;
        private readonly IWebAuthBroker _webAuthBroker;

        public OAuth1TokenRetriever(
            IHttpClientFactory clientFactory,
            IWebAuthBroker webAuthBroker)
        {
            _client = clientFactory.GetClient()
                ?? throw new ArgumentNullException(nameof(clientFactory));
            _webAuthBroker = webAuthBroker
                ?? throw new ArgumentNullException(nameof(webAuthBroker));
        }

        /// <inheritdoc/>
        public async Task<string> GetAccessTokenResultAsync(
            string consumerKey,
            string consumerSecret, 
            string callbackUrl,
            string requestTokenUrl,
            string userAuthUrl, 
            string accessTokenUrl,
            ILogger logger)
        {
            try
            {
                var parameters = new string[]
                {
                    consumerKey,
                    consumerSecret,
                    callbackUrl,
                    requestTokenUrl,
                    userAuthUrl,
                    accessTokenUrl
                };

                Validate(parameters);
                var oauthToken = await GetRequestTokenAsync(consumerKey, consumerSecret, requestTokenUrl);
                logger.Log("Temporary request token: " + oauthToken);

                var userAuthResultQuery = await GetUserAuthResultAsync(userAuthUrl + "?oauth_token=" + oauthToken, callbackUrl);
                logger.Log("User authorization result: " + userAuthResultQuery);

                var accessTokenResult = await GetAccessTokenAsync(accessTokenUrl + userAuthResultQuery);
                logger.Log("Access Token result: " + accessTokenResult);

                return accessTokenResult;
            }
            catch (Exception e)
            {
                logger.Log(e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
            }

            return null;
        }

        public async Task<string> GetRequestTokenAsync(string consumerKey, string consumerSecret, string requestTokenUrl)
        {
            if (!Uri.IsWellFormedUriString(requestTokenUrl, UriKind.Absolute))
            {
                throw new ArgumentException("Request Token URL is either empty or invalid. " +
                    "Ensure it is a full URL, e.g. https://api.nightingale.ca/request_token");
            }

            Uri authUri = new Uri(requestTokenUrl);

            using (var message = new HttpRequestMessage(HttpMethod.Post, authUri))
            {
                string authHeader = new Oauth1Authenticator(
                    authUri.Host,
                    authUri.AbsolutePath,
                    consumerKey,
                    consumerSecret).GetRequestTokenAuthHeader(message.Method, null, null);
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", authHeader);
                var response = await _client.SendAsync(message);
                var responseString = await response.Content.ReadAsStringAsync();
                var queryString = HttpUtility.ParseQueryString(responseString);
                var oauthToken = queryString["oauth_token"];
                return oauthToken;
            }
        }

        public async Task<string> GetUserAuthResultAsync(string userAuthUrl, string callbackUrl)
        {
            if (!Uri.IsWellFormedUriString(userAuthUrl, UriKind.Absolute))
            {
                throw new ArgumentException("Authorization URL is either empty or invalid. " +
                    "Ensure it is a full URL, e.g. https://api.nightingale.ca/authorize");
            }
            if (!Uri.IsWellFormedUriString(callbackUrl, UriKind.Absolute))
            {
                throw new ArgumentException("Callback URL is either empty or invalid. " +
                    "Ensure it is a full URL, e.g. https://api.nightingale.ca/callback");
            }

            Uri callbackUri = new Uri(callbackUrl);
            Uri authUrl = new Uri(userAuthUrl);

            // launches WebView pop up.
            var result = await _webAuthBroker.GetAuthorizationResultAsync(authUrl.AbsoluteUri, callbackUri.AbsoluteUri);
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException($"User authorization failed. Auth URL = {authUrl.AbsoluteUri}. Callback URL = {callbackUri.AbsoluteUri}");
            }

            return new Uri(result).Query;
        }

        private async Task<string> GetAccessTokenAsync(string accessTokenUrl)
        {
            if (!Uri.IsWellFormedUriString(accessTokenUrl, UriKind.Absolute))
            {
                throw new ArgumentException("Access Token URL is either empty or invalid. " +
                    "Ensure it is a full URL, e.g. https://api.nightingale.ca/access_token");
            }
            Uri authUri = new Uri(accessTokenUrl);

            using (var message = new HttpRequestMessage(HttpMethod.Post, authUri))
            {
                var response = await _client.SendAsync(message);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
        }

        private void Validate(string[] parameters)
        {
            if (parameters.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                throw new ArgumentException("Missing required properties!");
            }
        }
    }
}
