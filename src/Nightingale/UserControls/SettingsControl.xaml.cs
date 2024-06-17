using System;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Utilities;
using Nightingale.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class SettingsControl : ObservableUserControl
    {
        public event EventHandler ExitButtonClicked;

        public SettingsControl()
        {
            this.InitializeComponent();
            this.DataContext = App.Services.GetRequiredService<SettingsViewModel>();
        }

        public SettingsViewModel ViewModel => (SettingsViewModel)this.DataContext;

        private void ExitButton_Clicked() => ExitButtonClicked?.Invoke(this, new EventArgs());
    }
}
