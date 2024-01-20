using Microsoft.AppCenter.Analytics;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Settings;
using Nightingale.Core.Storage.Interfaces;
using Nightingale.CustomEventArgs;
using Nightingale.Dialogs;
using Nightingale.Handlers;
using Nightingale.VisualState;
using System;
using System.Collections.Generic;

namespace Nightingale.ViewModels
{
    public class RequestPageViewModel : ViewModelBase
    {
        private readonly IVisualStatePublisher _visualStatePublisher;
        private readonly IParameterStorageAccessor _parameterStorageAccessor;
        private readonly ICookieDialogService _cookieDialogService;
        private readonly IEnvironmentContainer _environmentContainer;

        public RequestPageViewModel(
            IVisualStatePublisher visualStatePublisher,
            IParameterStorageAccessor parameterStorageAccessor,
            ICookieDialogService cookieDialogService,
            IEnvironmentContainer environmentContainer,
            IEnvironmentDialogService environmentDialogService)
        {
            _visualStatePublisher = visualStatePublisher 
                ?? throw new ArgumentNullException(nameof(visualStatePublisher));
            _parameterStorageAccessor = parameterStorageAccessor 
                ?? throw new ArgumentNullException(nameof(parameterStorageAccessor));
            _cookieDialogService = cookieDialogService
                ?? throw new ArgumentNullException(nameof(cookieDialogService));
            _environmentContainer = environmentContainer
                ?? throw new ArgumentNullException(nameof(environmentContainer));
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

            Analytics.TrackEvent("Variable quick add", new Dictionary<string, string>
            {
                { "successful", success.ToString() },
            });
        }

        public async void ManageCookiesClicked()
        {
            await _cookieDialogService.OpenCookieDialog();
            Analytics.TrackEvent("Manage response cookies clicked");
        }
    }
}
