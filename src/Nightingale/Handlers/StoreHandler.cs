using Microsoft.Toolkit.Uwp.Connectivity;
using Nightingale.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Handlers
{
    public class StoreHandler
    {
        private static Dictionary<string, StoreProduct> _productsCache = new Dictionary<string, StoreProduct>();
        private static StoreContext _context = null;
        public const string PremiumMonthlyId = "9P5R7HN3XZST";
        public const string PremiumYearlyId = "9P7RKK41J260";
        public const string OldPremiumYearlyId = "9N94DBHZ7L4C";
        public const string PremiumDurable = "9MT68PCB9Q37";
        public const string MvpBadgeId = "9NJ6LPPR04XF";
        public const string MvpBadgeGoldId = "9P4SQBX623D9";

        public static async Task<IReadOnlyDictionary<string, StoreProduct>> GetLicensesAsync()
        {
            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                return null;
            }

            if (_context == null)
                _context = StoreContext.GetDefault();

            var queryResult = await _context.GetUserCollectionAsync(new string[] { "Durable", "Consumable" });
            return queryResult.Products;
        }

        /// <summary>
        /// Returns true if user is subscribed to a subscription or has purchased premium.
        /// </summary>
        /// <returns>True if user is subscribed or has purchased premium, false otherwise.</returns>
        public static async Task<bool> IsUserSubscribed()
        {
            long userSubDateTicks = UserSettings.Get<long>(SettingsConstants.PremiumDateUnlocked);
            DateTime userSubDate = new DateTime(userSubDateTicks);

            if (userSubDate > DateTime.MinValue)
            {
                string premiumId = UserSettings.Get<string>(SettingsConstants.PremiumIapId);

                if (premiumId == PremiumDurable)
                {
                    return true;
                }
                else if (premiumId == PremiumMonthlyId && DateTime.Now > userSubDate && DateTime.Now < userSubDate.AddMonths(1))
                {
                    return true;
                }
                else if (premiumId == PremiumYearlyId && DateTime.Now > userSubDate && DateTime.Now < userSubDate.AddYears(1))
                {
                    return true;
                }
                else if (premiumId == OldPremiumYearlyId && DateTime.Now > userSubDate && DateTime.Now < userSubDate.AddYears(1))
                {
                    return true;
                }
            }

            bool result = false;

            if (_context == null)
                _context = StoreContext.GetDefault();

            StoreAppLicense appLicense = await _context.GetAppLicenseAsync();

            if (appLicense == null)
                return false;

            StoreLicense activeLicense = null;

            /// Check if user has an active license for given add-on id.
            foreach (var addOnLicense in appLicense.AddOnLicenses)
            {
                var license = addOnLicense.Value;
                if ((license.SkuStoreId.StartsWith(PremiumMonthlyId)
                    || license.SkuStoreId.StartsWith(OldPremiumYearlyId)
                    || license.SkuStoreId.StartsWith(PremiumYearlyId))
                    && license.IsActive)
                {
                    result = true;
                    activeLicense = license;
                    break;
                }

                if (license.SkuStoreId.StartsWith(PremiumDurable) && license.IsActive)
                {
                    result = true;
                    activeLicense = license;
                    break;
                }
            }

            if (result && activeLicense != null)
            {
                string id = "";

                if (activeLicense.SkuStoreId.StartsWith(PremiumDurable))
                {
                    id = PremiumDurable;
                }
                else if (activeLicense.SkuStoreId.StartsWith(PremiumMonthlyId))
                {
                    id = PremiumMonthlyId;
                }
                else if (activeLicense.SkuStoreId.StartsWith(PremiumYearlyId))
                {
                    id = PremiumYearlyId;
                }
                else if (activeLicense.SkuStoreId.StartsWith(OldPremiumYearlyId))
                {
                    id = OldPremiumYearlyId;
                }

                UpdateLocalSettings(id, DateTime.Now.Ticks);
            }

            return result;
        }

        private static void UpdateLocalSettings(string iapId, long ticks)
        {
            UserSettings.Set<long>(SettingsConstants.PremiumDateUnlocked, ticks);
            UserSettings.Set<string>(SettingsConstants.PremiumIapId, iapId.ToUpper());
        }

        /// <summary>
        /// Retrieves add-on object from store context.
        /// </summary>
        /// <param name="id">Id of add-on to retrieve.</param>
        public static async Task<StoreProduct> GetAddOn(string id)
        {
            if (_productsCache.ContainsKey(id))
            {
                return _productsCache[id];
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

                if (product.StoreId == id)
                {
                    _productsCache.TryAdd(id, product);
                    return product;
                }
            }

            return null;
        }

        /// <summary>
        /// Purchases given add-on.
        /// </summary>
        /// <param name="id">Add-on id to purchase.</param>
        public static async Task<bool> PurchaseAddOn(string id)
        {
            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                return false;
            }

            var addOnProduct = await GetAddOn(id);
            if (addOnProduct == null)
                return false;

            /// Attempt purchase
            var result = await addOnProduct.RequestPurchaseAsync();
            if (result == null)
                return false;

            bool purchased = false;
            switch (result.Status)
            {
                case StorePurchaseStatus.NotPurchased:
                case StorePurchaseStatus.ServerError:
                case StorePurchaseStatus.NetworkError:
                    break;
                case StorePurchaseStatus.Succeeded:
                case StorePurchaseStatus.AlreadyPurchased:
                    purchased = true;
                    break;
            }

            return purchased;
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
                if (jsonObject.SelectToken("status").ToString() == "success")
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
}
