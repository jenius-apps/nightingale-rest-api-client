using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Core.Workspaces.Services
{
    public interface IWorkspaceListModifier
    {
        /// <summary>
        /// Gets the current workspace list reference.
        /// </summary>
        IList<Workspace> Current { get; }

        /// <summary>
        /// Updates the current workspace list
        /// reference.
        /// </summary>
        void SetList(IList<Workspace> list);

        /// <summary>
        /// Pops up UI for creating a new workspace, 
        /// and adds the workspace into workspace list.
        /// Returns workspace if successful, null otherwise. Performs
        /// limitation checks to see if this
        /// operation is allowed.
        /// </summary>
        /// <returns>Returns workspace if successful, null otherwise.</returns>
        Task<Workspace> NewWorkspaceAsync();

        /// <summary>
        /// Adds a workspace with the given name into
        /// the workspace list. Performs
        /// limitation checks to see if this
        /// operation is allowed.
        /// </summary>
        Task<Workspace> NewWorkspaceAsync(string name);

        /// <summary>
        /// Adds the given workspace into the
        /// workspace list. Performs limitation checks
        /// to see if this operation is allowed.
        /// Return true if successful.
        /// </summary>
        Task<bool> AddWorkspaceAsync(Workspace w);

        /// <summary>
        /// Deletes the given workspace. Pops dialog
        /// to confirm deletion request. Returns true
        /// if successful.
        /// </summary>
        Task<bool> DeleteWorkspaceAsync(Workspace forDeletion);

        /// <summary>
        /// Pops dialog to edit the workspace. Returns true if successful.
        /// </summary>
        Task<bool> EditWorkspaceAsync(Workspace w);
    }
}
