using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nightingale.Core.Constants;
using Nightingale.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Nightingale.ViewModels;

public partial class PremiumDialogViewModel : ObservableObject
{
    private readonly IStoreHandler _storeHandler;

    public event EventHandler? CloseRequested;

    public PremiumDialogViewModel(IStoreHandler storeHandler)
    {
        _storeHandler = storeHandler;
    }

    [ObservableProperty]
    private bool _premiumPriceLoading;

    [ObservableProperty]
    private string _premiumPrice = string.Empty;

    public async Task InitializeAsync(CancellationToken token)
    {
        PremiumPriceLoading = true;
        token.ThrowIfCancellationRequested();
        var priceInfo = await _storeHandler.GetPriceAsync(IapConstants.PremiumDurable);
        PremiumPrice = priceInfo.FormattedPrice;
        PremiumPriceLoading = false;
    }

    [RelayCommand]
    private async Task BuyPremiumAsync()
    {
        PremiumPriceLoading = true;
        var successful = await _storeHandler.BuyAsync(IapConstants.PremiumDurable);

        if (successful)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
            return;
        }

        PremiumPriceLoading = false;
    }
}
