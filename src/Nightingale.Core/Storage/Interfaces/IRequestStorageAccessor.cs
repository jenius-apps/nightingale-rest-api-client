using Nightingale.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IRequestStorageAccessor
    {
        Task Hydrate<T>(T request) where T : IWorkspaceRequest;

        Task SaveRequestAsync<T>(T request, string parentId) where T : IWorkspaceRequest;

        Task<IList<T>> GetRequestsAsync<T>(string parentId) where T : IWorkspaceRequest;

        Task DeleteRequestAsync<T>(T request) where T : IWorkspaceRequest;

        Task DeleteAllAsync<T>(IList<string> parentIds) where T : IWorkspaceRequest;
    }
}
