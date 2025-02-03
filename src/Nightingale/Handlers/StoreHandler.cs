using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Windows.Services.Store;

#nullable enable

namespace Nightingale.Handlers;

public class StoreHandler : IStoreHandler
{
    private readonly ConcurrentDictionary<string, StoreProduct> _productsCache = new();
    private readonly ConcurrentDictionary<string, bool> _ownershipCache = new();
    private StoreContext? _context = null;

    public event EventHandler<string>? ProductPurchased;

    public async Task<bool> IsOwnedAsync(string storeProductId)
    {
#if DEBUG
        //return true;
#endif

        if (_ownershipCache.TryGetValue(storeProductId, out bool isOwned))
        {
            return isOwned;
        }

        _context ??= StoreContext.GetDefault();

        StoreAppLicense appLicense = await _context.GetAppLicenseAsync();
        if (appLicense is null)
        {
            return false;
        }

        foreach (var addOnLicense in appLicense.AddOnLicenses)
        {
            StoreLicense license = addOnLicense.Value;

            if (license.SkuStoreId == storeProductId && license.IsActive)
            {
                _ownershipCache.TryAdd(storeProductId, true);
                return true;
            }
        }

        _ownershipCache.TryAdd(storeProductId, false);
        return false;
    }

    public async Task<bool> BuyAsync(string storeProductId)
    {
        if (_ownershipCache.TryGetValue(storeProductId, out bool isOwned) && isOwned)
        {
            // Product is already owned, so return true to simulate that the purchase was successful.
            return true;
        }

        StorePurchaseStatus result = await PurchaseAddOnAsync(storeProductId);

        if (result == StorePurchaseStatus.Succeeded || result == StorePurchaseStatus.AlreadyPurchased)
        {
            _ownershipCache.AddOrUpdate(storeProductId, true, (id, oldValue) => true);
            ProductPurchased?.Invoke(this, storeProductId);
        }

        return result switch
        {
            StorePurchaseStatus.Succeeded => true,
            StorePurchaseStatus.AlreadyPurchased => true,
            _ => false
        };
    }

    private async Task<StoreProduct?> GetAddOnAsync(string storeProductId)
    {
        if (_productsCache.ContainsKey(storeProductId))
        {
            return _productsCache[storeProductId];
        }

        if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
        {
            return null;
        }

        if (_context == null)
            _context = StoreContext.GetDefault();

        /// Get all add-ons for this app.
        var result = await _context.GetAssociatedStoreProductsAsync(new string[] { "Durable", "Consumable" });
        if (result.ExtendedError != null)
        {
            return null;
        }

        foreach (var item in result.Products)
        {
            var product = item.Value;

            if (product.StoreId == storeProductId)
            {
                _productsCache.TryAdd(storeProductId, product);
                return product;
            }
        }

        return null;
    }

    private async Task<StorePurchaseStatus> PurchaseAddOnAsync(string storeProductId)
    {
        if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
        {
            return StorePurchaseStatus.NetworkError;
        }

        var addOnProduct = await GetAddOnAsync(storeProductId);

        if (addOnProduct is null)
            return StorePurchaseStatus.ServerError;

        /// Attempt purchase
        var result = await addOnProduct.RequestPurchaseAsync();
        if (result is null)
            return StorePurchaseStatus.ServerError;

        return result.Status;
    }

    public static async Task<bool> ShowRatingReviewDialog()
    {
        StoreSendRequestResult result = await StoreRequestHelper.SendRequestAsync(
            StoreContext.GetDefault(), 16, String.Empty);

        if (result == null)
        {
            return false;
        }

        if (result.ExtendedError == null)
        {
            Newtonsoft.Json.Linq.JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(result.Response);
            if (jsonObject.SelectToken("status")?.ToString() == "success")
            {
                // The customer rated or reviewed the app.
                return true;
            }
        }

        // There was an error with the request, or the customer chose not to
        // rate or review the app.
        return false;
    }
}
