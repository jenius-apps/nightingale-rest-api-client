using Microsoft.AppCenter.Analytics;
using Nightingale.Core.Cookies.Models;
using Nightingale.Utilities;
using Nightingale.ViewModels;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class CookiesDialog : ContentDialog
    {
        public CookiesDialog()
        {
            this.InitializeComponent();
            this.RequestedTheme = ThemeController.GetTheme();
        }

        public CookiesViewModel ViewModel { get; set; }

        private void ExitButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is Cookie c)
            {
                ViewModel.DeleteCookie(c);
            }
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                ViewModel.AddCookie();
            }
        }
    }
}
