using Nightingale.Core.Models;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IWorkspaceItemStorageAccessor
    {
        Task DeleteItemAsync(WorkspaceItem item);
    }
}
