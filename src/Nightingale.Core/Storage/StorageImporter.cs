using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage
{
    public class StorageImporter : IStorageImporter
    {
        private readonly IStorage _storage;
        private readonly Dictionary<string, string> _idMap = new Dictionary<string, string>();
        private List<Workspace> _workspaces = new List<Workspace>();
        private List<WorkspaceRequest> _requests = new List<WorkspaceRequest>();
        private List<HistoryRequest> _historyRequests = new List<HistoryRequest>();
        private List<WorkspaceCollection> _collections = new List<WorkspaceCollection>();
        private List<HistoryCollection> _historyCollections = new List<HistoryCollection>();
        private List<RequestBody> _bodies = new List<RequestBody>();
        private List<Models.Environment> _envs = new List<Models.Environment>();
        private List<ApiTest> _tests = new List<ApiTest>();
        private List<Authentication> _auths = new List<Authentication>();
        private List<Parameter> _parameters = new List<Parameter>();

        public StorageImporter(IStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task ImportAsync(string contents)
        {
            if (string.IsNullOrWhiteSpace(contents))
            {
                return;
            }

            Dictionary<string, JToken> deserialized;

            try
            {
                deserialized = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(contents);
            }
            catch
            {
                return;
            }

            Parallel.ForEach(deserialized, DeserializeItems);
            await InsertAll();
        }

        private void DeserializeItems(KeyValuePair<string, JToken> pair)
        {
            switch (pair.Key)
            {
                case nameof(Workspace):
                    _workspaces = pair.Value.ToObject<List<Workspace>>();
                    break;
                case nameof(WorkspaceRequest):
                    _requests = pair.Value.ToObject<List<WorkspaceRequest>>();
                    break;
                case nameof(WorkspaceCollection):
                    _collections = pair.Value.ToObject<List<WorkspaceCollection>>();
                    break;
                case nameof(RequestBody):
                    _bodies = pair.Value.ToObject<List<RequestBody>>();
                    break;
                case nameof(Parameter):
                    _parameters = pair.Value.ToObject<List<Parameter>>();
                    break;
                case nameof(Models.Environment):
                    _envs = pair.Value.ToObject<List<Models.Environment>>();
                    break;
                case nameof(ApiTest):
                    _tests = pair.Value.ToObject<List<ApiTest>>();
                    break;
                case nameof(Authentication):
                    _auths = pair.Value.ToObject<List<Authentication>>();
                    break;
                case nameof(HistoryRequest):
                    _historyRequests = pair.Value.ToObject<List<HistoryRequest>>();
                    break;
                case nameof(HistoryCollection):
                    _historyCollections = pair.Value.ToObject<List<HistoryCollection>>();
                    break;
                default:
                    return;
            }
        }

        private async Task InsertAll()
        {
            // ORDER MATTERS!
            await InsertIntoStorage(_workspaces, false);
            await InsertIntoStorage(_collections, true);
            await InsertIntoStorage(_historyCollections, true);
            await InsertIntoStorage(_requests, true);
            await InsertIntoStorage(_historyRequests, true);
            await InsertIntoStorage(_bodies, true);
            await InsertIntoStorage(_envs, true);
            await InsertIntoStorage(_tests, true);
            await InsertIntoStorage(_auths, true);
            await InsertIntoStorage(_parameters, true);
        }

        private async Task InsertIntoStorage<T>(List<T> list, bool updateParentId) where T : IStorageItem
        {
            foreach (T item in list)
            {
                string oldId = item.Id;
                item.Id = null;

                if (updateParentId)
                {
                    if (!_idMap.ContainsKey(item.ParentId))
                    {
                        throw new KeyNotFoundException("Failed assigning new parent ID.");
                    }

                    item.ParentId = _idMap[item.ParentId];
                }

                string newId = await _storage.SaveAsync(item);

                if (!_idMap.ContainsKey(oldId))
                {
                    _idMap.Add(oldId, newId);
                }
            }
        }
    }
}
