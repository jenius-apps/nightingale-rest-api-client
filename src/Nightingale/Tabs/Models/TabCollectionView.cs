using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using Nightingale.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Nightingale.Tabs.Models
{
    /// <summary>
    /// Custom observable collection representing
    /// Nightingale's tab collection. Used to synchronize the
    /// tabs in the UI with the tabs stored in the workspace object.
    /// </summary>
    public class TabCollectionView : ObservableCollection<RequestViewModel>
    {
        private readonly Workspace _workspace;

        /// <summary>
        /// Create instance of class with a reference to
        /// the workspace whose tabs will be managed.
        /// </summary>
        /// <param name="workspace">The workspace whose tabs will be managed.</param>
        public TabCollectionView(Workspace workspace)
        {
            _workspace = workspace
                ?? throw new ArgumentNullException(nameof(workspace), "A null workspace is invalid for this class");

            this.CollectionChanged += TabCollectionView_CollectionChanged;
        }

        /// <summary>
        /// If true, changes to this list will not be reflected
        /// into the workspace attached to this tab view collection. Often used
        /// when the list is being initialized.
        /// </summary>
        public bool IgnoreChanges { get; set; }

        private void TabCollectionView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IgnoreChanges)
            {
                // Ignore collection changes during initialization.
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                TabAdded(GetItem(e.NewItems[0]));
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                TabRemoved(GetItem(e.OldItems[0]));
            }
        }

        private Item GetItem(object tabItem)
        {
            if (tabItem is RequestViewModel x)
            {
                return x.ViewModel.Request;
            }

            throw new InvalidCastException("The tab item couldn't be cast correctly. Did you change the type?");
        }

        private void TabAdded(Item addedItem)
        {
            _workspace.OpenItemIds.Add(addedItem.Id);

            if (addedItem.IsTemporary)
            {
                _workspace.TempItems.Add(addedItem);
            }
        }

        private void TabRemoved(Item removedItem)
        {
            _workspace.OpenItemIds.Remove(removedItem.Id);

            if (removedItem.IsTemporary)
            {
                _workspace.TempItems.Remove(removedItem);
            }
        }
    }
}
