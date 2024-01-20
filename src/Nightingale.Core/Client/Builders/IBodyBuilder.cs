using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nightingale.Core.Client.Builders
{
    /// <summary>
    /// Configures the body content in the http message.
    /// </summary>
    public interface IBodyBuilder
    {
        /// <summary>
        /// Uses <see cref="RequestBody"/> to configure
        /// the content of the http request message.
        /// </summary>
        /// <param name="body">The body to use.</param>
        /// <param name="message">The message to configure.</param>
        /// <param name="logger">Optional logger.</param>
        /// <param name="bodyContentType">Optional body content type that is assumed to be already variable resolved.</param>
        Task SetBody(
            RequestBody body,
            HttpRequestMessage message,
            ILogger logger = null,
            string bodyContentType = null);
    }
}
