using System.Threading.Tasks;

namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Interface for producing a UI pop up
    /// to authenticate a user.
    /// </summary>
    public interface IWebAuthBroker
    {
        /// <summary>
        /// Pops open a web view dialog and attempts to authenticate
        /// the user.
        /// </summary>
        /// <param name="url">The authorization URL including any query paremeters.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="modalBase">Optional. Some platforms like Xamarin require a reference to the page that triggers the dialog.</param>
        /// <returns>The string result of the authorization. E.g. "https://{callback}?authorizationcode=xxxxx"</returns>
        Task<string> GetAuthorizationResultAsync(string url, string redirectUrl, object modalBase = null);
    }
}
