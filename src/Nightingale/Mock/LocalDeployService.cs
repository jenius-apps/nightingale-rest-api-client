using Newtonsoft.Json;
using Nightingale.Core.Dialogs;
using Nightingale.Core.Helpers;
using Nightingale.Core.Extensions;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Mock.Models;
using Nightingale.Core.Mock.Services;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;

namespace Nightingale.Mock
{
    /// <summary>
    /// Class for deploying local mock server.
    /// </summary>
    public class LocalDeployService : IDeployService
    {
        private readonly IStorage _storage;
        private readonly IMessageBus _messageBus;
        private readonly IDialogService _dialogService;
        private readonly ICurrentWorkspaceContainer _workspaceContainer;
        private StorageFile serverConfigFile;

        public LocalDeployService(
            IStorage storage,
            IMessageBus messageBus,
            IDialogService dialogService,
            ICurrentWorkspaceContainer workspaceContainer)
        {
            _storage = storage
                ?? throw new ArgumentNullException(nameof(storage));
            _messageBus = messageBus
                ?? throw new ArgumentNullException(nameof(messageBus));
            _dialogService = dialogService
                ?? throw new ArgumentNullException(nameof(dialogService));
            _workspaceContainer = workspaceContainer
                ?? throw new ArgumentNullException(nameof(workspaceContainer));
        }

        private bool IsFullTrustAvailable => ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0);

        /// <inheritdoc/>
        public async Task<bool> DeployAsync(Item item = null)
        {
            Workspace workspace = _workspaceContainer.Get();
            var env = workspace.Environments?.GetActive();

            if (!IsFullTrustAvailable || workspace?.Id == null)
            {
                return false;
            }

            // Trigger save
            _messageBus.Publish(new Message("save"));

            if (serverConfigFile == null)
            {
                serverConfigFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "serverConfig.json",
                    CreationCollisionOption.ReplaceExisting);
            }

            if (serverConfigFile == null)
            {
                return false;
            }

            bool confirm = await _dialogService.MockServerDialogAsync(workspace.Name, item?.Name, env?.Name);
            if (!confirm)
            {
                return false;
            }

            var config = new ServerConfiguration
            {
                EnvironmentId = env?.Id,
                WorkspaceId = workspace.Id,
                ItemId = item?.Id,
                NcfPath = _storage.GetPathAndName()
            };

            await FileIO.WriteTextAsync(serverConfigFile, JsonConvert.SerializeObject(config));

            // Launch console window
            await Windows.ApplicationModel.FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync("ServerGroup");
            return true;
        }
    }
}
