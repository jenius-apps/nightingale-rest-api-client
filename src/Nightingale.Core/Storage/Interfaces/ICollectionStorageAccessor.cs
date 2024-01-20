using Nightingale.Core.Interfaces;
using Nightingale.Core.Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface ICollectionStorageAccessor
    {
        Task SaveCollectionAsync<T, R>(T collection, string parentId)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest;

        Task<IList<T>> GetCollectionsAsync<T, R>(string parentId, int? take = null)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest;

        Task DeleteCollectionAsync<T, R>(T collection)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest;

        Task DeleteAllAsync<T, R>(IList<string> parentIds)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest;

        Task<T> GetCollectionAsync<T, R>(string collectionId, bool passReference = false)
            where T : IWorkspaceCollection
            where R : IWorkspaceRequest;
    }
}
