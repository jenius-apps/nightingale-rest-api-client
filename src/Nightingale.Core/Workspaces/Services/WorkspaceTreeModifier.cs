using Nightingale.Core.Dialogs;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Factories;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Threading.Tasks;

namespace Nightingale.Core.Workspaces.Services
{
    /// <summary>
    /// Class for modifying the workspace's list of
    /// <see cref="Item"/>.
    /// </summary>
    public class WorkspaceTreeModifier : IWorkspaceTreeModifier
    {
        private readonly IDialogService _dialogService;
        private readonly IItemFactory _itemFactory;

        public WorkspaceTreeModifier(
            IDialogService dialogService,
            IItemFactory itemFactory)
        {
            _dialogService = dialogService ??
                throw new ArgumentNullException(nameof(dialogService));
            _itemFactory = itemFactory ??
                throw new ArgumentNullException(nameof(itemFactory));
        }

        /// <inheritdoc/>
        public Workspace Current { get; private set; }

        /// <inheritdoc/>
        public Task<bool> AddItemAsync(Item item, Item parent = null, int? insertPosition = null)
        {
            if (item is null || Current == null)
            {
                return Task.FromResult(false);
            }

            var destinationList = parent != null ? parent.Children : Current.Items;

            if (insertPosition == null 
                || insertPosition < 0 
                || insertPosition >= destinationList.Count)
            {
                destinationList.Add(item);
            }
            else
            {
                destinationList.Insert(insertPosition.Value, item);
            }

            if (parent is Item i)
            {
                // Ensure the parent's children
                // are visible.
                i.IsExpanded = true;
            }

            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public async Task<Item> CloneItemAsync(Item item)
        {
            if (item == null || Current == null)
            {
                return null;
            }

            var clone = item.DeepClone() as Item;
            if (clone == null)
            {
                return null;
            }

            clone.Name += " (copy)";
            int insertPosition = item.Parent == null 
                ? Current.Items.IndexOf(item) + 1
                : item.Parent.Children.IndexOf(item) + 1;

            bool success = await AddItemAsync(clone, item.Parent, insertPosition);

            // Only return item if it was added successfully.
            return success ? clone : null;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAllItemsAsync()
        {
            if (Current == null)
            {
                return false;
            }

            bool confirmed = await _dialogService.ConfirmDeleteAllAsync();
            if (!confirmed)
            {
                return false;
            }

            Current.Items.Clear();
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteItemAsync(Item item)
        {
            if (item == null || Current == null)
            {
                return false;
            }

            bool confirmed = await _dialogService.ConfirmDeleteAsync();
            if (!confirmed)
            {
                return false;
            }

            if (item.Parent == null)
            {
                Current.Items.Remove(item);
            }
            else
            {
                item.Parent.Children.Remove(item);
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> EditItemAsync(Item item)
        {
            if (item == null || Current == null)
            {
                return false;
            }

            var tupleResult = await _dialogService.EditItemDialogAsync(item.Name);
            if (tupleResult.Item1 == false || string.IsNullOrWhiteSpace(tupleResult.Item2))
            {
                return false;
            }

            item.Name = tupleResult.Item2;
            return true;
        }

        /// <inheritdoc/>
        public async Task<Item> NewItemAsync(
            ItemType type,
            bool showCollectionSelector,
            Item currentParent = null)
        {
            if (Current == null)
            {
                return null;
            }

            // TODO try using Current.CurrentItem as the current parent.

            var tupleResult = await _dialogService.NewItemDialogAsync(
                type,
                showCollectionSelector ? Current.Items : null,
                currentParent);

            if (!tupleResult.Item1 || string.IsNullOrWhiteSpace(tupleResult.Item2))
            {
                return null;
            }

            Item newItem = _itemFactory.Create(type, tupleResult.Item2, tupleResult.Item3);
            bool success = await AddItemAsync(newItem, tupleResult.Item3);

            // Only return item if it was added successfully.
            return success ? newItem : null;
        }

        /// <inheritdoc/>
        public async Task<Item> InsertToWorkspaceAsync(Item item)
        {
            if (Current == null || item == null || !item.IsTemporary)
            {
                return null;
            }

            (bool userConfirmed, string itemName, Item selectedParent) = await _dialogService.NewItemDialogAsync(
                item.Type,
                Current.Items,
                prepopulatedName: item.Name);

            if (!userConfirmed || string.IsNullOrWhiteSpace(itemName))
            {
                return null;
            }

            item.Name = itemName;
            item.Parent = selectedParent;
            item.IsTemporary = false; // ensure false because item is now added into the workspace, thus no longer a temporary item.

            bool success = await AddItemAsync(item, selectedParent);
            return success ? item : null;
        }

        /// <inheritdoc/>
        public void SetWorkspace(Workspace w)
        {
            Current = w;
        }
    }
}
