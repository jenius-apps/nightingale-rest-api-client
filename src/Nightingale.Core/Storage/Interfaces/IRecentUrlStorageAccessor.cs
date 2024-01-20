using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IRecentUrlStorageAccessor
    {
        Task SaveRecentUrlAsync(RecentUrl recentUrl, string parentId);

        Task<IList<RecentUrl>> GetRecentUrlsAsync(string parentId);

        Task DeleteRecentUrlAsync(RecentUrl recentUrl);

        Task DeleteAllAsync(string parentId);
    }
}
