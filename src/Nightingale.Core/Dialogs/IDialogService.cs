using Nightingale.Core.Export;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Dialogs
{
    /// <summary>
    /// Interface for all available dialog
    /// service in Nightingale.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Pops a settings dialog.
        /// </summary>
        /// <returns></returns>
        Task OpenSettingsAsync();

        /// <summary>
        /// Display pop up for mock server
        /// with preview of what mock data will be used.
        /// </summary>
        /// <param name="config">Server configuration that will be used.</param>
        /// <returns>True if user confirms. False if user cancels.</returns>
        Task<bool> MockServerDialogAsync(
            string workspaceName,
            string itemName,
            string envName);

        /// <summary>
        /// Pops dialog for creating a new item.
        /// </summary>
        /// <param name="type">The type of item.</param>
        /// <param name="rootItems">Optional. The list of items in the root
        /// which is used to let user select the parent of the new item. A null list
        /// will hide the parent selector UI element.</param>
        /// <param name="currentParent">Optional. The contextual item parent the user is currently in.
        /// Used to prepopulate the parent selection list.</param>
        /// <param name="prepopulatedName">Name to display in dialog.</param>
        /// <returns>True if user confirmed. String for item name. Item for selected parent (null if root is desired).</returns>
        Task<(bool, string, Item)> NewItemDialogAsync(
            ItemType type,
            IList<Item> rootItems = null,
            Item currentParent = null,
            string prepopulatedName = null);

        /// <summary>
        /// Pops dialog for editing an item's name.
        /// Returns true if confirmed, and 
        /// string for the new name of the item.
        /// </summary>
        Task<(bool, string)> EditItemDialogAsync(string itemName);

        /// <summary>
        /// Pops display to confirm deletion if user setting
        /// is to confirm before deletion. 
        /// Returns true if confirmed. 
        /// </summary>
        /// <param name="forceConfirm">Optional, default false. If true,
        /// the dialog will be popped even if the user setting is to
        /// delete without asking.</param>
        Task<bool> ConfirmDeleteAsync(bool forceConfirm = false);

        /// <summary>
        /// Pops display to confirm deleting all.
        /// Return true if confirmed.
        /// </summary>
        Task<bool> ConfirmDeleteAllAsync();

        /// <summary>
        /// Pops dialog to obtain workspace name. Returns true if confirmed, and string for the name.
        /// </summary>
        /// <param name="prepopulatedName">Optional. Used to prepopulate the name textbox.</param>
        /// <returns>Returns true if confirmed, and string for the name.</returns>
        Task<(bool, string)> WorkspaceDialogAsync(string prepopulatedName = null);

        /// <summary>
        /// Pops dialog to perform import file operations.
        /// Returns items that will be imported.
        /// </summary>
        Task<(IList<Item>, IList<Workspace>)> ImportDialogAsync();

        /// <summary>
        /// Pops the export dialog to select
        /// what to export. Returns list of workspaces to export.
        /// </summary>
        /// <param name="workspaces">List of workspaces to choose from.</param>
        /// <returns></returns>
        Task<ExportConfiguration> ExportDialogAsync(IList<Workspace> workspaces);

        /// <summary>
        /// Pops dialog to confirm if customer
        /// wants to overwrite the given parameter
        /// variable in the environment.
        /// </summary>
        /// <param name="p">The pre-existing parameter that will be overwritten.</param>
        /// <returns>True if customer wants to overwrite. False otherwise.</returns>
        Task<bool> ConfirmParameterOverwriteAsync(Parameter p);

        /// <summary>
        /// Pops onboarding tutorial dialog.
        /// </summary>
        Task TutorialAsync();
        Task PremiumDialogAsync();
    }
}
