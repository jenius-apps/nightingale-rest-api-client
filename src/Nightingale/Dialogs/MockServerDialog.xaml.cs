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
    public sealed partial class MockServerDialog : ContentDialog
    {
        public MockServerDialog()
        {
            this.InitializeComponent();
        }

        public string WorkspaceName { get; set; }

        public string ItemName
        {
            get => string.IsNullOrWhiteSpace(_itemName) ? "--" : _itemName;
            set => _itemName = value;
        }
        private string _itemName;

        public string EnvName
        {
            get => string.IsNullOrWhiteSpace(_envName) ? "Base" : _envName;
            set => _envName = value;
        }
        private string _envName;

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
