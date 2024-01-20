using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    public class WorkspaceItemStorageAccessor : IWorkspaceItemStorageAccessor
    {
        private readonly IRequestStorageAccessor _requestStorageAccessor;
        private readonly ICollectionStorageAccessor _collectionStorageAccessor;

        public WorkspaceItemStorageAccessor(IRequestStorageAccessor requestStorageAccessor, ICollectionStorageAccessor collectionStorageAccessor)
        {
            _requestStorageAccessor = requestStorageAccessor ?? throw new ArgumentNullException(nameof(requestStorageAccessor));
            _collectionStorageAccessor = collectionStorageAccessor ?? throw new ArgumentNullException(nameof(collectionStorageAccessor));
        }

        public async Task DeleteItemAsync(WorkspaceItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item is WorkspaceRequest r)
            {
                await _requestStorageAccessor.DeleteRequestAsync(r);
            }
            else if (item is WorkspaceCollection c)
            {
                await _collectionStorageAccessor.DeleteCollectionAsync<WorkspaceCollection, WorkspaceRequest>(c);
            }
        }
    }
}
