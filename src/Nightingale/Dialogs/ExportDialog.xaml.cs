using Nightingale.Core.Export;
using Nightingale.Core.Models;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Dialogs
{
    public sealed partial class ExportDialog : ContentDialog
    {
        public ExportDialog()
        {
            this.InitializeComponent();
        }

        public ExportConfiguration ExportConfiguration { get; private set; }

        public void Load(IList<Workspace> workspaces) => ExportControl.ViewModel.Load(workspaces);

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ExportConfiguration = ExportControl.ViewModel.GetExportConfiguration();
        }
    }
}
