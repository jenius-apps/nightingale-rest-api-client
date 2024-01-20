using Nightingale.Core.Models;

namespace Nightingale.Core.Workspaces.Services
{
    /// <summary>
    /// Interface for holding the current workspace.
    /// </summary>
    public interface ICurrentWorkspaceContainer
    {
        /// <summary>
        /// Updates the current workspace.
        /// </summary>
        void Set(Workspace w);

        /// <summary>
        /// Returns the current workspace.
        /// </summary>
        Workspace Get();
    }
}
