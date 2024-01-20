using Nightingale.Core.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Navigation
{
    public class RootFrameNavigationService : IRootFrameNavigationService
    {
        public void NavigateToMainPage(IStorage storageContext)
        {
            Frame frame = Window.Current.Content as Frame;

            if (frame == null)
            {
                return;
            }

            frame.Navigate(typeof(Views.MainPage2), storageContext);
            frame.BackStack.Clear();
        }
    }
}
