using Nightingale.Core.Interfaces;

namespace Nightingale.Navigation
{
    public interface IRootFrameNavigationService
    {
        void NavigateToMainPage(IStorage storageContext);
    }
}
