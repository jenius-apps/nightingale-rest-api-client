using Nightingale.Core.Interfaces;
using Nightingale.Core.Models.Interfaces;
using System;

namespace Nightingale.Core.Models
{
    public class HistoryCollection : WorkspaceCollection, IHistoryItem
    {
        public HistoryCollection()
        {
        }

        public DateTime LastUsedDate { get; set; }
    }
}
