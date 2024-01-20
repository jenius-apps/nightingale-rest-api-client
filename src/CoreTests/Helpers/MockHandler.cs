using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreTests.Helpers
{
    /// <summary>
    /// Class for mocking an HttpClientHandler
    /// so that an HttpClient can return a specific response.
    /// </summary>
    internal class MockHandler : HttpClientHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly HttpContent _content;

        public MockHandler(HttpStatusCode statusCode, HttpContent content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = _content
            };

            return Task.FromResult(response);
        }
    }
}
