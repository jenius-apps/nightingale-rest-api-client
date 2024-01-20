using Nightingale.Utilities;
using System;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public SettingsDialog()
        {
            this.InitializeComponent();
            this.RequestedTheme = ThemeController.GetTheme();
            ThemeController.ThemeChanged += ThemeController_ThemeChanged;
        }

        private void ThemeController_ThemeChanged(object sender, EventArgs e)
        {
            this.RequestedTheme = ThemeController.GetTheme();
        }

        private void SettingsControl_ExitButtonClicked(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
