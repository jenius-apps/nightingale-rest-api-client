using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IRecentUrlCache
    {
        Task AddRecentUrlAsync(string url);

        IList<string> GetSimilarUrls(string urlSubstring);

        Task InitializeAsync();

        Task ClearAllUrlsAsync();
    }
}
