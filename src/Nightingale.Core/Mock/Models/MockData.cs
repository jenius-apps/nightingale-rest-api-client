using Nightingale.Core.Models;

namespace Nightingale.Core.Mock.Models
{
    /// <summary>
    /// Data used for a mock server.
    /// </summary>
    public class MockData : ObservableBase
    {
        /// <summary>
        /// Body to return when using a mock server.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Status to return when using a mock server.
        /// Default 200.
        /// </summary>
        public int? StatusCode { get; set; } = 200;

        /// <summary>
        /// Content type to return when using a mock server.
        /// </summary>
        public string ContentType { get; set; } = "application/json";
    }
}
