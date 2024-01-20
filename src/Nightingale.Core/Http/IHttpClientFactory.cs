using System.Net.Http;

namespace Nightingale.Core.Http
{
    /// <summary>
    /// Factory for HttpClient.
    /// </summary>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Returns an HttpClient.
        /// </summary>
        HttpClient GetClient();
    }
}
