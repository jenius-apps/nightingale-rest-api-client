using Nightingale.Core.Dialogs;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Nightingale.Core.Workspaces.Services
{
    /// <summary>
    /// Class for modifying the current
    /// workspace's history list.
    /// </summary>
    public class HistoryListModifier : IHistoryListModifier
    {
        private const int MaxCount = 20;
        private readonly ICurrentWorkspaceContainer _currentWorkspace;
        private readonly IDialogService _dialogService;

        public HistoryListModifier(
            ICurrentWorkspaceContainer currentWorkspace,
            IDialogService dialogService)
        {
            _currentWorkspace = currentWorkspace
                ?? throw new ArgumentNullException(nameof(currentWorkspace));
            _dialogService = dialogService
                ?? throw new ArgumentNullException(nameof(dialogService));
        }

        /// <inheritdoc/>
        public Task AddAsync(Item item, DateTime lastUsed)
        {
            if (item == null
                || item is HistoryItem
                || item.Type == ItemType.Collection)
            {
                return Task.CompletedTask;
            }

            var current = _currentWorkspace.Get();
            if (current == null)
            {
                return Task.CompletedTask;
            }

            TryTrimOld(current.HistoryItems, MaxCount);
            var historyItem = new HistoryItem(item, lastUsed);
            current.HistoryItems.Add(historyItem);
            return Task.CompletedTask;
        }

        private void TryTrimOld(ObservableCollection<HistoryItem> list, int maxItems)
        {
            if (list == null || list.Count < maxItems)
            {
                return;
            }

            while (list.Count > maxItems)
            {
                list.RemoveAt(0);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ClearAsync()
        {
            var current = _currentWorkspace.Get();
            if (current == null)
            {
                return false;
            }

            bool confirmed = await _dialogService.ConfirmDeleteAllAsync();
            if (!confirmed)
            {
                return false;
            }

            current.HistoryItems.Clear();
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(HistoryItem item)
        {
            if (item == null)
            {
                return false;
            }

            var current = _currentWorkspace.Get();
            if (current == null)
            {
                return false;
            }

            bool confirmed = await _dialogService.ConfirmDeleteAsync();
            if (!confirmed)
            {
                return false;
            }

            current.HistoryItems.Remove(item);
            return true;
        }
    }
}
