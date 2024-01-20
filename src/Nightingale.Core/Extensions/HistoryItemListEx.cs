using Nightingale.Core.Models.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Extensions
{
    public static class HistoryItemListEx
    {
        public static IList<T> NewestToOldest<T>(this IList<T> items) where T : IHistoryItem
        {
            return items.OrderByDescending(x => x.LastUsedDate).ToList();
        }
    }
}
