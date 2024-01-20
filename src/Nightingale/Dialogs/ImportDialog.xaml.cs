using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Utilities;
using Nightingale.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class ImportDialog : ContentDialog
    {
        public ImportDialog()
        {
            this.InitializeComponent();
            this.RequestedTheme = ThemeController.GetTheme();
        }

        public ImportPostmanViewModel ViewModel { get; set; }

        public List<Item> ImportedCollections { get; set; } = new List<Item>();

        public List<Workspace> ImportedWorkspaces { get; set; } = new List<Workspace>();

        private void ExitButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Rectangle_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
        
        private async void Rectangle_Drop(object sender, DragEventArgs e)
        {
            if (ViewModel == null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count == 0)
                {
                    return;
                }

                (var cols, var works) = await ViewModel.DropFiles(items.ToList());

                if (cols != null)
                {
                    ImportedCollections.AddRange(cols);
                }

                if (works != null)
                {
                    ImportedWorkspaces.AddRange(works);
                }
            }
        }

        private void ConvertCurlClicked()
        {
            if (ViewModel == null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            var item = ViewModel.ConvertCurl();
            if (item != null)
            {
                ImportedCollections.Add(item);
            }
        }

        private async void SelectFiles()
        {
            if (ViewModel == null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            (var cols, var works) = await ViewModel.SelectFilesAsync();
            
            if (cols != null)
            {
                ImportedCollections.AddRange(cols);
            }

            if (works != null)
            {
                ImportedWorkspaces.AddRange(works);
            }
        }
    }
}
