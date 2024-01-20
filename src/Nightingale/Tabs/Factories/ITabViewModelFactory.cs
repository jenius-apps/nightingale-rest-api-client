using Nightingale.Core.Workspaces.Models;
using Nightingale.ViewModels;
using System.Threading.Tasks;

namespace Nightingale.Tabs.Factories
{
    /// <summary>
    /// Interface for tab view model factory.
    /// </summary>
    public interface ITabViewModelFactory
    {
        /// <summary>
        /// Creates a view model representing the tab.
        /// </summary>
        /// <param name="item">The tab item.</param>
        /// <returns>The view model.</returns>
        Task<RequestViewModel> Create(Item item);
    }
}