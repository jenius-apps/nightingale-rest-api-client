using Nightingale.Core.Interfaces;
using Nightingale.Core.Workspaces.Models;
using System.Net.Http;

namespace Nightingale.Core.Client.Builders
{
    /// <summary>
    /// Configures the headers in the http message.
    /// </summary>
    public interface IHeaderBuilder
    {
        /// <summary>
        /// Sets the headers in the http request message using the
        /// headers inside of the request item.
        /// </summary>
        void SetHeaders(Item request, HttpRequestMessage message, ILogger logger = null);
    }
}
