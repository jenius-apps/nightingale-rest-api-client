using Nightingale.Core.Interfaces;
using System;

namespace Nightingale.Core.Models
{
    public class RecentUrl : IStorageItem
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public ModifiedStatus Status { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ParentId { get; set; }
    }
}
