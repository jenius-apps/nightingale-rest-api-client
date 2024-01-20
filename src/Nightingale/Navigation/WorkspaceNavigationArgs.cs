using Nightingale.History;
using Nightingale.ViewModels;

namespace Nightingale.Navigation
{
    public class WorkspaceNavigationArgs
    {
        public WorkspaceViewModel WorkspaceViewModel { get; set; }

        public HistoryViewModel HistoryViewModel { get; set; }
    }
}
