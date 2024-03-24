using CommunityToolkit.Mvvm.Input;
using JeniusApps.Common.Telemetry;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using Nightingale.Tabs.Services;
using Nightingale.Utilities;
using Nightingale.Views.NavigationParameters;
using System;

namespace Nightingale.ViewModels;

public class RequestViewModel : ObservableBase
{
    private readonly IWorkspaceTreeModifier _workspaceTreeModifier;
    private readonly ITabCollectionContainer _tabCollectionContainer;
    private readonly ITelemetry _telemetry;

    public IRelayCommand<Item> CloseTabCommand { get; }

    public IRelayCommand<Item> CloseOthersCommand { get; }

    public IRelayCommand<Item> SaveTabCommand { get; }

    public IRelayCommand<Item> DuplicateTabCommand { get; }

    public IRelayCommand<Item> RenameTabCommand { get; }

    public RequestViewModel(
        RequestPageParameters p,
        IWorkspaceTreeModifier workspaceListModifier,
        ITabCollectionContainer tabCollectionContainer,
        ITelemetry telemetry)
    {
        _workspaceTreeModifier = workspaceListModifier;
        _tabCollectionContainer = tabCollectionContainer;
        _telemetry = telemetry;

        PageViewModel = p.RequestPageViewModel ?? throw new ArgumentNullException(nameof(p.RequestPageViewModel));
        ViewModel = p.RequestControlViewModel ?? throw new ArgumentNullException(nameof(p.RequestControlViewModel));
        UrlBarViewModel = p.UrlBarViewModel ?? throw new ArgumentNullException(nameof(p.UrlBarViewModel));
        AuthControlViewModel = p.AuthControlViewModel ?? throw new ArgumentNullException(nameof(p.AuthControlViewModel));
        RequestBodyViewModel = p.RequestBodyViewModel ?? throw new ArgumentNullException(nameof(p.RequestBodyViewModel));
        BodyControlViewModel = p.BodyControlViewModel ?? throw new ArgumentNullException(nameof(p.BodyControlViewModel));
        StatusBarViewModel = p.StatusBarViewModel ?? throw new ArgumentNullException(nameof(p.StatusBarViewModel));
        ViewModel.ResponseReceived += (s, a) => BodyControlViewModel.WorkspaceResponse = ViewModel.Response;
        ViewModel.ResponseReceived += (s, a) => StatusBarViewModel.ResponseStatus = ViewModel.Response;

        SaveTabCommand = new RelayCommand<Item>((x) => SaveToWorkspace(x));
        DuplicateTabCommand = new RelayCommand<Item>((x) => DuplicateTab(x));
        RenameTabCommand = new RelayCommand<Item>((x) => RenameTab(x));
        CloseTabCommand = new RelayCommand<Item>(x => CloseTab(x));
        CloseOthersCommand = new RelayCommand<Item>(x => CloseOthers(x));
    }

    public RequestPageViewModel PageViewModel { get; }

    public RequestControlViewModel ViewModel { get; }

    public UrlBarViewModel UrlBarViewModel { get; }

    public AuthControlViewModel AuthControlViewModel { get; }

    public RequestBodyViewModel RequestBodyViewModel { get; }

    public BodyControlViewModel BodyControlViewModel { get; }

    public StatusBarViewModel StatusBarViewModel { get; }

    public bool SaveButtonVisible
    {
        get => _saveButtonVisible && ViewModel?.Request?.IsTemporary == true;
        set
        {
            if (_saveButtonVisible != value)
            {
                _saveButtonVisible = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(EllipseVisible));
            }
        }
    }
    private bool _saveButtonVisible;

    public bool EllipseVisible => !_saveButtonVisible && ViewModel?.Request?.IsTemporary == true;

    public void ShowSaveButton()
    {
        SaveButtonVisible = true;
    }

    public void HideSaveButton()
    {
        SaveButtonVisible = false;
    }

    public void CloseThis()
    {
        CloseTabCommand.Execute(ViewModel.Request);
    }

    private async void SaveToWorkspace(Item item)
    {
        _telemetry.TrackEvent(Telemetry.TabContext, Telemetry.Props(Telemetry.TabContextSave));
        await _workspaceTreeModifier.InsertToWorkspaceAsync(item);
    }

    private async void RenameTab(Item item) 
    { 
        _telemetry.TrackEvent(Telemetry.TabContext, Telemetry.Props(Telemetry.TabContextRename));
        await _workspaceTreeModifier.EditItemAsync(item); 
    }

    private async void DuplicateTab(Item item)
    {
        _telemetry.TrackEvent(Telemetry.TabContext, Telemetry.Props(Telemetry.TabContextDuplicate));
        await _tabCollectionContainer.DuplicateTabAsync(item);
    }

    private void CloseOthers(Item item)
    {
        _telemetry.TrackEvent(Telemetry.TabContext, Telemetry.Props(Telemetry.TabContextCloseOthers));
        _tabCollectionContainer.RemoveAllButThis(item);
    }

    private void CloseTab(Item item)
    {
        _telemetry.TrackEvent(Telemetry.TabContext, Telemetry.Props(Telemetry.TabContextClose));
        _tabCollectionContainer.RemoveTab(item);
    }
}
