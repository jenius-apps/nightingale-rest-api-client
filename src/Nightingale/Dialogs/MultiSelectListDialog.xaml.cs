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
    public sealed partial class MultiSelectListDialog : ContentDialog
    {
        public MultiSelectListDialog()
        {
            this.InitializeComponent();
        }
        
        public object ItemsSource { get; set; }

        public IList<object> SelectedItems { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (SelectedItems == null || SelectedItems.Count == 0)
            {
                args.Cancel = true;
            }
        }

        private void SelectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems == null)
            {
                SelectedItems = new List<object>();
            }

            foreach (var item in e.RemovedItems)
            {
                SelectedItems.Remove(item);
            }

            foreach (var item in e.AddedItems)
            {
                SelectedItems.Add(item);
            }
        }

        private void SelectionList_Loaded(object sender, RoutedEventArgs e)
        {
            // Select all items at dialog launch
            if (sender is ListView l)
            {
                l.SelectAll();
            }
        }
    }
}
