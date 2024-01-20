using Nightingale.Core.Models;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Navigation
{
    public interface IWorkspaceNavigationService
    {
        void NavigateTo(Workspace workspace);

        void GoBack();

        void SetFrame(Frame frame);
    }
}
