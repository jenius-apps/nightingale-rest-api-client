using Nightingale.Core.Extensions;
using Nightingale.Core.Models;
using Nightingale.Handlers;
using Nightingale.VisualState;
using Nightingale.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Nightingale.ViewModels.Factories;
using Nightingale.Views;
using Environment = Nightingale.Core.Models.Environment;
using Nightingale.Core.Settings;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using Nightingale.CustomEventArgs;
using Nightingale.Dialogs;
using System.Linq;
using System;
using Nightingale.Tabs.Services;
using Nightingale.Utilities;
using System.Collections.Generic;
using Nightingale.Core.Mock.Services;
using Nightingale.Core.Export;
using Microsoft.AppCenter.Crashes;
using JeniusApps.Common.Telemetry;

namespace Nightingale.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{
    private readonly IWorkspaceTreeModifier _workspaceModifier;
    private readonly IEnvironmentDialogService _environmentDialogService;
    private readonly IVisualStatePublisher _visualStatePublisher;
    private readonly IWorkspaceItemNavigationService _workspaceItemNavigationService;
    private readonly ICodeGeneratorViewModelFactory _codeGeneratorViewModelFactory;
    private readonly ITabCollectionContainer _tabContainer;
    private readonly IDeployService _deployService;
    private readonly KbShortcutsHandler _keyboardShortcutHandler;
    private readonly IExportService _exportService;
    private readonly IUserSettings _userSettings;
    private readonly ITelemetry _telemetry;
    private Workspace _selectedWorkspace;
    private bool _exportFlyoutVisible;

    public WorkspaceViewModel(
        IWorkspaceTreeModifier workspaceModifier,
        IVisualStatePublisher visualStatePublisher,
        IEnvironmentDialogService environmentDialogFactory,
        IWorkspaceItemNavigationService workspaceItemNavigationService,
        ICodeGeneratorViewModelFactory codeGeneratorViewModelFactory,
        ITabCollectionContainer tabContainer,
        KbShortcutsHandler keyboardShortcutHandler,
        IDeployService deployService,
        IExportService exportService,
        IUserSettings userSettings,
        ITelemetry telemetry)
    {
        _exportService = exportService;
        _workspaceModifier = workspaceModifier;
        _visualStatePublisher = visualStatePublisher;
        _environmentDialogService = environmentDialogFactory;
        _workspaceItemNavigationService = workspaceItemNavigationService;
        _codeGeneratorViewModelFactory = codeGeneratorViewModelFactory;
        _tabContainer = tabContainer;
        _deployService = deployService;
        _keyboardShortcutHandler = keyboardShortcutHandler;
        _userSettings = userSettings;
        _telemetry = telemetry;

        _visualStatePublisher.SideBarVisibilityChanged -= _visualStatePublisher_SideBarVisibilityChanged;
        _visualStatePublisher.SideBarVisibilityChanged += _visualStatePublisher_SideBarVisibilityChanged;

        _keyboardShortcutHandler.RenameRequested += OnRenameRequested;
    }

    private async void OnRenameRequested(object sender, EventArgs e)
    {
        await _workspaceModifier.EditItemAsync(_tabContainer.CurrentTab?.ViewModel?.Request);
    }

