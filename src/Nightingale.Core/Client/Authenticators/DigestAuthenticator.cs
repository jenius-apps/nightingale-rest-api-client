using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Nightingale.Core.Client.Authenticators
{
    /// <summary>
    /// Class for creating a Digest
    /// Authorization header.
    /// </summary>
    /// <remarks>
    /// Adapted from https://hifni.blogspot.com/2017/05/digest-authentication-with-restsharp.html?m=1
    /// </remarks>
    public class DigestAuthenticator
    {
        private string _host;
        private string _user;
        private string _password;
        private string _realm;
        private string _nonce;
        private string _qop;
        private string _cnonce;
        private string _opaque;
        private DateTime _cnonceDate;
        private int _nc;

        /// <summary>
        /// Retrieves details from the server necessary for 
        /// digest authentication and builds the digest authentication
        /// header. Does not include the "Digest" prefix.
        /// </summary>
        /// <param name="fullRequestUri">The Uri of the request the client wants to make to the server.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The digest authentication string to be added as the value in the authorization header.
        /// </returns>
        public string GetAuthHeader(Uri fullRequestUri, string username, string password)
        {
            if (fullRequestUri.Host != _host || username != _user || password != _password)
            {
                Reset();
                _host = fullRequestUri.Host;
                _user = username;
                _password = password;
            }

            // Performs first auth attempt to gather necessary auth info.
            GetResponse(fullRequestUri);

            return GetDigestHeader(fullRequestUri.PathAndQuery);
        }
        
        private void Reset()
        {
            _host = "";
            _user = "";
            _password = "";
            _realm = "";
            _nonce = "";
            _qop = "";
            _cnonce = "";
            _opaque = "";
            _cnonceDate = new DateTime();
            _nc = 0;
        }

        /// <summary>
        /// Digest auth sometimes requires two attempts to
        /// call the desired endpoint. The first attempt is
        /// to retrieve details back from the server that is
        /// needed to fully authenticate. This method
        /// performs that first attempt to gather the details.
        /// </summary>
        /// <param name="fullRequestUri">The request URI.</param>
        private void GetResponse(Uri fullRequestUri)
        {
            var request = (HttpWebRequest)WebRequest.Create(fullRequestUri);

            // If we've got a recent Auth header, re-use it!
            if (!string.IsNullOrEmpty(_cnonce) && DateTime.Now.Subtract(_cnonceDate).TotalHours < 1.0)
            {
                request.Headers.Add("Authorization", $"Digest {GetDigestHeader(fullRequestUri.PathAndQuery)}");
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                // Try to fix a 401 exception by adding a Authorization header
                if (ex.Response == null || ((HttpWebResponse)ex.Response).StatusCode != HttpStatusCode.Unauthorized)
                    throw;

                var wwwAuthenticateHeader = ex.Response.Headers["WWW-Authenticate"];
                _realm = GetHeaderVar("realm", wwwAuthenticateHeader);
                _nonce = GetHeaderVar("nonce", wwwAuthenticateHeader);
                _qop = GetHeaderVar("qop", wwwAuthenticateHeader);

                _nc = 0;
                _opaque = GetHeaderVar("opaque", wwwAuthenticateHeader);
                _cnonce = new Random().Next(123400, 9999999).ToString(CultureInfo.InvariantCulture);
                _cnonceDate = DateTime.Now;
            }

        }

        /// <summary>
        /// Builds the digest authe header from
        /// the data stored in private variables.
        /// </summary>
        /// <param name="pathAndQuery"></param>
        /// <returns>The digest header string, without the "Digest" prefix.</returns>
        private string GetDigestHeader(string pathAndQuery)
        {
            _nc++;

            var ha1 = CalculateMd5Hash(string.Format("{0}:{1}:{2}", _user, _realm, _password));
            var ha2 = CalculateMd5Hash(string.Format("{0}:{1}", "GET", pathAndQuery));
            var digestResponse = CalculateMd5Hash(string.Format("{0}:{1}:{2:00000000}:{3}:{4}:{5}", ha1, _nonce, _nc, _cnonce, _qop, ha2));

            var quotedParameters = new Dictionary<string, string>()
            {
                { "username", _user },
                { "realm", _realm },
                { "nonce", _nonce },
                { "uri", pathAndQuery },
                { "response", digestResponse },
                { "opaque", _opaque},
                { "cnonce", _cnonce }
            };

            var unquotedParameters = new Dictionary<string, string>()
            {
                { "algorithm","MD5" },
                { "qop", _qop },
                { "nc", _nc.ToString("6:00000000") }
            };

            var headerParams = new List<string>();
            foreach (var pair in quotedParameters)
            {
                if (!string.IsNullOrWhiteSpace(pair.Value))
                {
                    headerParams.Add($"{pair.Key}=\"{pair.Value}\"");
                }
            }

            foreach (var pair in unquotedParameters)
            {
                if (!string.IsNullOrWhiteSpace(pair.Value))
                {
                    headerParams.Add($"{pair.Key}={pair.Value}");
                }
            }

            return string.Join(", ", headerParams);

            //return string.Format("username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", " +
            //    "algorithm=MD5, response=\"{4}\", opaque=\"{8}\", qop={5}, nc={6:00000000}, cnonce=\"{7}\"",
            //    _user, _realm, _nonce, pathAndQuery, digestResponse, _qop, _nc, _cnonce, _opaque);
        }

        /// <summary>
        /// Extracts the value for the given variable name
        /// from within the given header.
        /// </summary>
        /// <param name="varName">Key name of variable to look for.</param>
        /// <param name="header">The header to search through.</param>
        /// <returns>The value of the given key.</returns>
        private string GetHeaderVar(string varName, string header)
        {
            var regHeader = new Regex(string.Format(@"{0}=""([^""]*)""", varName));
            var matchHeader = regHeader.Match(header);

            if (matchHeader.Success)
            {
                return matchHeader.Groups[1].Value;
            }

            return "";
            //throw new ApplicationException(string.Format("Header {0} not found", varName));
        }

        /// <summary>
        /// Calculates MD5 hash as required by digest auth.
        /// </summary>
        /// <param name="input">String to hash. Generally, this is a concatenation of digest auth parameters.</param>
        /// <returns>The hashed string.</returns>
        private string CalculateMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();

                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
