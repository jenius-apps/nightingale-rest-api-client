using Nightingale.Core.Export;
using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nightingale.Core.ViewModels
{
    public class ExportControlViewModel : ObservableBase
    {
        private int _formatIndex;

        public int FormatIndex
        {
            get => _formatIndex;
            set
            {
                _formatIndex = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MultiSelect));
            }
        }

        public bool MultiSelect
        {
            get
            {
                switch ((ExportFormat)FormatIndex)
                {
                    case ExportFormat.Nightingale:
                        return true;
                    case ExportFormat.Postman:
                        return false;
                    default:
                        return false;
                }
            }
        }

        public ObservableCollection<Workspace> Workspaces { get; } = new ObservableCollection<Workspace>();

        public List<Workspace> SelectedWorkspaces { get; } = new List<Workspace>();

        public void Load(IList<Workspace> workspaces)
        {
            Workspaces.Clear();
            if (workspaces == null)
            {
                return;
            }

            foreach (var w in workspaces)
            {
                Workspaces.Add(w);
            }
        }

        public ExportConfiguration GetExportConfiguration()
        {
            return new ExportConfiguration
            {
                Format = (ExportFormat)FormatIndex,
                WorkspacesToExport = SelectedWorkspaces.ToArray()
            };
        }
    }
}
