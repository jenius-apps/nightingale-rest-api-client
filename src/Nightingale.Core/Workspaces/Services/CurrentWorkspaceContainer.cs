using Nightingale.Core.Models;

namespace Nightingale.Core.Workspaces.Services
{
    /// <summary>
    /// Class for holding the current workspace.
    /// </summary>
    public class CurrentWorkspaceContainer : ICurrentWorkspaceContainer
    {
        private Workspace _current;

        /// <inheritdoc/>
        public Workspace Get()
        {
            return _current;
        }

        /// <inheritdoc/>
        public void Set(Workspace w)
        {
            _current = w;
        }
    }
}
