using Nightingale.Core.Mock.Models;
using Nightingale.Core.Models;

namespace Nightingale.Core.Mock.Services
{
    /// <summary>
    /// Interface for matching a request URL and method
    /// with the mock return value.
    /// </summary>
    public interface IRequestProcessor
    {
        /// <summary>
        /// Stores the appropriate collection data in memory
        /// to prepare for processing requests.
        /// </summary>
        /// <param name="config">The server configuration to use.</param>
        /// <param name="ncf">The NCF file containing the items to mock.</param>
        void Initialize(ServerConfiguration config, DocumentFile ncf);

        /// <summary>
        /// Retrieves the return value for the given path and method.
        /// </summary>
        MockData GetReturnValue(string path, string method);
    }
}
