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

#nullable enable

namespace Nightingale.Dialogs;

public sealed partial class PremiumDialog : ContentDialog
{
    public PremiumDialog()
    {
        this.InitializeComponent();
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        this.Hide();
    }
}
