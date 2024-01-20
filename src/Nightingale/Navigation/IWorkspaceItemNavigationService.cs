using Nightingale.Core.Workspaces.Models;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Navigation
{
    /// <summary>
    /// Interface for navigating to different items
    /// on the workspace page.
    /// </summary>
    public interface IWorkspaceItemNavigationService
    {
        /// <summary>
        /// Navigates current frame to the given item.
        /// </summary>
        /// <param name="item">The item to navigate to.</param>
        Task NavigateTo(Item item);

        /// <summary>
        /// Constructs the navigation parameters that would
        /// be required for a page to receive based on the
        /// given item's type.
        /// </summary>
        /// <param name="item">The item used to construct the navigation parameters.</param>
        /// <returns>Navigation parameters baed on the given item's type. Returns null if item is null.</returns>
        Task<object> GetNavParams(Item item);

        void GoBack();

        void SetFrame(Frame frame);
    }
}
