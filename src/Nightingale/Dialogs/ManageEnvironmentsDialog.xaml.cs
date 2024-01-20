using Nightingale.Utilities;
using Nightingale.ViewModels;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class ManageEnvironmentsDialog : ContentDialog
    {
        public ManageEnvironmentsDialog()
        {
            this.InitializeComponent();
            this.RequestedTheme = ThemeController.GetTheme();
        }

        public EnvironmentsViewModel ViewModel { get; set; }

        private void EnvironmentsControl_ExitButtonClicked(object sender, System.EventArgs e)
        {
            Hide();
        }
    }
}
