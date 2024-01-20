using Newtonsoft.Json;
using Nightingale.Core.Http;
using Nightingale.Core.ImportConverters.Nightingale;
using Nightingale.Core.Models;
using Nightingale.Core.Storage;
using Nightingale.Core.Workspaces.Factories;
using Nightingale.Handlers;
using Nightingale.Modifiers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Importers
{
    /// <summary>
    /// Implementation of <see cref="INcfImporter"/> for UWP.
    /// </summary>
    public class NcfImporter : INcfImporter
    {
        /// <inheritdoc/>
        public async Task<IList<Workspace>> ImportFileAsync(object storageFile)
        {
            if (!(storageFile is StorageFile file))
            {
                return new List<Workspace>();
            }

            string content = await FileIO.ReadTextAsync(file);
            DocumentFile data = JsonConvert.DeserializeObject<DocumentFile>(content);

            if (data is null)
            {
                return new List<Workspace>();
            }

            var storage = new DocumentStorage(file);
            await storage.InitializeAsync();

            var parameterStorage = new ParameterStorageAccessor(storage);
            var testStorage = new ApiTestStorageAccessor(storage);
            var authStorage = new AuthStorageAccessor(storage);
            var bodyStorage = new RequestBodyStorageAccessor(storage, parameterStorage);
            var envStorage = new EnvironmentStorageAccessor(storage, parameterStorage);
            var envModifier = new EnvironmentListModifier(envStorage);

            var requestStorage = new RequestStorageAccessor(
                storage,
                parameterStorage,
                testStorage,
                authStorage,
                bodyStorage);

            var colStorage = new CollectionStorageAccessor(
                storage,
                requestStorage,
                authStorage);

            var methods = new MethodsContainer();
            var itemFactory = new ItemFactory(methods);
            var workspaceStorage = new WorkspaceStorageAccessor(
                storage,
                requestStorage,
                colStorage,
                envStorage,
                envModifier,
                itemFactory);

            var workspaces = await workspaceStorage.GetWorkspacesAsync("root");
            if (workspaces == null)
            {
                return new List<Workspace>();
            }

            // Ensure the Ids are new to
            // prevent collision if user imports
            // the same file multiple times.
            foreach (var w in workspaces)
            {
                w.Id = Guid.NewGuid().ToString();
            }

            return workspaces;
        }
    }
}
