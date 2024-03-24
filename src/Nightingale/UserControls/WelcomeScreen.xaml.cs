using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Handlers;
using Nightingale.Utilities;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Nightingale.UserControls;

public sealed partial class WelcomeScreen : UserControl
{
    public event EventHandler NewTabClicked;
    public event EventHandler NewRequestClicked;
    public event EventHandler NewCollectionClicked;

    public WelcomeScreen()
    {
        this.InitializeComponent();
        TryLoadVip();
    }

    private void NewTab()
    {
        NewTabClicked?.Invoke(this, new EventArgs());
        App.Services.GetRequiredService<ITelemetry>().TrackEvent(Telemetry.WelcomeNewTab);
    }

    private void NewRequest()
    {
        NewRequestClicked?.Invoke(this, new EventArgs());
        App.Services.GetRequiredService<ITelemetry>().TrackEvent(Telemetry.WelcomeNewRequest);
    }

    private void NewCollection()
    {
        NewCollectionClicked?.Invoke(this, new EventArgs());
        App.Services.GetRequiredService<ITelemetry>().TrackEvent(Telemetry.WelcomeNewCollection);
    }

    private async void TryLoadVip()
    {
        bool subscribed = await StoreHandler.IsUserSubscribed();
        VipGrid.Visibility = subscribed
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
