using Nightingale.Core.Auth;
using Nightingale.Core.Client.Authenticators;
using Nightingale.Core.Client.Builders;
using Nightingale.Core.Client.Ssl;
using Nightingale.Core.Cookies;
using Nightingale.Core.Extensions;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Logging;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Extensions;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nightingale.Core.Client
{
    /// <summary>
    /// Converts an <see cref="Item"/>
    /// into an <see cref="HttpRequestMessage"/> 
    /// and sends it using <see cref="HttpClient"/>.
    /// </summary>
    public class NightingaleClient : INightingaleClient
    {
        private readonly HttpClient _client;
        private readonly DigestAuthenticator _digestAuthenticator;
        private readonly IVariableResolver _varRes;
        private readonly ICookieJar _cookieJar;
        private readonly IBodyBuilder _bodyBuilder;
        private readonly IHeaderBuilder _headerBuilder;

        public NightingaleClient(
            IVariableResolver variableResolver,
            ICookieJar cookieJar,
            ISslValidator validator,
            IBodyBuilder bodyBuilder,
            IHeaderBuilder headerBuilder)
        {
            var handler = new HttpClientHandler
            {
                // Do not use the cookie container of 
                // http client since we're managing cookies ourselves.
                UseCookies = false,

                // Customize SSL validation to support user settings.
                ServerCertificateCustomValidationCallback = validator.Validate,

                // Ref: https://stackoverflow.com/a/6569042/10953422
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _client = new HttpClient(handler);
            _varRes = variableResolver
                ?? throw new ArgumentNullException(nameof(variableResolver));
            _cookieJar = cookieJar
                ?? throw new ArgumentNullException(nameof(cookieJar));
            _bodyBuilder = bodyBuilder
                ?? throw new ArgumentNullException(nameof(bodyBuilder));
            _headerBuilder = headerBuilder
                ?? throw new ArgumentNullException(nameof(headerBuilder));
            _digestAuthenticator = new DigestAuthenticator();
        }

        /// <inheritdoc/>
        public async Task<WorkspaceResponse> SendAsync(
            Item request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request?.Url?.ToString()))
            {
                throw new ArgumentNullException(nameof(request.Url), "Base URL cannot be null or empty.");
            }

            var logger = new LogBuilder();

            using (HttpRequestMessage message = await ToHttpMessage(request))
            {
                logger.Log(message.ToString());
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                HttpResponseMessage response = await _client.SendAsync(message, ct);
                stopwatch.Stop();
                logger.Log("");
                logger.Log(response.ToString());
                WorkspaceResponse result = await ToWorkspaceResponse(response, message, logger);
                result.TimeElapsed = stopwatch.ElapsedMilliseconds;
                return result;
            }
        }

        /// <summary>
        /// Converts <see cref="HttpResponseMessage"/>
        /// into <see cref="WorkspaceResponse"/>.
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponseMessage"/> to convert.</param>
        /// <param name="message">The <see cref="HttpRequestMessage"/> associated with the response.</param>
        /// <returns>Returns a <see cref="WorkspaceResponse"/>.</returns>
        private async Task<WorkspaceResponse> ToWorkspaceResponse(
            HttpResponseMessage httpResponse,
            HttpRequestMessage message,
            ILogger logger = null)
        {
            if (httpResponse == null)
            {
                return new WorkspaceResponse
                {
                    StatusCode = 0,
                    StatusDescription = "Null",
                    Body = "Null",
                    Successful = false,
                    RequestBaseUrl = message.RequestUri.AbsoluteUri
                };
            }

            string content = await httpResponse.Content.ReadAsStringAsync();
            byte[] bytes = await httpResponse.Content.ReadAsByteArrayAsync();

            var response = new WorkspaceResponse
            {
                StatusCode = (int)httpResponse.StatusCode,
                StatusDescription = httpResponse.ReasonPhrase,
                Body = content,
                RawBytes = bytes,
                Successful = httpResponse.IsSuccessStatusCode,
                ContentType = httpResponse.Content.Headers.ContentType?.MediaType,
                RequestBaseUrl = message.RequestUri.AbsoluteUri,
                Log = logger?.FlushLogs() ?? ""
            };

            // Extract cookies
            if (httpResponse.Headers.TryGetValues("Cookie", out var cookies))
            {
                foreach (var c in cookies)
                {
                    var split = c.Split('=');
                    if (split != null && split.Length == 2)
                    {
                        response.Cookies.Add(new KeyValuePair<string, string>(split[0], split[1]));
                    }
                }
            }

            // Extract headers
            foreach (var header in httpResponse.Headers.Concat(httpResponse.Content.Headers))
            {
                var name = header.Key;
                var value = string.Join(";", header.Value);
                response.Headers.Add(new KeyValuePair<string, string>(name, value));
            }

            return response;
        }

        /// <summary>
        /// Converts <see cref="Item"/>
        /// to <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="request">The <see cref="Item"/> to convert.</param>
        /// <returns>Returns a <see cref="HttpRequestMessage"/>.</returns>
        private async Task<HttpRequestMessage> ToHttpMessage(
            Item request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request), "Request can't be null when converting to HttpRequestMessage.");
            }

            var message = new HttpRequestMessage();

            // Set method
            message.Method = GetHttpMethod(request);

            // Set base url + query string
            _varRes.UpdateEnvironmentVariablesCache();
            string baseUrl = _varRes.ResolveVariable(request.Url.Base, useCache: true) ?? "";

            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new UriFormatException($"{baseUrl} doesn't seem to be a proper URI." +
                    $" Are you missing the http scheme?");
            }

            string query = GetQueryString(request);
            string uri = string.IsNullOrWhiteSpace(query) 
                ? baseUrl 
                : baseUrl + (baseUrl.EndsWith("?") || query.StartsWith("?") ? "" : "?") + query;

            message.RequestUri = new Uri(uri);

            // Set authentication
            SetAuth(request, message);

            // Set body content
            string contentType = _varRes.ResolveVariable(request.TryGetHeader("Content-Type"));
            await _bodyBuilder.SetBody(request.Body, message, null, contentType);

            // Set cookies
            var cookies = _cookieJar.GetCookieString(baseUrl);
            if (!string.IsNullOrWhiteSpace(cookies))
            {
                message.Headers.Add("Cookie", cookies);
            }

            // Set headers
            _headerBuilder.SetHeaders(request, message);

            return message;
        }

        /// <summary>
        /// Retrieves the method from the request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        /// <returns>An <see cref="HttpMethod"/>.</returns>
        private HttpMethod GetHttpMethod(Item request)
        {
            if (request == null)
            {
                return HttpMethod.Get;
            }

            if (string.IsNullOrWhiteSpace(request.Method))
            {
                throw new ArgumentNullException("Request.Method",
                    $"The method for request {request.Name} is null or empty. " +
                    $"Ensure that a method is selected!");
            }

            return new HttpMethod(request.Method);
        }

        /// <summary>
        /// Forms query string from active
        /// query parameters inside workspace request.
        /// Also resolves all variables. E.g. "?key1=value1&key2=value2"
        /// </summary>
        /// <param name="request">Workspace request to use.</param>
        /// <returns>Returns query string.</returns>
        private string GetQueryString(Item request)
        {
            if (request?.Url?.Queries == null)
            {
                return "";
            }

            var activeQueries = request.GetDistinctQueries(activeOnly: true);
            var queries = new List<string>();
            foreach (var activeQuery in activeQueries)
            {
                string key = _varRes.ResolveVariable(activeQuery.Key, useCache: true);
                string value = _varRes.ResolveVariable(activeQuery.Value, useCache: true);
                queries.Add($"{key}={value}");
            }

            return queries.Count == 0 ? "" : string.Join("&", queries);
        }

        private void SetAuth(
            Item request,
            HttpRequestMessage message)
        {
            if (request?.Auth == null || message == null)
            {
                return;
            }

            Authentication auth = request.GetAuthInheritance();
            if (auth is null)
            {
                return;
            }

            switch (auth.AuthType)
            {
                case AuthType.InheritParent:
                case AuthType.None:
                    break;
                case AuthType.Basic:
                    string username = _varRes.ResolveVariable(auth.GetProp(AuthConstants.BasicUsername), useCache: true);
                    string password = _varRes.ResolveVariable(auth.GetProp(AuthConstants.BasicPassword), useCache: true);
                    string cred = $"{username}:{password}";
                    byte[] credBytes = Encoding.ASCII.GetBytes(cred);
                    string base64 = Convert.ToBase64String(credBytes);
                    message.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64);
                    break;
                case AuthType.OAuth1:
                    string oauth1AccessToken = auth.GetProp(AuthConstants.OAuth1AccessToken);
                    string tokenSecret = auth.GetProp(AuthConstants.OAuth1TokenSecret);
                    if (string.IsNullOrWhiteSpace(oauth1AccessToken))
                    {
                        throw new ArgumentNullException(nameof(oauth1AccessToken),
                            "Access token must be provided. Ensure the access token has been fetched first.");
                    }
                    if (string.IsNullOrWhiteSpace(tokenSecret))
                    {
                        throw new ArgumentNullException(nameof(tokenSecret),
                            "Token secret must be provided. Ensure the access token has been fetched first.");
                    }

                    var authenticator = new Oauth1Authenticator(
                        message.RequestUri.Host,
                        message.RequestUri.AbsolutePath,
                        auth.GetProp(AuthConstants.OAuth1ConsumerKey),
                        auth.GetProp(AuthConstants.OAuth1ConsumerSecret));
                    var authHeaderParams = authenticator.GetAuthHeaderParameters(
                        message.Method,
                        new Uri(auth.GetProp(AuthConstants.OAuth1CallbackUrl)),
                        new string[] { request.Url.Queries.GetActive().ToList().ToQueryString().Trim('?') },
                        oauth1AccessToken,
                        tokenSecret);
                    message.Headers.Authorization = new AuthenticationHeaderValue("OAuth", authHeaderParams);
                    break;
                case AuthType.OAuth2:
                    string oauth2AccessToken = auth.GetProp(AuthConstants.OAuth2AccessToken);
                    if (string.IsNullOrWhiteSpace(oauth2AccessToken))
                    {
                        throw new ArgumentNullException(nameof(oauth2AccessToken),
                            "Access token must be provided. Ensure the access token has been fetched first.");
                    }
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", oauth2AccessToken);
                    break;
                case AuthType.Bearer:
                    string token = _varRes.ResolveVariable(auth.GetProp(AuthConstants.BearerToken), useCache: true);
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    break;
                case AuthType.Digest:
                    string digestUsername = _varRes.ResolveVariable(auth.GetProp(AuthConstants.DigestUsername), useCache: true);
                    string digestPassword = _varRes.ResolveVariable(auth.GetProp(AuthConstants.DigestPassword), useCache: true);
                    string digestAuthHeader = _digestAuthenticator.GetAuthHeader(
                        message.RequestUri,
                        digestUsername,
                        digestPassword);
                    message.Headers.Authorization = new AuthenticationHeaderValue("Digest", digestAuthHeader);
                    break;
                default:
                    throw new NotImplementedException("Unknown auth type: " + auth.AuthType.ToString());
            }
        }
    }
}
