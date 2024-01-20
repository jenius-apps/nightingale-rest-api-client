using Nightingale.Core.Workspaces.Models;
using System;
using System.Threading.Tasks;

namespace Nightingale.Core.Workspaces.Services
{
    /// <summary>
    /// Interface for modifying the current
    /// workspace's list of history items.
    /// </summary>
    public interface IHistoryListModifier
    {
        /// <summary>
        /// Converts an item to a
        /// history item and adds it to
        /// the history list.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="dateTime">Last used datetime of item.</param>
        Task AddAsync(Item item, DateTime dateTime);

        /// <summary>
        /// Clears all history items for the current
        /// workspace. Returns true if successful.
        /// </summary>
        Task<bool> ClearAsync();

        /// <summary>
        /// Deletes the given history item.
        /// Returns true if successful.
        /// </summary>
        Task<bool> DeleteAsync(HistoryItem item);
    }
}
