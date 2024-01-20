using Microsoft.AppCenter.Analytics;
using Nightingale.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class CustomMethodsDialog : ContentDialog
    {
        public CustomMethodsDialog()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<string> Methods { get; set; }

        private void ContentDialog_PrimaryButtonClick(
            ContentDialog sender, 
            ContentDialogButtonClickEventArgs args)
        {
        }

        private void DeleteMethodClicked(
            object sender, 
            RoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement f 
                && f.DataContext is string method)
            {
                Methods.Remove(method);
                Analytics.TrackEvent(Telemetry.CustomMethodRemoved, new Dictionary<string, string>
                {
                    { "value", method }
                });
            }
        }

        private void ResetListClicked(
            ContentDialog sender, 
            ContentDialogButtonClickEventArgs args)
        {
            Methods.Clear();
            foreach (var method in Core.Http.Method.Defaults)
            {
                Methods.Add(method);
            }

            Analytics.TrackEvent(Telemetry.CustomMethodReset);
            args.Cancel = true;
        }

        private void AddMethodClicked(
            object sender, 
            RoutedEventArgs e)
        {
            AddNew();
        }

        private void NewMethodBoxKeyDown(
            object sender, 
            KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AddNew();
            }
        }

        private void AddNew()
        {
            if (!string.IsNullOrWhiteSpace(NewMethodBox.Text)
                && !Methods.Contains(NewMethodBox.Text.Trim()))
            {
                Methods.Add(NewMethodBox.Text.Trim().ToUpper());
                NewMethodBox.Text = "";

                Analytics.TrackEvent(Telemetry.CustomMethodAdded, new Dictionary<string, string>
                {
                    { "value", NewMethodBox.Text }
                });
            }
        }
    }
}
