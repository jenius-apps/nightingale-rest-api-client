using Nightingale.Core.Workspaces.Models;
using System.Collections.Specialized;

namespace Nightingale.Core.Workspaces.EventHandlers
{
    /// <summary>
    /// Class for static event handlers
    /// for anything that handles items.
    /// </summary>
    public class ItemEventHandlers
    {
        /// <summary>
        /// Changes the parent property
        /// of each newly added item to point
        /// to the given parent <see cref="Item"/>.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Args for <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        /// <param name="parent">The parent <see cref="Item"/> of the newly added item.</param>
        public static void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e, Item parent)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var child in e.NewItems)
                {
                    if (child is Item i)
                    {
                        i.Parent = parent;
                    }
                }
            }
        }
    }
}
