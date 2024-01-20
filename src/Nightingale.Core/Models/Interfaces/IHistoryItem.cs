using System;

namespace Nightingale.Core.Models.Interfaces
{
    public interface IHistoryItem
    {
        DateTime LastUsedDate { get; set; }
    }
}
