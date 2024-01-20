using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nightingale.Core.Helpers
{
    public class RecentUrlCache : IRecentUrlCache
    {
        private readonly IRecentUrlStorageAccessor _recentUrlStorageAccessor;
        private readonly string _parentId;
        private IList<RecentUrl> _recentUrls;

        public RecentUrlCache(
            IRecentUrlStorageAccessor recentUrlStorageAccessor,
            string rootParentId)
        {
            _recentUrlStorageAccessor = recentUrlStorageAccessor ?? throw new ArgumentNullException(nameof(recentUrlStorageAccessor));
            _parentId = !string.IsNullOrWhiteSpace(rootParentId) ? rootParentId : throw new ArgumentNullException(nameof(rootParentId));
        }

        public async Task InitializeAsync()
        {
            _recentUrls = await _recentUrlStorageAccessor.GetRecentUrlsAsync(_parentId);
        }

        public async Task AddRecentUrlAsync(string url)
        {
            NullRecentUrlsGuard();

            if (_recentUrls.Any(x => x.Url == url))
            {
                return;
            }

            var newRecentUrl = new RecentUrl
            {
                ParentId = _parentId,
                Url = url,
                Timestamp = DateTimeOffset.Now
            };

            _recentUrls.Add(newRecentUrl);
            await _recentUrlStorageAccessor.SaveRecentUrlAsync(newRecentUrl, _parentId);
        }

        public IList<string> GetSimilarUrls(string urlSubstring)
        {
            NullRecentUrlsGuard();

            return _recentUrls.Where(x => x.Url.ToLower().Contains(urlSubstring)).Select(x => x.Url).ToList();
        }

        public async Task ClearAllUrlsAsync()
        {
            NullRecentUrlsGuard();

            _recentUrls.Clear();
            await _recentUrlStorageAccessor.DeleteAllAsync(_parentId);
        }

        private void NullRecentUrlsGuard()
        {
            if (_recentUrls == null)
            {
                throw new ArgumentNullException(nameof(_recentUrls), "RecentUrlManager requires initialization");
            }
        }
    }
}
