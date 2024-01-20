using Microsoft.AppCenter.Analytics;
using Nightingale.Handlers;
using Nightingale.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
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
            Analytics.TrackEvent(Telemetry.WelcomeNewTab);
        }

        private void NewRequest()
        {
            NewRequestClicked?.Invoke(this, new EventArgs());
            Analytics.TrackEvent(Telemetry.WelcomeNewRequest);
        }

        private void NewCollection()
        {
            NewCollectionClicked?.Invoke(this, new EventArgs());
            Analytics.TrackEvent(Telemetry.WelcomeNewCollection);
        }

        private async void TryLoadVip()
        {
            bool subscribed = await StoreHandler.IsUserSubscribed();
            VipGrid.Visibility = subscribed
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}
