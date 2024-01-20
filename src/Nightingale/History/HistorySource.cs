using Microsoft.Toolkit.Collections;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nightingale.History
{
    public class HistorySource : IIncrementalSource<HistoryItem>
    {
        private readonly IList<HistoryItem> _cache;

        public HistorySource(Workspace workspace)
        {
            _cache = workspace.HistoryItems;
        }

        public Task<IEnumerable<HistoryItem>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var orderedList = _cache.OrderByDescending(x => x.LastUsedDate);

            // Gets items from the collection according to pageIndex and pageSize parameters.
            var result = (from p in orderedList
                          select p).Skip(pageIndex * pageSize).Take(pageSize);

            return Task.FromResult(result);
        }
    }
}
