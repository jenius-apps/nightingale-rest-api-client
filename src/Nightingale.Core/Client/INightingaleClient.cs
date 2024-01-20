using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nightingale.Core.Client
{
    /// <summary>
    /// Interface for Nightingale's customized
    /// http client.
    /// </summary>
    public interface INightingaleClient
    {
        /// <summary>
        /// Sends request and returns response.
        /// </summary>
        /// <returns>WorkspaceResponse if successful.</returns>
        Task<WorkspaceResponse> SendAsync(
            Item request,
            CancellationToken ct);
    }
}
