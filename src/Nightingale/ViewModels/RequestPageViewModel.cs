using JeniusApps.Common.Telemetry;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Storage.Interfaces;
using Nightingale.CustomEventArgs;
using Nightingale.Dialogs;
using Nightingale.VisualState;
using System;
using System.Collections.Generic;

namespace Nightingale.ViewModels;

public class RequestPageViewModel : ViewModelBase
{
    private readonly IVisualStatePublisher _visualStatePublisher;
    private readonly IParameterStorageAccessor _parameterStorageAccessor;
    private readonly ICookieDialogService _cookieDialogService;
    private readonly IEnvironmentContainer _environmentContainer;
    private readonly ITelemetry _telemetry;

    public RequestPageViewModel(
        IVisualStatePublisher visualStatePublisher,
        IParameterStorageAccessor parameterStorageAccessor,
        ICookieDialogService cookieDialogService,
        IEnvironmentContainer environmentContainer,
        IEnvironmentDialogService environmentDialogService,
        ITelemetry telemetry)
    {
        _visualStatePublisher = visualStatePublisher;
        _parameterStorageAccessor = parameterStorageAccessor;
        _cookieDialogService = cookieDialogService;
        _environmentContainer = environmentContainer;
        _telemetry = telemetry;
        _visualStatePublisher.PaneLayoutToggled += VisualStatePublisher_PaneLayoutToggled;
    }

    private void VisualStatePublisher_PaneLayoutToggled(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(IsTwoPaneLayout));
        RaisePropertyChanged(nameof(IsSinglePaneLayout));
    }

    public bool IsTwoPaneLayout
    {
        get => _visualStatePublisher.IsLayoutTwoPane();
    }

    public bool IsSinglePaneLayout => !IsTwoPaneLayout;

    public async void ParameterDeleted(object sender, DeletedItemArgs<Parameter> args)
    {
        if (args?.DeletedItem == null)
        {
            return;
        }

        await _parameterStorageAccessor.DeleteParameterAsync(args.DeletedItem);
    }

    public async void AddVariableClicked(object sender, AddedItemArgs<Parameter> args)
    {
        if (string.IsNullOrWhiteSpace(args?.AddedItem?.Key))
        {
            return;
        }

        bool success = await _environmentContainer.AddVariablePromptAsync(args.AddedItem);

        _telemetry.TrackEvent("Variable quick add", new Dictionary<string, string>
        {
            { "successful", success.ToString() },
        });
    }

    public async void ManageCookiesClicked()
    {
        await _cookieDialogService.OpenCookieDialog();
        _telemetry.TrackEvent("Manage response cookies clicked");
    }
}
