using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Models;
using Nightingale.Core.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Nightingale.UserControls
{
    public sealed partial class ExportControl : UserControl
    {
        public ExportControl()
        {
            this.InitializeComponent();
            this.DataContext = App.Services.GetRequiredService<ExportControlViewModel>();
        }

        public ExportControlViewModel ViewModel => (ExportControlViewModel)this.DataContext;

        private ListViewSelectionMode ToSelectionMode(bool multiSelect)
        {
            return multiSelect ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Single;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                if (item is Workspace w)
                {
                    ViewModel.SelectedWorkspaces.Remove(w);
                }
            }

            foreach (var item in e.AddedItems)
            {
                if (item is Workspace w)
                {
                    ViewModel.SelectedWorkspaces.Add(w);
                }
            }
        }

        private void ListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is ListView l && ViewModel.MultiSelect)
            {
                l.SelectAll();
            }
        }
    }
}
