using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class ConfirmDeleteDialog : ContentDialog
    {
        public ConfirmDeleteDialog()
        {
            this.InitializeComponent();
        }

        public bool DeleteWithoutAsking { get; set; }

        public bool ShowCheckBox { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DeleteWithoutAsking = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DeleteWithoutAsking = false;
        }
    }
}
