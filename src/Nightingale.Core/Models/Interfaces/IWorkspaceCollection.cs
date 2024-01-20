using Nightingale.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Nightingale.Core.Models.Interfaces
{
    public interface IWorkspaceCollection : IStorageItem
    {
        string Name { get; set; }

        bool IsExpanded { get; set; }

        int Position { get; set; }

        Authentication Authentication { get; set; }

        ObservableCollection<WorkspaceItem> Children { get; }
    }
}
