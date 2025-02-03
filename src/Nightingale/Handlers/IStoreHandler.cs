using System;
using System.Threading.Tasks;

#nullable enable

namespace Nightingale.Handlers;

public interface IStoreHandler
{
    event EventHandler<string>? ProductPurchased;

    Task<bool> BuyAsync(string storeProductId);
    Task<PriceInfo> GetPriceAsync(string storeProductId);
    Task<bool> IsOwnedAsync(string storeProductId);
}