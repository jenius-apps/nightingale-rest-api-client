using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Concurrent;
using System.Linq;
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
        return false;
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

    public async Task<PriceInfo> GetPriceAsync(string storeProductId)
    {
        var addon = await GetAddOnAsync(storeProductId).ConfigureAwait(false);

        if (addon?.Price is null)
        {
            return new PriceInfo { FormattedPrice = "-" };
        }

        var sku = addon.Skus?.FirstOrDefault();
        bool isSub = sku?.IsSubscription ?? false;

        return new PriceInfo
        {
            FormattedPrice = isSub ? addon.Price.FormattedRecurrencePrice : addon.Price.FormattedPrice,
            IsSubscription = isSub,
            RecurrenceLength = (int)(sku?.SubscriptionInfo?.BillingPeriod ?? 0),
            RecurrenceUnit = ToDurationUnit(sku?.SubscriptionInfo?.BillingPeriodUnit),
            HasSubTrial = sku?.SubscriptionInfo?.HasTrialPeriod ?? false,
            SubTrialLength = (int)(sku?.SubscriptionInfo?.TrialPeriod ?? 0),
            SubTrialLengthUnit = ToDurationUnit(sku?.SubscriptionInfo?.TrialPeriodUnit),
        };
    }

    private DurationUnit ToDurationUnit(StoreDurationUnit? storeDurationUnit)
    {
        return storeDurationUnit switch
        {
            StoreDurationUnit.Minute => DurationUnit.Minute,
            StoreDurationUnit.Hour => DurationUnit.Hour,
            StoreDurationUnit.Day => DurationUnit.Day,
            StoreDurationUnit.Week => DurationUnit.Week,
            StoreDurationUnit.Month => DurationUnit.Month,
            StoreDurationUnit.Year => DurationUnit.Year,
            _ => DurationUnit.Minute
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

/// <summary>
/// Class that holds price information.
/// </summary>
public class PriceInfo
{
    /// <summary>
    /// The localized and formatted price of the product.
    /// E.g. $1.49.
    /// </summary>
    public string FormattedPrice { get; init; } = string.Empty;

    /// <summary>
    /// Specifies if this product is a subscription.
    /// </summary>
    public bool IsSubscription { get; init; }

    /// <summary>
    /// If the product is a subscription,
    /// this specifies the length of recurrence.
    /// E.g. If it's a 1-month recurrence, then this property is 1.
    /// </summary>
    public int RecurrenceLength { get; init; }

    /// <summary>
    /// If the product is a subscription,
    /// this specifies the unit of recurrence.
    /// E.g. If it's a 1-month recurrence, then this property is <see cref="DurationUnit.Month"/>.
    /// </summary>
    public DurationUnit RecurrenceUnit { get; init; }

    /// <summary>
    /// If the product is a subscription,
    /// this specifies if there is a trial period.
    /// </summary>
    public bool HasSubTrial { get; init; }

    /// <summary>
    /// If the product is a subscription and has a trial period,
    /// this specifies the length of the trial.
    /// E.g. If it's a 1-week trial, then this property is 1.
    /// </summary>
    public int SubTrialLength { get; init; }


    /// <summary>
    /// If the product is a subscription and has a trial period,
    /// this specifies the unit of the trial period.
    /// E.g. If it's a 1-week trial, then this property is <see cref="DurationUnit.Week"/>.
    /// </summary>
    public DurationUnit SubTrialLengthUnit { get; init; }
}

public enum DurationUnit
{
    Minute,
    Hour,
    Day,
    Week,
    Month,
    Year,
}
