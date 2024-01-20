using Nightingale.Core.Models;

namespace Nightingale.Core.Workspaces.Factories
{
    /// <summary>
    /// Interface for generating workspace objects.
    /// </summary>
    public interface IWorkspaceFactory
    {
        /// <summary>
        /// Generates a new workspace object.
        /// </summary>
        /// <param name="name">Name of workspace.</param>
        /// <returns>Returns workspace item.</returns>
        Workspace Create(string name);
    }
}
