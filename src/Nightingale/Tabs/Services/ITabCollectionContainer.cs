using Nightingale.Core.Workspaces.Models;
using Nightingale.Tabs.Models;
using Nightingale.ViewModels;
using System;
using System.Threading.Tasks;

namespace Nightingale.Tabs.Services
{
    /// <summary>
    /// Interface responsible for making changes
    /// to a TabCollectionView.
    /// </summary>
    public interface ITabCollectionContainer
    {
        /// <summary>
        /// Event that is fired with the current tab is explicitly
        /// changed by this class.
        /// </summary>
        event EventHandler CurrentTabChanged;

        /// <summary>
        /// A pointer to the current tab.
        /// </summary>
        RequestViewModel CurrentTab { get; set; }

        /// <summary>
        /// Deep clones the given item
        /// and adds it to the tab collection
        /// as a temporary item.
        /// </summary>
        Task DuplicateTabAsync(Item item);

        /// <summary>
        /// Changes the current tab to the previous one. If the current tab
        /// is at 0-index, this will loop around to select the n-1 index.
        /// </summary>
        void SelectPreviousTab();
        
        /// <summary>
        /// Changes the current tab to the next one. If the curren tab
        /// is at n-1 index, this will loop around to select the 0 index.
        /// </summary>
        void SelectNextTab();

        /// <summary>
        /// Removes the currently selected tab.
        /// </summary>
        void RemoveCurrentTab();

        /// <summary>
        /// Sets the internal pointer for the TabCollectionView
        /// to the given tabs.
        /// </summary>
        /// <param name="tabs">The TabCollectionView to manipulate.</param>
        /// <param name="tabChangedHandler">The method that will be called when the tab is changed programmatically.</param>
        void SetTabCollection(TabCollectionView tabs, EventHandler tabChangedHandler);

        /// <summary>
        /// Adds a temporary tab to the tab collection.
        /// </summary>
        void AddTempTab();

        /// <summary>
        /// Adds the given item into the tab collection.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to add to the tab collection.</param>
        Task AddTabAsync(Item item);

        /// <summary>
        /// Removes the tab holding the given item.
        /// </summary>
        /// <param name="item">The item whose tab will be removed.</param>
        void RemoveTab(Item item);

        /// <summary>
        /// Removes all tabs except the given one.
        /// </summary>
        void RemoveAllButThis(Item item);
    }
}