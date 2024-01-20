using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Interfaces
{
    public interface IStorage
    {
        Task<IList<T>> GetCollectionAsync<T>(string parentId, bool passReference = false) where T : IStorageItem;

        Task<IList<T>> GetAsync<T>(IList<string> parentIds) where T : IStorageItem;

        Task<T> GetAsync<T>(string parentId, bool passReference = false) where T : IStorageItem;

        Task<T> GetItemAsync<T>(string id, bool passReference = false) where T : IStorageItem;

        Task<string> SaveAsync<T>(T item) where T : IStorageItem;

        Task DeleteAsync<T>(T item) where T : IStorageItem;

        Task DeleteAllAsync<T>(IList<string> parentIds) where T : IStorageItem;

        string GetPathAndName();

        string GetFileName();

        Task WriteChangesAsync(string activeWorkspaceId);

        string GetActiveWorkspaceId();
    }
}
