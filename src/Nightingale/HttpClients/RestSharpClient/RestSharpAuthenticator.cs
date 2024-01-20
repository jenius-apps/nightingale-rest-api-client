using Newtonsoft.Json;
using Nightingale.Core.Auth;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Nightingale.HttpClients.RestSharpClient
{
    public class RestSharpAuthenticator : Core.Interfaces.IOAuth2TokenRetriever
    {
        private readonly IVariableResolver _variableResolver;
        private readonly IImplicitTokenRetriver _implicitTokenRetriver;

        public RestSharpAuthenticator(
            IImplicitTokenRetriver implicitTokenRetriver,
            IVariableResolver variableResolver)
        {
            _variableResolver = variableResolver
                ?? throw new ArgumentNullException(nameof(variableResolver));
            _implicitTokenRetriver = implicitTokenRetriver
                ?? throw new ArgumentNullException(nameof(implicitTokenRetriver));
        }

        public async Task<Token> GetOAuth2Token(Authentication auth, ILogger logger, bool force = true)
        {
            if (auth == null)
            {
                throw new ArgumentNullException("Oauth2 data cannot be null when retrieving OAuth2 token.", nameof(auth));
            }

            Authentication oauth2Data = _variableResolver.ResolveAllVariables(auth);
            string accessToken = oauth2Data.GetProp(AuthConstants.OAuth2AccessToken);
            GrantType grantType = oauth2Data.GetEnumProp<GrantType>(AuthConstants.OAuth2GrantType);
            string scope = oauth2Data.GetProp(AuthConstants.OAuth2Scope);
            string accessTokenUrl = oauth2Data.GetProp(AuthConstants.OAuth2AccessTokenUrl);
            string clientId = oauth2Data.GetProp(AuthConstants.OAuth2ClientId);
            string clientSecret = oauth2Data.GetProp(AuthConstants.OAuth2ClientSecret);
            string callbackUrl = oauth2Data.GetProp(AuthConstants.OAuth2CallbackUrl);
            string authorizeUrl = oauth2Data.GetProp(AuthConstants.OAuth2AuthUrl);
            string state = oauth2Data.GetProp(AuthConstants.OAuth2State);

            Token resultingToken = null;

            if (grantType == GrantType.client_credentials)
            {
                string extras = !string.IsNullOrWhiteSpace(scope) ? $"&scope={scope}" : "";

                resultingToken = await GetAccessToken(
                    logger,
                    accessTokenUrl,
                    grantType.ToString(),
                    clientId,
                    clientSecret,
                    extras);
            }
            else if (grantType == GrantType.authorization_code)
            {
                if (!Uri.IsWellFormedUriString(callbackUrl, UriKind.Absolute))
                {
                    throw new UriFormatException("A valid callback URL is required for this operation.");
                }

                string authCode = await GetAuthCode(
                    logger,
                    authorizeUrl,
                    clientId,
                    callbackUrl,
                    scope);

                resultingToken = await GetAccessToken(
                    logger,
                    accessTokenUrl,
                    grantType.ToString(),
                    clientId,
                    clientSecret,
                    $"&code={Uri.EscapeDataString(authCode ?? string.Empty)}&redirect_uri={Uri.EscapeDataString(callbackUrl)}");
            }
            else if (grantType == GrantType.implicit_flow)
            {
                var token = await _implicitTokenRetriver.GetImplicitAccessToken(
                    accessTokenUrl,
                    clientId,
                    callbackUrl,
                    scope,
                    state);

                resultingToken = new Token
                {
                    AccessToken = token
                };
            }

            return resultingToken;
        }

        private async Task<Token> GetAccessToken(ILogger logger, string accessTokenUrl, string grantType, string clientId, string clientSecret, string extras = "")
        {
            if (string.IsNullOrWhiteSpace(accessTokenUrl)
                || string.IsNullOrWhiteSpace(grantType)
                || string.IsNullOrWhiteSpace(clientId))
            {
                return null;
            }

            string formParam = $"grant_type={Uri.EscapeDataString(grantType ?? string.Empty)}&client_id={Uri.EscapeDataString(clientId ?? string.Empty)}{extras}";

            if (!string.IsNullOrWhiteSpace(clientSecret))
                formParam += $"&client_secret={Uri.EscapeDataString(clientSecret ?? string.Empty)}";

            var client = new RestClient(accessTokenUrl);
            var request = new RestRequest();
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", formParam, ParameterType.RequestBody);

            logger.Log("\nSending access token request.");
            logger.Log("Parameters: " + formParam);

            try
            {
                Token tok = await client.PostAsync<Token>(request);
                return tok;
            }
            catch (Exception e)
            {
                throw new Exception("The authentication response could not be deserialized into a proper token. " + e.Message);
            }
        }

        private async Task<string> GetAuthCode(ILogger logger, string authUrl, string clientId, string callbackUrl, string scope)
        {
            if (string.IsNullOrWhiteSpace(authUrl)
               || string.IsNullOrWhiteSpace(clientId)
               || string.IsNullOrWhiteSpace(callbackUrl))
            {
                return string.Empty;
            }

            var queries = new List<string>()
            {
                { "client_id=" + Uri.EscapeDataString(clientId) },
                { "redirect_uri=" + Uri.EscapeDataString(callbackUrl) },
                { "response_type=code" }
            };
            if (!string.IsNullOrWhiteSpace(scope))
            {
                queries.Add("scope=" + Uri.EscapeDataString(scope));
            }

            // User might have added params into the authurl so need to append to it property
            var tempUri = new Uri(authUrl);
            if (string.IsNullOrWhiteSpace(tempUri.Query))
            {
                authUrl += "?" + string.Join("&", queries);
            }
            else if (tempUri.Query == "?")
            {
                authUrl += string.Join("&", queries);
            }
            else
            {
                authUrl += "&" + string.Join("&", queries);
            }

            logger.Log("\nStarting authorization.");
            logger.Log(authUrl);
            WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None, new Uri(authUrl), new Uri(callbackUrl));

            if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                throw new Exception("User canceled. Here's the authorization token URL we used in case you need it: " + authUrl);
            }
            else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                throw new Exception("Failed to get an authorization code. Response data: " + WebAuthenticationResult.ResponseData);
            }

            logger.Log("\nResponse content: " + WebAuthenticationResult.ResponseData);

            string[] keyVal = WebAuthenticationResult.ResponseData.Split('?')[1].Split('&');
            string codeParam = "";
            foreach (var s in keyVal)
            {
                if (s.StartsWith("code"))
                    codeParam = s;
            }

            if (string.IsNullOrWhiteSpace(codeParam))
            {
                throw new Exception("Failed to find an authorization code in response data: " + WebAuthenticationResult.ResponseData);
            }

            return codeParam.Split('=')[1];
        }
    }
}
