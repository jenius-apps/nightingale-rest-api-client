using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Models.Interfaces;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    public class CollectionStorageAccessor : ICollectionStorageAccessor
    {
        private readonly IStorage _storage;
        private readonly IRequestStorageAccessor _requestAccessor;
        private readonly IAuthStorageAccessor _authStorage;
        //private readonly IEnvironmentStorageAccessor _environmentAccessor;

        public CollectionStorageAccessor(
            IStorage storage,
            IRequestStorageAccessor requestAccessor,
            IAuthStorageAccessor authStorage)

        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _requestAccessor = requestAccessor ?? throw new ArgumentNullException(nameof(requestAccessor));
            _authStorage = authStorage ?? throw new ArgumentNullException(nameof(authStorage));
            //_environmentAccessor = environmentAccessor ?? throw new ArgumentNullException(nameof(environmentAccessor));
        }

        public async Task<T> GetCollectionAsync<T, R>(string collectionId, bool passReference = false)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest
        {
            if (string.IsNullOrWhiteSpace(collectionId))
            {
                return default;
            }

            // TODO add option to expand the properties of the
            // retrieved item.
            var collection = await _storage.GetItemAsync<T>(collectionId, passReference);
            return collection;
        }

        public async Task<IList<T>> GetCollectionsAsync<T, R>(string parentId, int? take = null)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new List<T>();
            }

            IList<T> collections = await _storage.GetCollectionAsync<T>(parentId, passReference: true);

            if (collections == null || collections.Count == 0)
            {
                return new List<T>();
            }

            if (take != null)
            {
                collections = collections.OrderBy(x => x.Position).Take(take.Value).ToList();
            }

            foreach (T collection in collections)
            {
                collection.Authentication = await _authStorage.GetAuthenticationAsync(collection.Id);

                IList<R> childRequests = await _requestAccessor.GetRequestsAsync<R>(collection.Id);
                IList<T> childCollections = await GetCollectionsAsync<T, R>(collection.Id);

                if (take != null)
                {
                    childRequests = childRequests.OrderBy(x => x.Position).Take(take.Value).ToList();
                    childCollections = childCollections.OrderBy(x => x.Position).Take(take.Value).ToList();
                }

                var childrenList = new List<WorkspaceItem>();
                childrenList.AddRange(childRequests.Cast<WorkspaceItem>());
                childrenList.AddRange(childCollections.Cast<WorkspaceItem>());

                foreach (WorkspaceItem item in childrenList.OrderBy(x => x.Position))
                {
                    collection.Children.Add(item);
                }
            }

            return collections;
        }

        //private async Task LoadEnvironmentsAsync(WorkspaceCollection collection)
        //{
        //    if (collection == null || string.IsNullOrWhiteSpace(collection.Id))
        //    {
        //        return;
        //    }

        //    IList<Models.Environment> environments = await _environmentAccessor.GetEnvironmentsAsync(collection.Id);

        //    foreach (Models.Environment e in environments)
        //    {
        //        collection.CollectionEnvironments.Add(e);
        //    } 
        //}

        public async Task SaveCollectionAsync<T, R>(T collection, string parentId)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest
        {
            if (collection == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            collection.ParentId = parentId;
            string collectionId = await _storage.SaveAsync(collection);

            int position = 0;
            foreach (WorkspaceItem item in collection.Children)
            {
                item.Position = position;

                if (item is R request)
                {
                    await _requestAccessor.SaveRequestAsync(request, collectionId);
                }
                else if (item is T col)
                {
                    await SaveCollectionAsync<T, R>(col, collectionId);
                }
                else
                {
                    throw new NotImplementedException();
                }

                position++;
            }

            await _authStorage.SaveAuthenticationAsync(collection.Authentication, collection.Id);

            //foreach (Models.Environment env in collection.CollectionEnvironments)
            //{
            //    await _environmentAccessor.SaveEnvironmentAsync(env, collectionId);
            //}
        }

        public async Task DeleteCollectionAsync<T, R>(T collection)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest
        {
            if (collection == null)
            {
                return;
            }

            await _storage.DeleteAsync(collection);

            foreach (WorkspaceItem item in collection.Children)
            {
                if (item is R request)
                {
                    await _requestAccessor.DeleteRequestAsync(request);
                }
                else if (item is T col)
                {
                    await DeleteCollectionAsync<T, R>(col);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            //foreach (Models.Environment env in collection.CollectionEnvironments)
            //{
            //    await _environmentAccessor.DeleteEnvironmentAsync(env);
            //}
        }

        public async Task DeleteAllAsync<T, R>(IList<string> parentIds)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest
        {
            if (parentIds != null && parentIds.Count == 0)
            {
                return;
            }

            IList<T> forDeletion = await _storage.GetAsync<T>(parentIds);
            IList<string> forDeletionIds = forDeletion.Select(x => x.Id).ToList();

            await _requestAccessor.DeleteAllAsync<R>(forDeletionIds);
            await this.DeleteAllAsync<T, R>(forDeletionIds);
            await _storage.DeleteAllAsync<T>(parentIds);
        }
    }
}
