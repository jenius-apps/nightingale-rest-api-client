using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    public class RecentUrlStorageAccessor : IRecentUrlStorageAccessor
    {
        private readonly IStorage _storage;

        public RecentUrlStorageAccessor(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task DeleteRecentUrlAsync(RecentUrl recentUrl)
        {
            if (recentUrl == null)
            {
                return;
            }

            await _storage.DeleteAsync(recentUrl);
        }

        public async Task<IList<RecentUrl>> GetRecentUrlsAsync(string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new List<RecentUrl>();
            }

            return await _storage.GetCollectionAsync<RecentUrl>(parentId);
        }

        public async Task SaveRecentUrlAsync(RecentUrl recentUrl, string parentId)
        {
            if (recentUrl == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            recentUrl.ParentId = parentId;
            await _storage.SaveAsync(recentUrl);
        }

        public async Task DeleteAllAsync(string parentId)
        {
            IList<RecentUrl> allList = await GetRecentUrlsAsync(parentId);

            foreach (var recentUrl in allList)
            {
                await DeleteRecentUrlAsync(recentUrl);
            }
        }
    }
}
