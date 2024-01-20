using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    /// <summary>
    /// Extracts data from storage
    /// and returns it in string format.
    /// </summary>
    public class StorageExporter : IStorageExporter
    {
        private readonly IStorage _storage;

        public StorageExporter(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task<string> ExportAsync()
        {
            var document = new DocumentFile()
            {
                Workspaces = await GetDocumentFromStorage<Workspace>(),
                Environments = await GetDocumentFromStorage<Models.Environment>(),
            };

            return document.ToString();
        }

        private async Task<IList<T>> GetDocumentFromStorage<T>() where T : IStorageItem
        {
            var items = await _storage.GetCollectionAsync<T>(string.Empty);

            return items;
        }
    }
}
