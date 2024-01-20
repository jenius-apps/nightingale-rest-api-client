using Nightingale.Core.Dialogs;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using Nightingale.Core.Workspaces.Factories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Workspaces.Services
{
    /// <summary>
    /// Class for modifying the list of workspaces.
    /// </summary>
    public class WorkspaceListModifier : IWorkspaceListModifier
    {
        private readonly IDialogService _dialogService;
        private readonly IWorkspaceFactory _workspaceFactory;
        private readonly IWorkspaceStorageAccessor _workspaceStorageAccessor;

        public WorkspaceListModifier(
            IDialogService dialogService,
            IWorkspaceFactory workspaceFactory,
            IWorkspaceStorageAccessor workspaceStorageAccessor)
        {
            _dialogService = dialogService ??
                throw new ArgumentNullException(nameof(dialogService));
            _workspaceFactory = workspaceFactory ??
                throw new ArgumentNullException(nameof(workspaceFactory));
            _workspaceStorageAccessor = workspaceStorageAccessor ??
                throw new ArgumentNullException(nameof(workspaceStorageAccessor));
        }

        /// <inheritdoc/>
        public IList<Workspace> Current { get; private set; }

        /// <inheritdoc/>
        public Task<bool> AddWorkspaceAsync(Workspace w)
        {
            if (w == null || Current == null)
            {
                return Task.FromResult(false);
            }

            Current.Add(w);
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteWorkspaceAsync(Workspace forDeletion)
        {
            if (forDeletion == null || Current == null)
            {
                return false;
            }

            bool confirmed = await _dialogService.ConfirmDeleteAsync(forceConfirm: true);
            if (!confirmed)
            {
                return false;
            }

            Current.Remove(forDeletion);

            // delete from storage
            await _workspaceStorageAccessor.DeleteWorkspaceAsync(forDeletion);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> EditWorkspaceAsync(Workspace w)
        {
            if (w == null || Current == null)
            {
                return false;
            }

            (bool confirm, string newName) = await _dialogService.WorkspaceDialogAsync(w.Name);
            if (!confirm || string.IsNullOrWhiteSpace(newName))
            {
                return false;
            }

            w.Name = newName;
            return true;
        }

        /// <inheritdoc/>
        public async Task<Workspace> NewWorkspaceAsync()
        {
            if (Current == null)
            {
                return null;
            }

            (bool confirm, string workspaceName) = await _dialogService.WorkspaceDialogAsync("Untitled");
            if (!confirm || string.IsNullOrWhiteSpace(workspaceName))
            {
                return null;
            }

            return await NewWorkspaceAsync(workspaceName);
        }

        /// <inheritdoc/>
        public async Task<Workspace> NewWorkspaceAsync(string name)
        {
            if (Current == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var workspace = _workspaceFactory.Create(name);
            bool success = await AddWorkspaceAsync(workspace);

            return success ? workspace : null;
        }

        /// <inheritdoc/>
        public void SetList(IList<Workspace> list)
        {
            Current = list;
        }
    }
}
