using Newtonsoft.Json;
using Nightingale.Core.Dialogs;
using Nightingale.Core.Helpers;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JeniusApps.Nightingale.Converters.Postman;
using DTO = JeniusApps.Nightingale.Data.Models;
using Newtonsoft.Json.Serialization;

namespace Nightingale.Core.Export
{
    public class ExportService : IExportService
    {
        private readonly IDialogService _dialogService;
        private readonly IStorageFileWriter _storageFileWriter;
        private readonly IPostmanConverter _postmanConverter;

        public ExportService(
            IDialogService dialogService,
            IStorageFileWriter storageFileWriter,
            IPostmanConverter postmanConverter)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _storageFileWriter = storageFileWriter ?? throw new ArgumentNullException(nameof(storageFileWriter));
            _postmanConverter = postmanConverter ?? throw new ArgumentNullException(nameof(postmanConverter));
        }

        /// <inheritdoc/>
        public async Task<string> ExportAsync(IList<Workspace> workspaces)
        {
            if (workspaces == null || workspaces.Count == 0)
            {
                return null;
            }

            ExportConfiguration config = await _dialogService.ExportDialogAsync(workspaces);
            if (config == null)
            {
                return null;
            }

            string exportedPath = string.Empty;

            // perform export
            switch (config.Format)
            {
                case ExportFormat.Nightingale:
                    exportedPath = await ExportNightingaleAsync(workspaces);
                    break;
                case ExportFormat.Postman:
                    break;
            }

            return exportedPath;
        }

        /// <inheritdoc/>
        public async Task<string> ExportCollectionAsync(Item collection, ExportFormat format)
        {
            if (collection is null)
            {
                return string.Empty;
            }

            string exportedPath = string.Empty;
            switch (format)
            {
                case ExportFormat.Nightingale:
                    break;
                case ExportFormat.Postman:
                    exportedPath = await ExportAsPostmanAsync(collection);
                    break;
            }

            return exportedPath;
        }

        private Task<string> ExportAsPostmanAsync(Item collection)
        {
            if (collection != null)
            {
                var serialized = JsonConvert.SerializeObject(collection);
                var dtoItem = JsonConvert.DeserializeObject<DTO.Item>(serialized);
                var postmanCollection = _postmanConverter.ConvertCollection(dtoItem);

                string doc = JsonConvert.SerializeObject(
                    postmanCollection,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                return _storageFileWriter.WriteTextAsync(
                    doc,
                    collection.Name,
                    "JSON file",
                    ".json");
            }

            return Task.FromResult(string.Empty);
        }

        private Task<string> ExportNightingaleAsync(IList<Workspace> list)
        {
            if (list != null && list.Count > 0)
            {
                var document = new DocumentFile { Workspaces = list };

                string doc = JsonConvert.SerializeObject(
                    document,
                    Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new PrivateVariableMaskResolver() });

                return _storageFileWriter.WriteTextAsync(
                    doc,
                    "workspaces",
                    "Nightingale Collection Format",
                    ".ncf");
            }

            return Task.FromResult(string.Empty);
        }
    }
}
