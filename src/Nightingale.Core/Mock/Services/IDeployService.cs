using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System.Threading.Tasks;

namespace Nightingale.Core.Mock.Services
{
    /// <summary>
    /// Interface for service that deploys a mock server.
    /// </summary>
    public interface IDeployService
    {
        /// <summary>
        /// Deploys the mock server. Returns whether or
        /// not the deployment was successful
        /// </summary>
        Task<bool> DeployAsync(Item item = null);
    }
}
