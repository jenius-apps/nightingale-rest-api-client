using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using Nightingale.Navigation;
using Nightingale.Tabs.Services;
using Nightingale.Utilities;
using System;

namespace Nightingale.History
{
    public class HistoryViewModel : ViewModels.ViewModelBase
    {
        private readonly IHistoryListModifier _historyListModifier;
        private readonly IWorkspaceItemNavigationService _workspaceNavigationService;
        private readonly ITabCollectionContainer _tabContainer;
        private bool _historyLoading;
        private Workspace _currentWorkspace;

        public HistoryViewModel(
            IHistoryListModifier historyListModifier,
            IWorkspaceItemNavigationService workspaceNavigationService,
            ITabCollectionContainer tabContainer)
        {
            _historyListModifier = historyListModifier
                ?? throw new ArgumentNullException(nameof(historyListModifier));
            _workspaceNavigationService = workspaceNavigationService
                ?? throw new ArgumentNullException(nameof(workspaceNavigationService));
            _tabContainer = tabContainer
                ?? throw new ArgumentNullException(nameof(tabContainer));
        }

        public IncrementalLoadingCollection<HistorySource, HistoryItem> HistoryList { get; private set; }

        public HistoryItem ContextSelectedItem { get; set; }

        public bool IsDeleteAllVisible
        {
            get => _currentWorkspace.HistoryItems.Count > 0;
        }

        public bool IsEmptyPlaceHolderVisible => !IsDeleteAllVisible;

        public bool HistoryLoading
        {
            get => HistoryList.IsLoading;
        }

        public async void RefreshAsync()
        {
            await HistoryList.RefreshAsync();
            UpdateVisibilities();
        }

        public void Initialize(Workspace w)
        {
            if (w == null)
            {
                return;
            }

            _currentWorkspace = w;
            var source = new HistorySource(w);
            HistoryList = new IncrementalLoadingCollection<HistorySource, HistoryItem>(source)
            {
                OnStartLoading = RaiseLoading,
                OnEndLoading = RaiseLoading
            };
        }

        public async void ClearHistory()
        {
            if (_historyLoading)
            {
                return;
            }

            _historyLoading = true;

            // clear the underlying history source
            bool success = await _historyListModifier.ClearAsync();
            if (success)
            {
                // make sure all open history tabs are closed
                foreach (var item in HistoryList)
                {
                    _tabContainer.RemoveTab(item);
                }

                // clear the list that's displayed on the UI
                HistoryList.Clear();
                Analytics.TrackEvent(Telemetry.HistoryCleared);
                UpdateVisibilities();

                if (_currentWorkspace.CurrentItem is HistoryItem)
                {
                    _currentWorkspace.CurrentItem = null;
                    await _workspaceNavigationService.NavigateTo(null);
                }
            }

            _historyLoading = false;
        }

        public async void DeleteContextSelectedItem()
        {
            if (ContextSelectedItem == null || _historyLoading)
            {
                return;
            }

            _historyLoading = true;

            bool success = await _historyListModifier.DeleteAsync(ContextSelectedItem);
            if (success)
            {
                HistoryList.Remove(ContextSelectedItem);
                _tabContainer.RemoveTab(ContextSelectedItem);
                Analytics.TrackEvent(Telemetry.HistoryItemDeleted);
                UpdateVisibilities();
            }

            ContextSelectedItem = null;
            _historyLoading = false;
        }

        private void RaiseLoading() => RaisePropertyChanged(nameof(HistoryLoading));

        private void UpdateVisibilities()
        {
            RaisePropertyChanged(nameof(IsDeleteAllVisible));
            RaisePropertyChanged(nameof(IsEmptyPlaceHolderVisible));
        }
    }
}
