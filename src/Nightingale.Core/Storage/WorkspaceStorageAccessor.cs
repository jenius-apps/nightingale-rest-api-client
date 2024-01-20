using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using Nightingale.Core.Workspaces.Factories;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    public class WorkspaceStorageAccessor : IWorkspaceStorageAccessor
    {
        private readonly IStorage _storage;
        private readonly IRequestStorageAccessor _requestAccessor;
        private readonly ICollectionStorageAccessor _collectionAccessor;
        private readonly IEnvironmentStorageAccessor _environmentAccessor;
        private readonly IEnvironmentListModifier _envListModifier;
        private readonly IItemFactory _itemFactory;

        public WorkspaceStorageAccessor(
            IStorage storage,
            IRequestStorageAccessor requestAccessor,
            ICollectionStorageAccessor collectionAccessor,
            IEnvironmentStorageAccessor environmentAccessor,
            IEnvironmentListModifier envListModifier,
            IItemFactory itemFactory)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _requestAccessor = requestAccessor ?? throw new ArgumentNullException(nameof(requestAccessor));
            _collectionAccessor = collectionAccessor ?? throw new ArgumentNullException(nameof(collectionAccessor));
            _environmentAccessor = environmentAccessor ?? throw new ArgumentNullException(nameof(environmentAccessor));
            _envListModifier = envListModifier ?? throw new ArgumentNullException(nameof(envListModifier));
            _itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
        }

        public async Task<IList<Workspace>> GetWorkspacesAsync(string parentId)
        {
            if (parentId == null)
            {
                return new List<Workspace>();
            }

            IList<Workspace> workspaces = await _storage.GetCollectionAsync<Workspace>(parentId);
            
            foreach (Workspace w in workspaces)
            {
                await LoadWorkspaceItemsAsync(w);
                await LoadEnvironmentsAsync(w);
            }

            // Part of migration to new item
            var idList = workspaces.Select(x => x.Id).ToList();
            await _requestAccessor.DeleteAllAsync<WorkspaceRequest>(idList);
            await _collectionAccessor.DeleteAllAsync<WorkspaceCollection, WorkspaceRequest>(idList);
            await _environmentAccessor.DeleteAllAsync(idList);
            return workspaces;
        }

        private async Task LoadWorkspaceItemsAsync(Workspace workspace)
        {
            if (workspace == null || string.IsNullOrWhiteSpace(workspace.Id))
            {
                return;
            }

            IList<WorkspaceRequest> requests = await _requestAccessor.GetRequestsAsync<WorkspaceRequest>(workspace.Id);
            IList<WorkspaceCollection> cols = await _collectionAccessor.GetCollectionsAsync<WorkspaceCollection, WorkspaceRequest>(workspace.Id);

            var sortingList = new List<WorkspaceItem>();
            sortingList.AddRange(requests);
            sortingList.AddRange(cols);
            sortingList = sortingList.OrderBy(x => x.Position).ToList();

            // Migrate all to new item
            foreach (WorkspaceItem item in sortingList)
            {
                Item newItem = _itemFactory.Migrate(item);

                if (newItem != null)
                {
                    workspace.Items.Add(newItem);
                }
            }
        }

        private async Task LoadEnvironmentsAsync(Workspace workspace)
        {
            if (workspace == null || string.IsNullOrWhiteSpace(workspace.Id))
            {
                return;
            }

            IList<Models.Environment> environments = await _environmentAccessor.GetEnvironmentsAsync(workspace.Id);

            foreach (Models.Environment e in environments)
            {
                workspace.Environments.Add(e);
            }

            if (workspace.Environments.Count == 0)
            {
                _envListModifier.AddBaseEnvironment(workspace.Environments);
            }
        }

        public async Task SaveWorkspacesAsync(IList<Workspace> workspaces, string parentId, string activeWorkspaceId, bool commitChanges = true)
        {
            if (workspaces == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            foreach (var workspace in workspaces)
            {
                await SaveAsync(workspace, parentId);
            }

            if (commitChanges)
            {
                await _storage.WriteChangesAsync(activeWorkspaceId);
            }
        }

        public async Task SaveAsync(Workspace workspace, string parentId)
        {
            if (workspace == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            workspace.ParentId = parentId;
            await _storage.SaveAsync(workspace);
        }

        public async Task DeleteWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null)
            {
                return;
            }

            await _storage.DeleteAsync(workspace);
        }
    }
}
