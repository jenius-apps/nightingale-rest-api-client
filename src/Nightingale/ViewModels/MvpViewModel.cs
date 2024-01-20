using Newtonsoft.Json;
using Nightingale.Core.Models;
using Nightingale.Core.Settings;
using Nightingale.Handlers;
using Nightingale.StoreHelpers.Models;
using System;
using System.Linq;
using Windows.Services.Store;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Nightingale.ViewModels
{
    public class MvpViewModel : ObservableBase
    {
        private readonly IUserSettings _userSettings;
        private const string PlainBadgeColour = "#FF0DFFBB";

        public MvpViewModel(IUserSettings userSettings)
        {
            _userSettings = userSettings
                ?? throw new ArgumentNullException(nameof(userSettings));
            BadgeBrush = new SolidColorBrush(GetWindowsColour(PlainBadgeColour));

            Initialize();
        }

        public bool BadgeVisible
        {
            get => _badgeVisible;
            set
            {
                if (_badgeVisible != value)
                {
                    _badgeVisible = value;
                    RaisePropertyChanged();
                }
            }
        }
        private bool _badgeVisible;

        public SolidColorBrush BadgeBrush { get; set; }

        public void UpdateBadgeBrush(bool useGold)
        {
            BadgeBrush = useGold 
                ? new SolidColorBrush(Colors.Gold)
                : new SolidColorBrush(GetWindowsColour(PlainBadgeColour));
            RaisePropertyChanged(nameof(BadgeBrush));
        }

        private Color GetWindowsColour(string hex)
        {
            hex = hex.Replace("#", "");
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            return Color.FromArgb(a, r, g, b);
        }

        public async void Initialize()
        {
            bool show = await _userSettings.GetAsync<bool>(SettingsConstants.ShowMvpBadge);
            if (!show)
            {
                BadgeVisible = false;
                return;
            }

            var appLicense = await StoreHandler.GetLicensesAsync();
            StoreProduct mvpAddOn = appLicense?.Values?.FirstOrDefault(x => x.StoreId == StoreHandler.MvpBadgeGoldId) 
                ?? appLicense?.Values?.FirstOrDefault(x => x.StoreId == StoreHandler.MvpBadgeId);
            if (mvpAddOn == null)
            {
                return;
            }

            var data = JsonConvert.DeserializeObject<ExtendedData>(mvpAddOn.ExtendedJsonData)?.DisplaySkuAvailabilities?.FirstOrDefault()?.Sku?.CollectionData;
            if (data == null)
            {
                return;
            }

            if (data.ModifiedDate.AddDays(30) > DateTime.Now && data.Quantity > 0)
            {
                UpdateBadgeBrush(mvpAddOn.StoreId == StoreHandler.MvpBadgeGoldId);
                BadgeVisible = true;
            }
        }
    }
}
