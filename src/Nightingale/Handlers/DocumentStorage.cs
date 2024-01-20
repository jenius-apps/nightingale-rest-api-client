using Newtonsoft.Json;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OsStorage = Windows.Storage;

namespace Nightingale.Handlers
{
    public class DocumentStorage : IStorage
    {
        public const string DefaultLocalStorageFileName = "nightingale3.ncf";
        private readonly OsStorage.StorageFile _storageFile;
        private DocumentFile _documentCache;

        public DocumentStorage(OsStorage.StorageFile storageFile)
        {
            _storageFile = storageFile ?? throw new ArgumentNullException(nameof(storageFile));
            OsStorage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(_storageFile);
        }

        public async Task InitializeAsync()
        {
            string content = await OsStorage.FileIO.ReadTextAsync(_storageFile);
            _documentCache = JsonConvert.DeserializeObject<DocumentFile>(content) ?? new DocumentFile();
        }

        public async Task DeleteAsync<T>(T item) where T : IStorageItem
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
            {
                return;
            }

            await Task.Delay(1);
            IList<T> list = _documentCache.GetList<T>();
            var existingItem = list.Where(x => x.Id == item.Id).FirstOrDefault();

            if (existingItem == null)
            {
                return;
            }

            list.Remove(existingItem);
        }

        public async Task DeleteAllAsync<T>(IList<string> parentIds) where T : IStorageItem
        {
            await Task.Delay(1);
            IList<T> list = _documentCache.GetList<T>();

            if (parentIds == null)
            {
                list.Clear();
            }
            else
            {
                var forRemoval = list.Where(x => parentIds.Contains(x.ParentId)).ToList();

                foreach (var item in forRemoval)
                {
                    list.Remove(item);
                }
            }
        }

        public async Task<T> GetItemAsync<T>(string id, bool passReference = false) where T : IStorageItem
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return default;
            }

            IList<T> list = _documentCache.GetList<T>();
            var item = list.Where(x => x.Id == id).FirstOrDefault();

            if (item == null)
            {
                return default;
            }

            if (passReference)
            {
                return item;
            }

            await Task.Delay(1);
            var serialized = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public async Task<T> GetAsync<T>(string parentId, bool passReference = false) where T : IStorageItem
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                return default;
            }

            await Task.Delay(1);
            IList<T> list = _documentCache.GetList<T>();
            var item = list.Where(x => x.ParentId == parentId).FirstOrDefault();

            if (item == null)
            {
                return default;
            }

            if (passReference)
            {
                return item;
            }

            // Serialize then deserialize in order to prevent pass by reference.
            var itemstring = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<T>(itemstring);
        }

        public async Task<IList<T>> GetAsync<T>(IList<string> parentIds) where T : IStorageItem
        {
            await Task.Delay(1);
            IList<T> list = _documentCache.GetList<T>();

            // Serialize then deserialize in order to prevent pass by reference.
            var listString = JsonConvert.SerializeObject(list);
            list = JsonConvert.DeserializeObject<IList<T>>(listString);

            return parentIds == null ? list : list.Where(x => parentIds.Contains(x.ParentId)).ToList();
        }

        public async Task<IList<T>> GetCollectionAsync<T>(string parentId, bool passReference = false) where T : IStorageItem
        {
            await Task.Delay(1);
            IList<T> list = _documentCache.GetList<T>();

            IList<T> result;

            if (passReference)
            {
                result = list;
            }
            else
            {
                // Serialize then deserialize in order to prevent pass by reference.
                var listString = JsonConvert.SerializeObject(list);
                result = JsonConvert.DeserializeObject<IList<T>>(listString);
            }

            return string.IsNullOrWhiteSpace(parentId) ? result : result.Where(x => x.ParentId == parentId).ToList();
        }

        public async Task<string> SaveAsync<T>(T item) where T : IStorageItem
        {
            if (item == null)
            {
                return null;
            }

            await Task.Delay(1);
            IList<T> list = _documentCache.GetList<T>();

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();

                // Serialize then deserialize in order to prevent pass by reference.
                var itemString = JsonConvert.SerializeObject(item);
                var itemToAdd = JsonConvert.DeserializeObject<T>(itemString);

                list.Add(itemToAdd);
            }
            else
            {
                // Serialize then deserialize in order to prevent pass by reference.
                var itemString = JsonConvert.SerializeObject(item);
                var itemToAdd = JsonConvert.DeserializeObject<T>(itemString);

                var existingItem = list.Where(x => x.Id == itemToAdd.Id).FirstOrDefault();

                if (existingItem != null)
                {
                    int index = list.IndexOf(existingItem);
                    list[index] = itemToAdd;
                }
                else
                {
                    list.Add(itemToAdd);
                }
            }

            return item.Id;
        }

        public string GetPathAndName()
        {
            return _storageFile.Path;
        }

        public string GetFileName()
        {
            return _storageFile.Path.Contains(OsStorage.ApplicationData.Current.LocalFolder.Path) ? "" : _storageFile.DisplayName;
        }

        public string GetActiveWorkspaceId()
        {
            return _documentCache.ActiveWorkspaceId;
        }

        public async Task WriteChangesAsync(string activeWorkspaceId)
        {
            _documentCache.ActiveWorkspaceId = activeWorkspaceId;
            string text = _documentCache.ToString();

            try
            {
                using (var stream = await _storageFile.OpenAsync(OsStorage.FileAccessMode.ReadWrite))
                using (var outputStream = stream.GetOutputStreamAt(0))
                using (var dataWriter = new OsStorage.Streams.DataWriter(outputStream))
                {
                    dataWriter.WriteString(text);

                    // We set the stream size to be the same size as the text we are inserting.
                    // This ensures that the file is completely overwritten.
                    stream.Size = await dataWriter.StoreAsync();

                    await outputStream.FlushAsync();
                }
            }
            catch
            {
                // Mitigate crashing when file was moved or deleted.

                // TODO: improve handling of scenario when file was moved or deleted.
                // Perhaps create new file using same path.
            }
        }
    }
}
