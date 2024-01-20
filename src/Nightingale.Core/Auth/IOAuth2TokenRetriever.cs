using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Interface for retrieving access tokens for OAuth 2.0.
    /// </summary>
    public interface IOAuth2TokenRetriever
    {
        /// <summary>
        /// Refreshes token based on given parameters.
        /// </summary>
        /// <param name="tokenUrl">The token URL to use.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="scope">Optional. The scope parameter for the refresh token.</param>
        /// <param name="otherParameters">Other parameters to include in the body of the request.</param>
        /// <returns>A refreshed <see cref="Token"/>.</returns>
        Task<Token> RefreshAccessToken(
            string tokenUrl,
            string refreshToken,
            string scope = null,
            IDictionary<string, string> otherParameters = null);
    }
}