    public void TabChangedHandler(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(SelectedTab));
    }

    public Workspace SelectedWorkspace
    {
        get => _selectedWorkspace;
        set
        {
            _selectedWorkspace = value;
            RaisePropertyChanged(string.Empty);
        }
    }

    public bool IsSideBarVisible
    {
        get => _userSettings.Get<bool>(SettingsConstants.SideBarVisible);
        set
        {
            if (IsSideBarVisible != value)
            {
                _visualStatePublisher.SetSideBarVisibility(value);
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsSideBarHidden));
            }
        }
    }

    public bool IsSideBarHidden => !IsSideBarVisible;

    public ObservableCollection<Item> Items
    {
        get => SelectedWorkspace?.Items;
    }

    /// <summary>
    /// The tabs that are displayed in the UI.
    /// </summary>
    public ObservableCollection<RequestViewModel> Tabs
    {
        get => _tabs;
        set
        {
            if (_tabs != null)
            {
                _tabs.CollectionChanged -= TabsCollectionChanged;
            }

            _tabs = value;

            if (_tabs != null)
            {
                _tabs.CollectionChanged += TabsCollectionChanged;
            }
        }
    }

    private void TabsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaisePropertyChanged(nameof(WelcomeScreenVisible));
    }

    private ObservableCollection<RequestViewModel> _tabs;

    public RequestViewModel SelectedTab
    {
        get => _tabContainer.CurrentTab;
        set
        {
            if (_tabContainer.CurrentTab != value)
            {
                _tabContainer.CurrentTab = value;
                RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Returns true if WelcomeScreen should be visible.
    /// </summary>
    public bool WelcomeScreenVisible
    {
        get => (Tabs == null || Tabs.Count == 0) && !FrameVisible;
    }

    public ObservableCollection<Environment> WorkspaceEnvironments
    {
        get => SelectedWorkspace?.Environments;
    }

    public Environment SelectedEnv
    {
        get => WorkspaceEnvironments?.GetActive() ?? null;
        set
        {
            if (value != SelectedEnv)
            {
                WorkspaceEnvironments?.SetActive(value);
                RaisePropertyChanged("SelectedEnv");
            }
        }
    }

    public bool FrameVisible
    {
        get => _frameVisible;
        set
        {
            if (_frameVisible != value)
            {
                _frameVisible = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(TabsVisible));
                RaisePropertyChanged(nameof(WelcomeScreenVisible));
            }
        }
    }
    private bool _frameVisible;

    public bool ExportFlyoutVisible
    {
        get => _exportFlyoutVisible;
        set
        {
            _exportFlyoutVisible = value;
            RaisePropertyChanged();
        }
    }

    public bool TabsVisible
    {
        get => !FrameVisible;
    }

    public GridLength UserSidebarWidth
    {
        get
        {
            double widthValue = IsSideBarVisible ? _userSettings.Get<double>(SettingsConstants.SidebarWidth) : 0d;
            return new GridLength(widthValue, GridUnitType.Pixel);
        }
    }

    public void HideFrame()
    {
        FrameVisible = false;
    }

    public void AddTempTab()
    {
        _tabContainer.AddTempTab();
        _telemetry.TrackEvent(Telemetry.TempTabAdded);
    }

    public async Task SaveTempItemAsync(Item tabItem)
    {
        if (tabItem is null)
        {
            return;
        }

        var item = await _workspaceModifier.InsertToWorkspaceAsync(tabItem);

        if (item != null)
        {
            _telemetry.TrackEvent(Telemetry.TempTabSaved);
        }
    }

    public async void InvokeItem(object sender, AddedItemArgs<object> e)
    {
        if (e.AddedItem is Item t && t.Type == ItemType.Collection)
        {
            if (t != SelectedWorkspace.CurrentItem)
            {
                SelectedWorkspace.CurrentItem = t;
                await NavigateToCurrentItem();
                await Task.Delay(100);
            }

            FrameVisible = true;
        }
        else if (e.AddedItem is Item item && item.Type == ItemType.Request)
        {
            FrameVisible = false;

            if (item == SelectedTab?.ViewModel?.Request)
            {
                // Fast path when item invoked is already
                // the selected item.
                return;
            }

            RequestViewModel tab = Tabs
                .Where(x => x.ViewModel.Request == item)
                .FirstOrDefault();

            if (tab != null)
            {
                SelectedTab = tab;
                _telemetry.TrackEvent(Telemetry.OpenTreeItemTabFocused);
            }
            else
            {
                await _tabContainer.AddTabAsync(item);
                _telemetry.TrackEvent(Telemetry.TreeItemTabOpened);
            }
        }
        else if (e.AddedItem is null)
        {
            await NavigateToEmptyState();
        }
    }

    public async void NewRequest()
    {
        await _workspaceModifier.NewItemAsync(ItemType.Request, true);
    }

    public async void NewCollection()
    {
        await _workspaceModifier.NewItemAsync(ItemType.Collection, true);
    }

    public async void CloneWorkspaceItem(object sender, AddedItemArgs<object> e)
    {
        if (e.AddedItem is Item item)
        {
            Item newItem = await _workspaceModifier.CloneItemAsync(item);

            if (newItem != null)
            {
                _telemetry.TrackEvent(Telemetry.ItemCloned);

                // TODO https://github.com/jenius-apps/nightingale-rest-api-client/issues/32
                //SelectedWorkspace.CurrentItem = newItem;
                //await _workspaceItemNavigationService.NavigateTo(newItem);
            }
        }
    }

    public async void EditTreeItem(object sender, AddedItemArgs<object> e)
    {
        if (e.AddedItem is Item item)
        {
            var success = await _workspaceModifier.EditItemAsync(item);
            if (success)
            {
                _telemetry.TrackEvent(Telemetry.ItemEdited);
            }
        }
    }

    public async void AddCollectionToParent(object sender, AddedItemArgs<object> e)
    {
        if (e.AddedItem is Item parent)
        {
            var newItem = await _workspaceModifier.NewItemAsync(ItemType.Collection, false, parent);
            if (newItem != null)
            {
                _telemetry.TrackEvent(Telemetry.NewColInParent);
            }
        }
    }

    public async void DeployMockServer(object sender, AddedItemArgs<object> e)
    {
        if (e.AddedItem is Item item)
        {
            bool deploymentSuccessful = await _deployService.DeployAsync(item);
            _telemetry.TrackEvent(Telemetry.MockServerDeployed, Telemetry.MockTelemetryProps(deploymentSuccessful, "treeContextMenu"));
        }
    }


    public async void AddRequestToParent(object sender, AddedItemArgs<object> e)
    {
        if (e.AddedItem is Item parent)
        {
            var newItem = await _workspaceModifier.NewItemAsync(ItemType.Request, false, parent);
            if (newItem != null)
            {
                _telemetry.TrackEvent(Telemetry.NewReqInParent);
            }
        }
    }

    public async void AddRequestToRoot()
    {
        if (SelectedWorkspace == null)
        {
            return;
        }

        var item = await _workspaceModifier.NewItemAsync(ItemType.Request, true);
        if (item != null)
        {
            _telemetry.TrackEvent(Telemetry.NewReqInRoot);
        }
    }

    public async void OnTreeViewExportClicked(object sender, ExportConfiguration config)
    {
        if (config is null)
        {
            return;
        }

        try
        {
            var exportedPath = await _exportService.ExportCollectionAsync(
                config.CollectionToExport,
                config.Format);

            if (!string.IsNullOrWhiteSpace(exportedPath))
            {
                _telemetry.TrackEvent(Telemetry.ContextMenuExportClicked);
                ExportFlyoutVisible = true;
            }
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
        }
    }


    public async void AddCollectionToRoot()
    {
        if (SelectedWorkspace == null)
        {
            return;
        }

        var item = await _workspaceModifier.NewItemAsync(ItemType.Collection, true);
        if (item != null)
        {
            _telemetry.TrackEvent(Telemetry.NewColInRoot);
            // TODO https://github.com/jenius-apps/nightingale-rest-api-client/issues/32
            //SelectedWorkspace.CurrentItem = item;
            //await _workspaceItemNavigationService.NavigateTo(item);
        }
    }

    public async void DeleteAll()
    {
        if (SelectedWorkspace == null)
        {
            return;
        }

        bool success = await _workspaceModifier.DeleteAllItemsAsync();
        if (success)
        {
            _telemetry.TrackEvent(Telemetry.DeleteAll);

            if (SelectedWorkspace.CurrentItem != null)
            {
                SelectedWorkspace.CurrentItem = null;
                await NavigateToEmptyState();
            }
        }
    }

    public async void DeleteTreeItem(object sender, AddedItemArgs<object> e)
    {
        if (SelectedWorkspace == null)
        {
            return;
        }

        if (e.AddedItem is Item deletedItem)
        {
            bool success = await _workspaceModifier.DeleteItemAsync(deletedItem);

            if (success)
            {
                _telemetry.TrackEvent(Telemetry.ItemDeleted);
                _tabContainer.RemoveTab(deletedItem);
            }
        }
    }

    public void SetWorkspaceItemFrame(Windows.UI.Xaml.Controls.Frame frame)
    {
        _workspaceItemNavigationService.SetFrame(frame);
    }

    public async Task NavigateToEmptyState()
    {
        await _workspaceItemNavigationService.NavigateTo(null);
    }

    /// <summary>
    /// Used in workspacepage.xaml.cs to navigate
    /// to a history item.
    /// </summary>
    public async Task NavigateToCurrentItem()
    {
        await _workspaceItemNavigationService.NavigateTo(SelectedWorkspace?.CurrentItem);
    }

    public void GenerateCode(object sender, AddedItemArgs<object> e)
    {
        if (e.AddedItem is Item item)
        {
            // use factory to generate viewmodel
            var vm = _codeGeneratorViewModelFactory.CreateViewModel(item);

            // launch new window for code generation
            // _extensionViewService.LaunchCodeGeneratorView(vm);
            App.NewWindow(typeof(CodeGeneratorPage), vm);
            _telemetry.TrackEvent(Telemetry.GenCode);
        }
    }

    public void ToggleSideBar()
    {
        IsSideBarVisible = !IsSideBarVisible;

        _telemetry.TrackEvent(Telemetry.ToggleSideBarClicked, new Dictionary<string, string>
        {
            { "IsSideBarVisible", IsSideBarVisible.ToString() }
        });
    }

    public void SetTempSinglePane(bool value)
    {
        _visualStatePublisher.SetTempSinglePane(value);
    }

    private void _visualStatePublisher_SideBarVisibilityChanged(object sender, System.EventArgs e)
    {
        RaisePropertyChanged(nameof(IsSideBarVisible));
        RaisePropertyChanged(nameof(IsSideBarHidden));
        RaisePropertyChanged(nameof(UserSidebarWidth));
    }

    public async void OpenEnvironmentManagerDialog()
    {
        if (SelectedWorkspace == null)
        {
            return;
        }

        await _environmentDialogService.OpenEnvironmentDialog(SelectedWorkspace.Environments);
        _telemetry.TrackEvent("Environment dialog opened");

        // Ensure the combobox is selecting the env.
        RaisePropertyChanged("SelectedEnv");
    }
}
