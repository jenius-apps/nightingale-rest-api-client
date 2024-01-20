using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System.Threading.Tasks;

namespace Nightingale.Core.Workspaces.Services
{
    /// <summary>
    /// Interface for a class that modifies the
    /// workspace tree.
    /// </summary>
    public interface IWorkspaceTreeModifier
    {
        /// <summary>
        /// A reference to the current workspace.
        /// Must be manually updated when workspace changes.
        /// </summary>
        Workspace Current { get; }

        /// <summary>
        /// Sets the current workspace which will
        /// be modified.
        /// </summary>
        void SetWorkspace(Workspace w);

        /// <summary>
        /// Pops a UI dialog to add
        /// a new item and provides
        /// list of possible parents.
        /// Returns the created item if successful.
        /// </summary>
        Task<Item> NewItemAsync(
            ItemType type,
            bool showCollectionSelector,
            Item currentParent = null);

        /// <summary>
        /// Pops a UI dialog for adding
        /// an existing item into the workspace. Provides
        /// list of possible parents. Returns the item if
        /// successful.
        /// </summary>
        /// <param name="item">The existing item that will be inserted into workspace tree.</param>
        /// <returns>The item after it was inserted.</returns>
        Task<Item> InsertToWorkspaceAsync(Item item);

        /// <summary>
        /// Adds the given item to the
        /// current workspace. Returns true if successful.
        /// Performs limitation check to see if allowed.
        /// </summary>
        Task<bool> AddItemAsync(Item item, Item parent = null, int? insertPosition = null);

        /// <summary>
        /// Deletes an item from the current
        /// workspace.
        /// </summary>
        Task<bool> DeleteItemAsync(Item item);

        /// <summary>
        /// Deletes all items from the current
        /// workspace.
        /// </summary>
        Task<bool> DeleteAllItemsAsync();

        /// <summary>
        /// Clones the given item. If item has
        /// a parent, the clone will have the same
        /// parent.
        /// </summary>
        Task<Item> CloneItemAsync(Item item);

        /// <summary>
        /// Pops a dialog to edit the given item.
        /// </summary>
        Task<bool> EditItemAsync(Item item);
    }
}
