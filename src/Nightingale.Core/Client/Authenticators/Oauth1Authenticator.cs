using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Nightingale.Core.Client.Authenticators
{
    /// <summary>
    /// Class for generating an OAuth 1.0a
    /// authentication header.
    /// </summary>
    /// <remarks>
    /// Ref: http://commandlinefanatic.com/cgi-bin/showarticle.cgi?article=art014
    /// </remarks>
    public class Oauth1Authenticator
    {
        private readonly string _host;
        private readonly string _path;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public Oauth1Authenticator(
            string host,
            string path,
            string consumerKey,
            string consumerSecret)
        {
            _host = host;
            _path = path;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        /// <summary>
        /// Generates an OAuth 1.0a header for 
        /// requesting a temporary "request token"
        /// from your service's request token endpoint.
        /// This is step 1 of the OAuth process.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="callbackUri"></param>
        /// <param name="additionalParameters">List of key value pairs formatted as key1=value1 where key and value are URL encoded.</param>
        /// <returns></returns>
        public string GetRequestTokenAuthHeader(
            HttpMethod method,
            Uri callbackUri,
            IList<string> additionalParameters)
        {
            return GetAuthHeaderParameters(method, callbackUri, additionalParameters);
        }

        /// <summary>
        /// Use the generated header to make a request
        /// to the "access token" endpoint. This exchanges
        /// the temporary request token for a long-lasting
        /// access token. This is step 2 of the OAuth process.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="callbackUri"></param>
        /// <param name="additionalParameters"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="verifier"></param>
        /// <returns></returns>
        public string GetAccessTokenAuthHeader(
            HttpMethod method,
            Uri callbackUri,
            IList<string> additionalParameters,
            string token,
            string tokenSecret,
            string verifier)
        {
            return GetAuthHeaderParameters(method, callbackUri, additionalParameters, token, tokenSecret, verifier);
        }

        /// <summary>
        /// Generic method for generating an OAuth 1.0a header.
        /// Supply a long-lasting access token and its matching token secret
        /// to create an OAuth header that can be used for accessing
        /// real data. This is step 3 (and final step) of the OAuth process.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="callbackUri"></param>
        /// <param name="additionalParameters">
        /// Parameters from the URL query formatted like key1=value1,
        /// where the key and value are URL encoded.
        /// </param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="verifier"></param>
        /// <returns></returns>
        public string GetAuthHeaderParameters(
            HttpMethod method,
            Uri callbackUri,
            IList<string> additionalParameters,
            string token = null,
            string tokenSecret = null,
            string verifier = null)
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string nonce = timestamp.ToString();

            var parameters = new List<string>()
            {
                "oauth_consumer_key=" + _consumerKey,
                "oauth_nonce=" + nonce,
                "oauth_signature_method=HMAC-SHA1",
                "oauth_timestamp=" + timestamp,
                "oauth_version=1.0",
            };
            if (additionalParameters != null && additionalParameters.Count > 0)
            {
                parameters.AddRange(additionalParameters);
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                parameters.Add("oauth_token=" + token);
            }
            if (!string.IsNullOrWhiteSpace(verifier))
            {
                parameters.Add("oauth_verifier=" + verifier);
            }
            if (callbackUri != null)
            {
                parameters.Add("oauth_callback=" + Uri.EscapeDataString(callbackUri.AbsoluteUri));
            }

            parameters.Sort();
            string query = Uri.EscapeDataString(string.Join("&", parameters));
            string hostPath = Uri.EscapeDataString("https://" + _host + _path);
            string signatureString = $"{method.Method}&{hostPath}&{query}";

            // Perform HMAC-SHA1 signing
            string signature;

            var sha1Key = _consumerSecret + "&" + (string.IsNullOrWhiteSpace(tokenSecret) ? "" : tokenSecret);
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(sha1Key)))
            {
                hmac.Initialize();
                byte[] buffer = Encoding.ASCII.GetBytes(signatureString);
                byte[] rawHash = hmac.ComputeHash(buffer);
                signature = Convert.ToBase64String(rawHash);
            }

            var headerParameters = new List<string>
            {
                $"oauth_consumer_key=\"{_consumerKey}\"",
                $"oauth_nonce=\"{nonce}\"",
                $"oauth_timestamp=\"{timestamp}\"",
                $"oauth_signature_method=\"HMAC-SHA1\"",
                $"oauth_signature=\"{Uri.EscapeDataString(signature)}\"",
                $"oauth_version=\"1.0\""
            };
            if (!string.IsNullOrWhiteSpace(token))
            {
                headerParameters.Add($"oauth_token=\"{token}\"");
            }
            if (!string.IsNullOrWhiteSpace(verifier))
            {
                headerParameters.Add($"oauth_verifier=\"{verifier}\"");
            }
            if (callbackUri != null)
            {
                headerParameters.Add($"oauth_callback=\"{Uri.EscapeDataString(callbackUri.AbsoluteUri)}\"");
            }

            var result = string.Join(", ", headerParameters);
            return result;
        }
    }
}
