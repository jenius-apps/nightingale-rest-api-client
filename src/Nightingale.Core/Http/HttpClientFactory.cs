using System.Net.Http;

namespace Nightingale.Core.Http
{
    /// <summary>
    /// Class for creating HttpClient.
    /// </summary>
    public class HttpClientFactory : IHttpClientFactory
    {
        private HttpClient _client;

        /// <inheritdoc/>
        public HttpClient GetClient()
        {
            if (_client is null)
            {
                _client = new HttpClient();
            }

            return _client;
        }
    }
}
