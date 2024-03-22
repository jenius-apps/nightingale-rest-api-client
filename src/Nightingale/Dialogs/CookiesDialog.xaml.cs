using Nightingale.Core.Cookies.Models;
using Nightingale.Utilities;
using Nightingale.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

#nullable enable

namespace Nightingale.Dialogs;

public sealed partial class CookiesDialog : ContentDialog
{
    public CookiesDialog()
    {
        this.InitializeComponent();
        this.RequestedTheme = ThemeController.GetTheme();
    }

    public CookiesViewModel? ViewModel { get; set; }

    private void ExitButton_Clicked(object sender, RoutedEventArgs e)
    {
        this.Hide();
    }

    private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && b.DataContext is Cookie c)
        {
            ViewModel?.DeleteCookie(c);
        }
    }

    private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            e.Handled = true;
            ViewModel?.AddCookie();
        }
    }
}
