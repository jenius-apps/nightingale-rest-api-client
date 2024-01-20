using Microsoft.AppCenter.Analytics;
using Nightingale.Handlers;
using Nightingale.ViewModels;
using SQLitePCL;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class MvpDialog : ContentDialog
    {
        private bool _purchasing;

        public MvpDialog()
        {
            this.InitializeComponent();
        }

        public MvpViewModel MvpViewModel { get; set; }

        private void Close()
        {
            this.Hide();
        }

        private void Purchasing(bool on)
        {
            _purchasing = on;

            if (on)
            {
                CloseButton.Visibility = Visibility.Collapsed;
                progressRing.IsActive = true;
            }
            else
            {
                CloseButton.Visibility = Visibility.Visible;
                progressRing.IsActive = false;
            }
        }

        private async void BuyBadge()
        {
            if (_purchasing)
            {
                return;
            }

            Purchasing(true);
            bool purchased = await StoreHandler.PurchaseAddOn(StoreHandler.MvpBadgeId);
            if (purchased)
            {
                MvpViewModel.Initialize();
                PurchasedUpdateUI();
            }
            Purchasing(false);

            Analytics.TrackEvent("badge", new Dictionary<string, string>
            {
                { "purchased", purchased.ToString() },
                { "type", "plain" }
            });
        }

        private async void BuyBadgeGold()
        {
            if (_purchasing)
            {
                return;
            }

            Purchasing(true);
            bool purchased = await StoreHandler.PurchaseAddOn(StoreHandler.MvpBadgeGoldId);
            if (purchased)
            {
                MvpViewModel.Initialize();
                PurchasedUpdateUI();
            }
            Purchasing(false);

            Analytics.TrackEvent("badge", new Dictionary<string, string>
            {
                { "purchased", purchased.ToString() },
                { "type", "gold" }
            });
        }

        private void PurchasedUpdateUI()
        {
            BadgeButton.Visibility = Visibility.Collapsed;
            GoldBadgeButton.Visibility = Visibility.Collapsed;
            PurchaseMsg.Visibility = Visibility.Visible;
        }
    }
}
