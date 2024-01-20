using Microsoft.AppCenter.Analytics;
using Nightingale.Core.Cookies;
using Nightingale.Core.Dialogs;
using Nightingale.Core.Helpers;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Http;
using Nightingale.Core.Mock.Services;
using Nightingale.Core.Models;
using Nightingale.Core.Settings;
using Nightingale.Core.Storage.Interfaces;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using Nightingale.Dialogs;
using Nightingale.Handlers;
using Nightingale.Navigation;
using Nightingale.Tabs.Services;
using Nightingale.Utilities;
using Nightingale.VisualState;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using System.Threading;
using Nightingale.Core.Interfaces;
using Microsoft.AppCenter.Crashes;
using Nightingale.Core.Export;
using System.IO;

namespace Nightingale.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly Timer _saveTimer;

        private readonly IWorkspaceStorageAccessor _workspaceStorageAccessor;
        private readonly IWorkspaceListModifier _workspaceListModifier;
        private readonly ICurrentWorkspaceContainer _workspaceContainer;
        private readonly IEnvironmentContainer _environmentContainer;
        private readonly ICookieJar _cookieJar;
        private readonly IWorkspaceNavigationService _workspaceNavigationService;
        private readonly IWorkspaceItemNavigationService _workspaceItemNavigationService;
        private readonly ICookieDialogService _cookieDialogService;
        private readonly IDialogService _dialogService;
        private readonly IMethodsContainer _methodsContainer;
        private readonly IWorkspaceTreeModifier _workspaceTreeModifier;
        private readonly IVisualStatePublisher _visualStatePublisher;
        private readonly IEnvironmentDialogService _environmentDialogService;
        private readonly IMessageBus _messageBus;
        private readonly ITabCollectionContainer _tabContainer;
        private readonly IDeployService _deployService;
        private readonly IStorage _storage;
        private readonly IExportService _exportService;
        private readonly string _workspaceRootId = "root";
        private Workspace _selectedWorkspace;
        private bool _saving;
        private bool _savingAs;
        private bool _rateButtonVisible;
        private bool _loading;
        private string _activeFileName;

        public MainPageViewModel(
            IVisualStatePublisher visualStatePublisher,
            IWorkspaceStorageAccessor workspaceStorageAccessor,
            IWorkspaceListModifier workspaceListModifier,
            ICurrentWorkspaceContainer workspaceContainer,
            IEnvironmentContainer environmentContainer,
            IWorkspaceNavigationService workspaceNavigationService,
            IWorkspaceItemNavigationService workspaceItemNavigationService,
            ICookieJar cookieJar,
            ICookieDialogService cookieDialogService,
            IDialogService dialogService,
            IMethodsContainer methodsContainer,
            IWorkspaceTreeModifier workspaceTreeModifier,
            IEnvironmentDialogService environmentDialogService,
            IMessageBus messageBus,
            ITabCollectionContainer tabContainer,
            IDeployService deployService,
            MvpViewModel mvpViewModel,
            IStorage storage,
            IExportService exportService)
        {
            MvpViewModel = mvpViewModel ?? throw new ArgumentNullException(nameof(mvpViewModel));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _workspaceStorageAccessor = workspaceStorageAccessor ?? throw new ArgumentNullException(nameof(workspaceStorageAccessor));
            _workspaceListModifier = workspaceListModifier ?? throw new ArgumentNullException(nameof(workspaceListModifier));
            _workspaceContainer = workspaceContainer ?? throw new ArgumentNullException(nameof(workspaceContainer));
            _environmentContainer = environmentContainer ?? throw new ArgumentNullException(nameof(environmentContainer));
            _cookieJar = cookieJar ?? throw new ArgumentNullException(nameof(cookieJar));
            _workspaceNavigationService = workspaceNavigationService ?? throw new ArgumentNullException(nameof(workspaceNavigationService));
            _cookieDialogService = cookieDialogService ?? throw new ArgumentNullException(nameof(cookieDialogService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _methodsContainer = methodsContainer ?? throw new ArgumentNullException(nameof(methodsContainer));
            _workspaceTreeModifier = workspaceTreeModifier ?? throw new ArgumentNullException(nameof(workspaceTreeModifier));
            _visualStatePublisher = visualStatePublisher ?? throw new ArgumentNullException(nameof(visualStatePublisher));
            _environmentDialogService = environmentDialogService ?? throw new ArgumentNullException(nameof(environmentDialogService));
            _tabContainer = tabContainer ?? throw new ArgumentNullException(nameof(tabContainer));
            _deployService = deployService ?? throw new ArgumentNullException(nameof(deployService));
            _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));

            this._workspaceItemNavigationService = workspaceItemNavigationService ?? throw new ArgumentNullException(nameof(workspaceItemNavigationService));
            UpdateRateButtonVisibility();

            // Subscribe to close requested event.
            Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += MainPageViewModel_CloseRequested;

            // Subscribe to message published event for localhost help.
            _messageBus.MessagePublished += MessageBus_MessagePublished;

            // Subscribe to app-wide keydown for Ctrl+S purposes.
            CoreWindow.GetForCurrentThread().KeyDown += CoreWindowKeyDown;

            _visualStatePublisher.PaneLayoutToggled += PaneLayoutChanged;

            if (UserSettings.Get<bool>(SettingsConstants.AutoSaveInterval))
            {
                _saveTimer = new Timer(TimerCallback, null, 300000 /* 5 minutes */, 300000);
            }
        }

        private async void TimerCallback(object state)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SaveWorkspace();
            });
        }

        public bool SuccessfulImportFlyoutVisible
        {
            get => _successfulImportFlyoutVisible;
            set
            {
                if (value != _successfulImportFlyoutVisible)
                {
                    _successfulImportFlyoutVisible = value;
                    RaisePropertyChanged();
                }
            }
        }
        private bool _successfulImportFlyoutVisible;

        public MvpViewModel MvpViewModel { get; }

        private void PaneLayoutChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(IsSinglePaneLayout));
            RaisePropertyChanged(nameof(IsTwoPaneLayout));
        }

        private async void CoreWindowKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (KeyboardState.IsCtrlKeyPressed() && args.VirtualKey == VirtualKey.S)
            {
                args.Handled = true;
                await SaveWorkspaceAsync();
                Analytics.TrackEvent(Telemetry.CtrlS);
            }
        }

        private async void MessageBus_MessagePublished(object sender, MessageArgs e)
        {
            if (e.Message.StringContent == "save")
            {
                await SaveWorkspaceAsync();
            }
        }

        private async void MainPageViewModel_CloseRequested(object sender, Windows.UI.Core.Preview.SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var deferral = e.GetDeferral();

            if (UserSettings.Get<bool>(SettingsConstants.AutoSaveEnabled))
            {
                Saving = true;
                await _workspaceStorageAccessor.SaveWorkspacesAsync(Workspaces, _workspaceRootId, SelectedWorkspace?.Id);
                Saving = false;
            }

            deferral.Complete();
        }

        public ObservableCollection<Workspace> Workspaces { get; } = new ObservableCollection<Workspace>();

        public string ActiveFileName
        {
            get => string.IsNullOrWhiteSpace(_activeFileName) ? "Nightingale" : _activeFileName;
            set
            {
                _activeFileName = value;
                RaisePropertyChanged("ActiveFileName");
            }
        }

        public bool RateButtonVisible
        {
            get => _rateButtonVisible;
            set
            {
                _rateButtonVisible = value;
                RaisePropertyChanged("RateButtonVisible");
            }
        }

        public bool Loading
        {
            get => _loading;
            set
            {
                if (_loading != value)
                {
                    _loading = value;
                    RaisePropertyChanged("Loading");
                }
            }
        }

        public bool IsWorkspaceListEmpty
        {
            get => Workspaces.Count == 0;
        }

        public async Task LoadAsync()
        {
            Loading = true;
            this.Workspaces.Clear();
            _workspaceListModifier.SetList(this.Workspaces);

            IList<Workspace> workspacesFromStorage = await _workspaceStorageAccessor.GetWorkspacesAsync(_workspaceRootId);

            foreach (Workspace w in workspacesFromStorage)
            {
                this.Workspaces.Add(w);
            }

            if (this.Workspaces.Count == 0)
            {
                // Add default workspace
                await _workspaceListModifier.NewWorkspaceAsync("My workspace");
            }

            string activeWorkspace = _storage.GetActiveWorkspaceId();
            SelectedWorkspace = !string.IsNullOrWhiteSpace(activeWorkspace)
                ? this.Workspaces.FirstOrDefault(x => x.Id == activeWorkspace) ?? this.Workspaces.FirstOrDefault()
                : this.Workspaces.FirstOrDefault();

            Loading = false;
        }

        public bool Saving
        {
            get => _saving;
            set
            {
                _saving = value;
                RaisePropertyChanged("Saving");
                RaisePropertyChanged("SaveButtonVisible");
            }
        }

        public bool IsSinglePaneLayout => !IsTwoPaneLayout;

        public bool IsTwoPaneLayout
        {
            get => _visualStatePublisher.IsLayoutTwoPane();
            set
            {
                if (IsTwoPaneLayout != value)
                {
                    _visualStatePublisher.SetPaneLayoutSideBySide(value);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSinglePaneLayout));
                }
            }
        }


        public bool SaveButtonVisible
        {
            get => !Saving;
        }

        public string WorkspaceName
        {
            get => SelectedWorkspace?.Name ?? "None";
        }

        /// <summary>
        /// Flag for whether or not this instance is running
        /// with Full Trust component available.
        /// </summary>
        public bool IsFullTrustAvailable => ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0);

        public Workspace SelectedWorkspace
        {
            get => _selectedWorkspace;
            set
            {
                if (_selectedWorkspace == value)
                {
                    return;
                }

                _selectedWorkspace = value;
                _environmentContainer.SetEnvironmentList(_selectedWorkspace?.Environments);
                _cookieJar.SetCookies(_selectedWorkspace?.WorkspaceCookies);
                _methodsContainer.SetMethods(_selectedWorkspace);
                _workspaceContainer.Set(value);
                _workspaceTreeModifier.SetWorkspace(value);
                RaisePropertyChanged("SelectedWorkspace");
                RaisePropertyChanged("IsWorkspaceListEmpty");
                RaisePropertyChanged("WorkspaceName");
            }
        }

        public async void DeployServer()
        {
            var deploymentSuccessful = await _deployService.DeployAsync();
            Analytics.TrackEvent(Telemetry.MockServerDeployed, Telemetry.MockTelemetryProps(deploymentSuccessful, "ribbon"));
        }

        public async void SaveAs()
        {
            if (Workspaces == null || Saving || _saving)
            {
                return;
            }

            _savingAs = true;
            await _workspaceStorageAccessor.SaveWorkspacesAsync(Workspaces, _workspaceRootId, SelectedWorkspace?.Id, false);
            string pathToExportFile = await _exportService.ExportAsync(Workspaces);
            bool exportSuccessful = !string.IsNullOrWhiteSpace(pathToExportFile);
            ExportFlyoutVisible = exportSuccessful;
            _lastExportedFilePath = pathToExportFile;

            Analytics.TrackEvent(Telemetry.MenuExportClicked, new Dictionary<string, string>
            {
                { "Export performed", exportSuccessful.ToString() }
            });


            _savingAs = false;
        }

        private string _lastExportedFilePath;

        public bool ExportFlyoutVisible
        {
            get => _exportFlyoutVisible;
            set
            {
                if (_exportFlyoutVisible != value)
                {
                    _exportFlyoutVisible = value;
                    RaisePropertyChanged();
                }
            }
        }
        private bool _exportFlyoutVisible;

        public async void OpenExportLocation()
        {
            if (_lastExportedFilePath != null)
            {
                try
                {
                    var parentFolder = _lastExportedFilePath.Replace(Path.GetFileName(_lastExportedFilePath), "");
                    await Launcher.LaunchFolderPathAsync(parentFolder);
                    Analytics.TrackEvent(Telemetry.ExportLocationOpened);
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e, new Dictionary<string, string>
                    {
                        { "filepath", _lastExportedFilePath }
                    });
                }
            }
        }

        public async Task SaveWorkspaceAsync()
        {
            if (Workspaces == null || Saving || _savingAs)
            {
                return;
            }

            Saving = true;
            Task saveTask = _workspaceStorageAccessor.SaveWorkspacesAsync(Workspaces, _workspaceRootId, SelectedWorkspace?.Id);
            Task deleyTask = Task.Delay(300);
            await saveTask;
            // Ensures at least a 300ms animation
            // so customers see the save animation
            // so they know the save actually happened.
            await deleyTask; 
            Saving = false;
        }

        public async void SaveWorkspace()
        {
            if (Workspaces == null || Saving || _savingAs)
            {
                return;
            }

            await SaveWorkspaceAsync();
            Analytics.TrackEvent(Telemetry.MenuSaveClicked, new Dictionary<string, string> 
            {
                { "Workspace count", Workspaces.Count.ToString() }
            });
        }

        public async void OpenCookiesDialog()
        {
            await _cookieDialogService.OpenCookieDialog();
            Analytics.TrackEvent(Telemetry.MenuCookiesClicked);
        }

        public async void NewWorkspace()
        {
            Workspace newWorkspace = await _workspaceListModifier.NewWorkspaceAsync();

            Analytics.TrackEvent(Telemetry.MenuNewWorkspaceClicked, new Dictionary<string, string>
            {
                { "workspace added", newWorkspace == null ? "false" : "true" }
            });

            if (newWorkspace != null)
            {
                SelectedWorkspace = newWorkspace;
            }
        }

        public async void NewRequest()
        {
            if (SelectedWorkspace == null)
            {
                return;
            }

            Item newItem = await _workspaceTreeModifier.NewItemAsync(ItemType.Request, true);

            if (newItem != null)
            {
                Analytics.TrackEvent(Telemetry.MenuNewRequestAdded, new Dictionary<string, string>
                {
                    { "parent", newItem.Parent == null ? "root" : "item" }
                });

                // This updates the selected node of the treeview
                //SelectedWorkspace.CurrentItem = newItem;
                //await _workspaceItemNavigationService.NavigateTo(newItem);
            }
        }

        public async void NewCollection()
        {
            if (SelectedWorkspace == null)
            {
                return;
            }

            Item newItem = await _workspaceTreeModifier.NewItemAsync(ItemType.Collection, true);

            if (newItem != null)
            {
                Analytics.TrackEvent(Telemetry.MenuNewCollectionAdded, new Dictionary<string, string>
                {
                    { "parent", newItem.Parent == null ? "root" : "item" }
                });

                // This updates the selected node of the treeview
                //SelectedWorkspace.CurrentItem = newItem;
                //await _workspaceItemNavigationService.NavigateTo(newItem);
            }
        }

        public async Task DeleteWorkspaceAsync(Workspace forDeletion)
        {
            if (forDeletion == null)
            {
                return;
            }

            bool success = await _workspaceListModifier.DeleteWorkspaceAsync(forDeletion);

            if (success)
            {
                // Update UI if list is empty
                RaisePropertyChanged("IsWorkspaceListEmpty");
                Analytics.TrackEvent(Telemetry.MenuWorkspaceDeleted);
            }
        }

        public async Task RenameWorkspaceAsync(Workspace workspace)
        {
            if (workspace == null)
            {
                return;
            }

            bool success = await _workspaceListModifier.EditWorkspaceAsync(workspace);

            if (success)
            {
                Analytics.TrackEvent(Telemetry.MenuWorkspaceEdited);
                RaisePropertyChanged("WorkspaceName");
            }
        }


        public void Navigate()
        {
            if (Workspaces.Count < 1)
            {
                _workspaceNavigationService.NavigateTo(null);
                return;
            }

            _workspaceNavigationService.NavigateTo(SelectedWorkspace);
        }

        public async void SaveCurrentTab()
        {
            var currentTab = _tabContainer.CurrentTab;
            if (currentTab == null)
            {
                return;
            }

            var item = await _workspaceTreeModifier.InsertToWorkspaceAsync(currentTab.ViewModel?.Request);
            if (item != null)
            {
                Analytics.TrackEvent(Telemetry.MenuSaveTab);
            }
        }

        public async void OpenImportDialog()
        {
            (var importedCollections, var importedWorkspaces) = await _dialogService.ImportDialogAsync();

            if (importedCollections != null && importedCollections.Count > 0)
            {
                foreach (var importedCollection in importedCollections)
                {
                    await _workspaceTreeModifier.AddItemAsync(importedCollection);
                }
            }

            if (importedWorkspaces != null && importedWorkspaces.Count > 0)
            {
                foreach (var workspace in importedWorkspaces)
                {
                    await _workspaceListModifier.AddWorkspaceAsync(workspace);
                }

                SuccessfulImportFlyoutVisible = true;
            }

            // In case workspace list was empty previous to importing,
            // this will remove the empty placeholder.
            RaisePropertyChanged(nameof(IsWorkspaceListEmpty));
            Analytics.TrackEvent(Telemetry.MenuImportClicked, new Dictionary<string, string>
            {
                { "imported collection count", importedCollections?.Count.ToString() ?? "0" },
                { "imported workspace count", importedWorkspaces?.Count.ToString() ?? "0" }
            });
        }

        public async void OpenMvp()
        {
            await _dialogService.OpenMvpAsync();
        }

        public async void OpenSettingsDialog()
        {
            await _dialogService.OpenSettingsAsync();
            Analytics.TrackEvent(Telemetry.MenuSettingsClicked);
        }

        public async void EmailDev()
        {
            Analytics.TrackEvent(Telemetry.EmailDev);
            await Launcher.LaunchUriAsync(new Uri("mailto:nightingale_app@outlook.com"));
        }

        public async void OpenLocalHostTroubleShoot()
        {
            Analytics.TrackEvent(Telemetry.LocalhostHelp);
            await Launcher.LaunchUriAsync(new Uri("https://github.com/jenius-apps/nightingale-rest-api-client/blob/master/docs/localhost.md"));
        }

        public async void OpenKnownBugs()
        {
            Analytics.TrackEvent(Telemetry.KnownBugs);
            await Launcher.LaunchUriAsync(new Uri("https://github.com/jenius-apps/nightingale-rest-api-client/issues?q=is%3Aopen+is%3Aissue+label%3Abug"));
        }

        public async void NewGitHubIssue()
        {
            Analytics.TrackEvent(Telemetry.GitHubIssue);
            await Launcher.LaunchUriAsync(new Uri("https://github.com/jenius-apps/nightingale-rest-api-client/issues/new"));
        }

        public async void OpenEnvManager()
        {
            if (SelectedWorkspace == null)
            {
                return;
            }

            await _environmentDialogService.OpenEnvironmentDialog(SelectedWorkspace.Environments);
            Analytics.TrackEvent(Telemetry.MenuEnvManager);
        }

        public void SinglePaneLayout()
        {
            _visualStatePublisher.SetPaneLayoutSideBySide(false);
            Analytics.TrackEvent(Telemetry.SinglePane, new Dictionary<string, string>
            {
                { "state", "single pane" }
            });
        }

        public void TwoPaneLayout()
        {
            _visualStatePublisher.SetPaneLayoutSideBySide(true);
            Analytics.TrackEvent(Telemetry.TwoPane, new Dictionary<string, string>
            {
                { "state", "two pane" }
            });
        }

        private async void UpdateRateButtonVisibility()
        {
            bool displayRateButton = await Task.Run(() =>
            {
                bool IsAlreadyRated = UserSettings.Get<bool>(SettingsConstants.IsAppRated);
                return IsAlreadyRated ? false : new Random().Next(2) == 0;
            });

            RateButtonVisible = displayRateButton;
        }
    }
}
