using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;

namespace Nightingale.Core.Storage
{
    public class ApiTestStorageAccessor : IApiTestStorageAccessor
    {
        private readonly IStorage _storage;

        public ApiTestStorageAccessor(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task DeleteTestAsync(ApiTest apiTest)
        {
            if (apiTest == null)
            {
                return;
            }

            await _storage.DeleteAsync(apiTest);
        }

        public async Task<IList<ApiTest>> GetApiTestsAsync(string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new List<ApiTest>();
            }

            return await _storage.GetCollectionAsync<ApiTest>(parentId);
        }

        public async Task DeleteAllAsync(IList<string> parentIds)
        {
            await _storage.DeleteAllAsync<ApiTest>(parentIds);
        }

        public async Task SaveTestAsync(ApiTest apiTest, string parentId)
        {
            if (apiTest == null || string.IsNullOrWhiteSpace(parentId))
            {
                return;
            }

            apiTest.ParentId = parentId;
            await _storage.SaveAsync(apiTest);
        }
    }
}
