using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Extensions;
using System;

namespace Nightingale.Core.Workspaces.Models
{
    /// <summary>
    /// Object designed to be a historical snapshot of a
    /// request item that was sent.
    /// </summary>
    public class HistoryItem : Item, IDeepCloneable
    {
        /// <summary>
        /// Used for deserializatoin.
        /// </summary>
        public HistoryItem() { }

        /// <summary>
        /// Creates a history item clone
        /// of the given item.
        /// </summary>
        public HistoryItem(Item request, DateTime lastUsed)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Cannot construct a history item with a null Item");
            }
            this.LastUsedDate = lastUsed;
            this.Name = request.Name;
            this.Type = request.Type;
            this.Method = request.Method;
            this.Url = new Url
            {
                Base = request.Url.Base
            };
            this.Body = request.Body.DeepClone() as RequestBody;
            this.Auth = request.Auth.DeepClone() as Authentication;
            this.Url.Queries.DeepClone(request.Url.Queries);
            this.Headers.DeepClone(request.Headers);
            this.ChainingRules.DeepClone(request.ChainingRules);
        }

        /// <summary>
        /// Last used date of this history item.
        /// </summary>
        public DateTime LastUsedDate { get; set; }

        public override object DeepClone()
        {
            var baseClone = base.DeepClone() as Item;
            if (baseClone == null)
            {
                return null;
            }

            var historyItem = new HistoryItem(baseClone, this.LastUsedDate);
            return historyItem;
        }
    }
}
