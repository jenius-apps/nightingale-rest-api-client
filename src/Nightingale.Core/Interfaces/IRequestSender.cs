using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nightingale.Core.Interfaces
{
    /// <summary>
    /// Interface for sending requests
    /// and collections.
    /// </summary>
    public interface IRequestSender
    {
        /// <summary>
        /// Send HTTP request using given <see cref="Item"/>.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to send.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <param name="addToHistory">If true, adds request to the history list.</param>
        /// <returns>An <see cref="WorkspaceResponse"/> generated after sending the request.</returns>
        Task<WorkspaceResponse> SendRequestAsync(
            Item item,
            CancellationToken cancellationToken,
            bool addToHistory);

        /// <summary>
        /// Sends the requests of immediate children
        /// of the given item.
        /// </summary>
        /// <param name="item">The item whose children will be run.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if all runs were completed. False otherwise.</returns>
        Task<bool> RunCollectionAsync(
            Item item,
            CancellationToken cancellationToken);
    }
}
