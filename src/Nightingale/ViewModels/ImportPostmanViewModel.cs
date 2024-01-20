using JeniusApps.Nightingale.Converters.Curl;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Nightingale.Core.Exceptions;
using Nightingale.Core.ImportConverters.Nightingale;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Settings;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Handlers;
using Nightingale.Importers;
using Nightingale.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Nightingale.ViewModels
{
    public enum ImportType
    {
        Nightingale,
        Postman,
        Swagger,
        Insomnia,
        Curl,
        OData
    }

    public class ImportPostmanViewModel : ObservableObject
    {
        private readonly IFilePicker _filePicker;
        private readonly IPostmanImporter _postmanImporter;
        private readonly ISwaggerImporter _swaggerImporter;
        private readonly IInsomniaImporter _insomniaImporter;
        private readonly INcfImporter _ncfImporter;
        private readonly ICurlConverter _curlConverter;
        private readonly IODataImporter _odataImporter;
        private string _message = "";

        public ImportPostmanViewModel(
            IFilePicker filePicker,
            IPostmanImporter postmanImporter,
            IInsomniaImporter insomniaImporter,
            INcfImporter ncfImporter,
            ISwaggerImporter swaggerImporter,
            IODataImporter odataImporter,
            ICurlConverter curlConverter)
        {
            _odataImporter = odataImporter ?? throw new ArgumentNullException(nameof(odataImporter));
            _filePicker = filePicker ?? throw new ArgumentNullException(nameof(filePicker));
            _postmanImporter = postmanImporter ?? throw new ArgumentNullException(nameof(postmanImporter));
            _swaggerImporter = swaggerImporter ?? throw new ArgumentNullException(nameof(swaggerImporter));
            _insomniaImporter = insomniaImporter ?? throw new ArgumentNullException(nameof(insomniaImporter));
            _curlConverter = curlConverter ?? throw new ArgumentNullException(nameof(curlConverter));
            _ncfImporter = ncfImporter ?? throw new ArgumentNullException(nameof(ncfImporter));
        }

        public int ImportTypeSelected
        {
            get
            {
                int lastTypeUsed = UserSettings.Get<int>(SettingsConstants.LastImportTypeUsed);

                return Enum.IsDefined(typeof(ImportType), lastTypeUsed)
                    ? lastTypeUsed
                    : 0;
            }
            set
            {
                if (ImportTypeSelected != value && Enum.IsDefined(typeof(ImportType), value))
                {
                    UserSettings.Set<int>(SettingsConstants.LastImportTypeUsed, value);
                    OnPropertyChanged(nameof(CurlBoxVisible));
                    OnPropertyChanged(nameof(DragDropVislble));
                }
            }
        }

        public bool CurlBoxVisible => (ImportType)ImportTypeSelected == ImportType.Curl;

        public bool DragDropVislble => !CurlBoxVisible;

        public string CurlInput
        {
            get => _curlInput;
            set => SetProperty(ref _curlInput, value);
        }
        private string _curlInput;

        public string ErrorMessage
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public Item ConvertCurl()
        {
            if (string.IsNullOrWhiteSpace(CurlInput))
            {
                return null;
            }

            var dtoItem = _curlConverter.Convert(CurlInput);
            if (dtoItem == null)
            {
                Analytics.TrackEvent("Curl convert failed");
                AppendMessage("Curl convert failed. Pleae help us improve Nightingale by reporting the issue on our GitHub.");
                return null;
            }

            dtoItem.Name = "curl_import";
            Analytics.TrackEvent("Curl convert success");
            AppendMessage("Curl convert successful. New item will be waiting when you close the dialog.");
            var result = JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(dtoItem));

            if (result.Type == ItemType.None)
            {
                result.Type = ItemType.Request;
            }

            return result;
        }

        public async Task<(IList<Item>, IList<Workspace>)> DropFiles(
            IList<Windows.Storage.IStorageItem> storageItems)
        {
            ResetErrorMessage();
            if (storageItems == null || storageItems.Count() == 0)
            {
                return (null, null);
            }

            foreach (var item in storageItems)
            {
                StorageApplicationPermissions.FutureAccessList.Add(item);
            }

            return await ImportFilesAsync(storageItems.Select(x => x.Path).ToArray());
        }

        public async Task<(IList<Item>, IList<Workspace>)> SelectFilesAsync()
        {
            ResetErrorMessage();
            IList<string> paths = await _filePicker.PickFilesAsync();

            if (paths == null || paths.Count == 0)
            {
                return (null, null);
            }

            return await ImportFilesAsync(paths);
        }

        private async Task<(IList<Item>, IList<Workspace>)> ImportFilesAsync(
            IList<string> storageItemPaths)
        {
            if (storageItemPaths == null || storageItemPaths.Count == 0)
            {
                return (null, null);
            }

            var collectionResult = new List<Item>();
            var workspacesResult = new List<Workspace>();

            foreach (var filePath in storageItemPaths)
            {
                StorageFile cachedFile = await Common.CacheFileAsync(filePath);
                if (cachedFile == null)
                {
                    continue;
                }

                StorageApplicationPermissions.FutureAccessList.Add(cachedFile);
                IList<Item> importedCollections = new List<Item>();
                IList<Workspace> importedWorkspaces = new List<Workspace>();

                try
                {
                    switch ((ImportType)ImportTypeSelected)
                    {
                        case ImportType.Nightingale:
                            importedWorkspaces = await _ncfImporter.ImportFileAsync(cachedFile);
                            break;
                        case ImportType.Postman:
                            Item c1 = await _postmanImporter.ImportFileAsync(cachedFile);
                            if (c1 != null)
                            {
                                importedCollections.Add(c1);
                            }
                            break;
                        case ImportType.Swagger:
                            Item c2 = await _swaggerImporter.ImportFileAsync(cachedFile);
                            if (c2 != null)
                            {
                                importedCollections.Add(c2);
                            }
                            break;
                        case ImportType.Insomnia:
                            importedWorkspaces = await _insomniaImporter.ImportFileAsync(cachedFile);
                            break;
                        case ImportType.OData:
                            Item item = await _odataImporter.ImportFileAsync(cachedFile);
                            if (item != null)
                            {
                                importedCollections.Add(item);
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (UnsupportedFileException e)
                {
                    LogUnsupportedFileError(e.Message, e.ContentTypeOrVersion, e.FileName);
                    continue;
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e, new Dictionary<string, string>
                    {
                        { "message", e.Message },
                        { "Call stack", e.StackTrace }
                    });

                    AppendMessage($"Unknown import error. Please create new issue on GitHub! Error message: {e.Message}");
                    continue;
                }

                if (importedCollections != null && importedCollections.Count > 0)
                {
                    collectionResult.AddRange(importedCollections);
                }

                if (importedWorkspaces != null && importedWorkspaces.Count > 0)
                {
                    workspacesResult.AddRange(importedWorkspaces);
                }
            }

            if (collectionResult.Count > 0)
            {
                AppendMessage($"{collectionResult.Count} collection(s) ready for import after closing dialog!");
            }
            if (workspacesResult.Count > 0)
            {
                AppendMessage($"{workspacesResult.Count} workspace(s) ready for import after closing dialog!");
            }

            Analytics.TrackEvent("Import postman performed", new Dictionary<string, string>
            {
                { "files provided", storageItemPaths?.Count.ToString() ?? "0" },
                { "collections imported", collectionResult?.Count.ToString() ?? "0" },
                { "workspaces imported", workspacesResult?.Count.ToString() ?? "0" }
            });

            return (collectionResult, workspacesResult);
        }

        private void AppendErrorMessage(string message, string contentType, string filename)
        {
            ErrorMessage += $"{message} | {(string.IsNullOrWhiteSpace(contentType) ? "unknown file type" : contentType)} | {filename}" 
                            + System.Environment.NewLine;
        }

        private void ResetErrorMessage() => ErrorMessage = "";

        private void AppendMessage(string message)
        {
            ErrorMessage += message + System.Environment.NewLine;
        }

        private void LogUnsupportedFileError(string message, string contentType, string filename)
        {
            Analytics.TrackEvent("Import file unsupported", new Dictionary<string, string>
            {
                { "message", message },
                { "content type", contentType },
            });
            AppendErrorMessage(message, contentType, filename);
        }

        public void LogConverterLinkClicked()
        {
            Analytics.TrackEvent(Telemetry.Docs, Telemetry.DocsTelemetryProps(Telemetry.ConverterSource));
        }
    }
}
