using Nightingale.Core.Auth;
using System;
using System.Threading.Tasks;

namespace Nightingale.Auth
{
    public class WebAuthBroker : IWebAuthBroker
    {
        public bool DialogOpen = false;

        /// <inheritdoc/>
        public async Task<string> GetAuthorizationResultAsync(string url, string redirectUrl, object modalBase = null)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)
                || !Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute)
                || DialogOpen)
            {
                return null;
            }

            DialogOpen = true;

            var msg = new WebAuthDialog
            {
                AuthUri = new Uri(url),
                CallbackUri = new Uri(redirectUrl)
            };
            await msg.ShowAsync();

            if (string.IsNullOrWhiteSpace(msg.ReturnUrl)
                || !Uri.IsWellFormedUriString(msg.ReturnUrl, UriKind.Absolute))
            {
                DialogOpen = false;
                return null;
            }

            DialogOpen = false;
            return msg.ReturnUrl;
        }
    }
}
