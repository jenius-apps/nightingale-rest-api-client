using Nightingale.Core.Workspaces.Models;
using Nightingale.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class NewRequestDialog : ContentDialog, INotifyPropertyChanged
    {
        private ItemShallowReference _selectedLocation;
        private string _title;
        private bool _isLocationSelectorVisible;
        private string _newName;

        public NewRequestDialog()
        {
            this.InitializeComponent();
            this.RequestedTheme = ThemeController.GetTheme();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string NewName
        {
            get => _newName;
            set
            {
                _newName = value;
                RaisePropertyChanged("NewName");
            }
        }

        public string DialogTitle
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged("DialogTitle");
            }
        }

        public bool IsLocationSelectorVisible
        {
            get => _isLocationSelectorVisible;
            set
            {
                _isLocationSelectorVisible = value;
                RaisePropertyChanged("IsLocationSelectorVisible");
            }
        }

        public ItemShallowReference SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                RaisePropertyChanged("SelectedLocation");
            }
        }

        public bool Create { get; private set; } = false;

        public List<ItemShallowReference> Locations { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CaptureUserInput();
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NameBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                CaptureUserInput();
                Hide();
                e.Handled = true;
            }
        }

        private void CaptureUserInput()
        {
            NewName = NameBox.Text;
            Create = true;
        }

        private void NameBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NameBox.SelectAll();
        }

        private void ItemTreeView_ItemInvoked(object sender, CustomEventArgs.AddedItemArgs<object> e)
        {
            if (e.AddedItem is ItemShallowReference item)
            {
                SelectedLocation = item;
            }
        }
    }
}
