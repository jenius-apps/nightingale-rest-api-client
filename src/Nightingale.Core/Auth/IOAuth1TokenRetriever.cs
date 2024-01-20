using Nightingale.Core.Interfaces;
using System.Threading.Tasks;

namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Interface for performing OAuth 1.0a
    /// authentication steps to retrieve
    /// the OAuth 1.0a access token.
    /// </summary>
    public interface IOAuth1TokenRetriever
    {
        /// <summary>
        /// Performs the 3 steps of OAuth 1.0a
        /// and returns the result of the final step
        /// when fetching the access token.
        /// </summary>
        /// <returns>Query string with "oauth_token" parameter in it.</returns>
        Task<string> GetAccessTokenResultAsync(
            string consumerKey,
            string consumerSecret,
            string callbackUrl,
            string requestTokenUrl,
            string userAuthUrl,
            string accessTokenUrl,
            ILogger logger);
    }
}