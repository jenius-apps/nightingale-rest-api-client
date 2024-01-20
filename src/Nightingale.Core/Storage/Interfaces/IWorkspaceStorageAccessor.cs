using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IWorkspaceStorageAccessor
    {
        Task SaveWorkspacesAsync(IList<Workspace> workspaces, string parentId, string activeWorkspaceId, bool commitChanges = true);

        Task SaveAsync(Workspace workspace, string parentId);

        Task<IList<Workspace>> GetWorkspacesAsync(string parentId);

        Task DeleteWorkspaceAsync(Workspace workspace);
    }
}
