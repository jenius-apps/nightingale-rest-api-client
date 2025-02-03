using Microsoft.Extensions.DependencyInjection;
using Nightingale.ViewModels;
using System;
using System.Threading;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Nightingale.Dialogs;

public sealed partial class PremiumDialog : ContentDialog
{
    private readonly CancellationTokenSource _cts = new();

    public PremiumDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PremiumDialogViewModel>();

        if (new GeographicRegion().CodeTwoLetter.Equals("us", System.StringComparison.OrdinalIgnoreCase))
        {
            TariffText.Visibility = Visibility.Visible;
        }
    }

    public PremiumDialogViewModel ViewModel { get; }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        this.Hide();
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        try
        {
            await ViewModel.InitializeAsync(_cts.Token);
            ViewModel.CloseRequested += OnCloseRequested;
        }
        catch { }
    }

    private void OnCloseRequested(object sender, EventArgs e)
    {
        this.Hide();
    }

    private void OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        ViewModel.CloseRequested -= OnCloseRequested;
        _cts.Cancel();
    }
}
