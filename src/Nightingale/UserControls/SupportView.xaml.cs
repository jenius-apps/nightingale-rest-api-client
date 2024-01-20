using Nightingale.Handlers;
using Nightingale.Utilities;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class SupportView : UserControl
    {
        public event EventHandler MvpClicked;

        public SupportView()
        {
            this.InitializeComponent();
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (DataTransferManager sender, DataRequestedEventArgs args) =>
            {
                DataRequest request = args.Request;
                request.Data.SetWebLink(new Uri("https://nightingale.rest"));
                request.Data.Properties.Title = "https://nightingale.rest";
                request.Data.Properties.Description = "Share Nightingale's website with others";
            };
        }

        private async void Rate()
        {
            Telemetry.Support(Telemetry.SupportRate);
            await StoreHandler.ShowRatingReviewDialog();
        }
        private async void Github()
        {
            Telemetry.Support(Telemetry.SupportGitHub);
            await Launcher.LaunchUriAsync(new Uri("https://www.github.com/jenius-apps/nightingale-rest-api-client"));
        }
        private async void Paypal()
        {
            Telemetry.Support(Telemetry.SupportPaypal);
            await Launcher.LaunchUriAsync(new Uri("https://paypal.me/kidjenius"));
        }
        private void Share()
        {
            Telemetry.Support(Telemetry.SupportShare);
            DataTransferManager.ShowShareUI();
        }
        private void Mvp()
        {
            Telemetry.Support(Telemetry.SupportMvp);
            MvpClicked?.Invoke(this, new EventArgs());
        }
    }
}
