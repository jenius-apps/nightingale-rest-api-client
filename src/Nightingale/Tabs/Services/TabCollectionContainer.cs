using Nightingale.Core.Workspaces.Factories;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Tabs.Factories;
using Nightingale.Tabs.Models;
using Nightingale.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nightingale.Tabs.Services
{
    /// <summary>
    /// Class for holding and manipulating the tab collection
    /// of a workspace. This allows any other class to manipulate
    /// the tabs via dependency injection.
    /// </summary>
    public class TabCollectionContainer : ITabCollectionContainer
    {
        private readonly IItemFactory _itemFactory;
        private readonly ITabViewModelFactory _tabVmFactory;
        private TabCollectionView _tabs;
        public event EventHandler CurrentTabChanged;

        public TabCollectionContainer(
            IItemFactory itemFactory,
            ITabViewModelFactory tabVmFactory)
        {
            _itemFactory = itemFactory 
                ?? throw new ArgumentNullException(nameof(itemFactory));
            _tabVmFactory = tabVmFactory
                ?? throw new ArgumentNullException(nameof(tabVmFactory));
        }

        /// <inheritdoc/>
        public RequestViewModel CurrentTab { get; set; }

        /// <inheritdoc/>
        public Task DuplicateTabAsync(Item item)
        {
            NullTabsGuard();

            Item clone = item.DeepClone() as Item;
            clone.Parent = null;
            clone.IsTemporary = true;

            return AddTabAsync(clone);
        }

        /// <inheritdoc/>
        public async Task AddTabAsync(Item item)
        {
            NullTabsGuard();

            var tab = await _tabVmFactory.Create(item);
            _tabs.Add(tab);

            // Ensure the tab is added by the UI first before switching tabs.
            await Task.Delay(100);
            CurrentTab = tab;
            TriggerCurrentTabChanged();
        }

        /// <inheritdoc/>
        public async void AddTempTab()
        {
            Item item = _itemFactory.Create(ItemType.Request, "Untitled", isTemp: true);
            await AddTabAsync(item);
        }

        /// <inheritdoc/>
        public void RemoveCurrentTab()
        {
            NullTabsGuard();
            if (_tabs.Count == 0 || CurrentTab == null || !_tabs.Contains(CurrentTab))
            {
                return;
            }

            _tabs.Remove(CurrentTab);   
        }

        /// <inheritdoc/>
        public void SelectNextTab()
        {
            NullTabsGuard();
            if (_tabs.Count == 0)
            {
                return;
            }

            if (CurrentTab == null)
            {
                CurrentTab = _tabs[0];
            }
            else
            {
                int currentIndex = _tabs.IndexOf(CurrentTab);
                if (currentIndex < 0 || currentIndex >= _tabs.Count - 1)
                {
                    CurrentTab = _tabs[0];
                }
                else
                {
                    CurrentTab = _tabs[currentIndex + 1];
                }
            }

            TriggerCurrentTabChanged();
        }

        /// <inheritdoc/>
        public void SelectPreviousTab()
        {
            NullTabsGuard();
            if (_tabs.Count == 0)
            {
                return;
            }

            if (CurrentTab == null)
            {
                CurrentTab = _tabs[0];
            }
            else
            {
                int currentIndex = _tabs.IndexOf(CurrentTab);
                if (currentIndex <= 0 || currentIndex >= _tabs.Count)
                {
                    CurrentTab = _tabs[_tabs.Count - 1];
                }
                else
                {
                    CurrentTab = _tabs[currentIndex - 1];
                }
            }

            TriggerCurrentTabChanged();
        }

        /// <inheritdoc/>
        public void SetTabCollection(TabCollectionView tabs, EventHandler tabChangedHandler)
        {
            // Ensure only one handler is subscribed at a time.
            if (CurrentTabChanged != null)
            {
                foreach (EventHandler eh in CurrentTabChanged.GetInvocationList())
                {
                    CurrentTabChanged -= eh;
                }
            }
            CurrentTabChanged += tabChangedHandler;

            _tabs = tabs;
            if (_tabs != null && _tabs.Count > 0)
            {
                CurrentTab = tabs[0];
                TriggerCurrentTabChanged();
            }
        }

        /// <inheritdoc/>
        public void RemoveTab(Item item)
        {
            NullTabsGuard();
            if (item == null)
            {
                return;
            }

            var tabToRemove = _tabs.Where(x => x.ViewModel?.Request.Id == item.Id).FirstOrDefault();
            if (tabToRemove != null)
            {
                _tabs.Remove(tabToRemove);
            }
        }

        /// <inheritdoc/>
        public void RemoveAllButThis(Item item)
        {
            NullTabsGuard();
            if (item == null)
            {
                return;
            }

            // ToArray required to avoid collection change conflicts with enumerables.
            var tabsToRemove = _tabs.Where(x => x.ViewModel?.Request.Id != item.Id).ToArray();
            foreach (var tab in tabsToRemove)
            {
                _tabs.Remove(tab);
            }
        }

        private void NullTabsGuard()
        {
            if (_tabs == null)
            {
                throw new InvalidOperationException("Tab container has null tabs. Are you forgetting to use SetTabCollection() first?");
            }
        }

        private void TriggerCurrentTabChanged() => CurrentTabChanged?.Invoke(this, new EventArgs());
    }
}
