using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Nightingale.Dialogs
{
    public interface IEnvironmentDialogService
    {
        Task<ManageEnvironmentsDialog> OpenEnvironmentDialog(ObservableCollection<Core.Models.Environment> list);
    }
}
