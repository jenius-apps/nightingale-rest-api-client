using System.Threading.Tasks;

namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Interface for retrieving OAuth 2.0 implicit access 
    /// tokens.
    /// </summary>
    public interface IImplicitTokenRetriver
    {
        /// <summary>
        /// Pops a dialog to authenticate user
        /// and returns the access token.
        /// </summary>
        /// <param name="accessTokenUrl">The URL to request access tokens from.</param>
        /// <param name="clientId">The user's client ID.</param>
        /// <param name="callbackUrl">The user's callback URL.</param>
        /// <param name="scope">The user's scope parameter.</param>
        /// <param name="state">The users's state parameter.</param>
        /// <returns></returns>
        Task<string> GetImplicitAccessToken(
            string accessTokenUrl,
            string clientId,
            string callbackUrl,
            string scope,
            string state);
    }
}