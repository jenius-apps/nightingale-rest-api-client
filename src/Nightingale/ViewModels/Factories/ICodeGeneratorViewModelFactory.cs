using Nightingale.Core.Workspaces.Models;

namespace Nightingale.ViewModels.Factories
{
    public interface ICodeGeneratorViewModelFactory
    {
        CodeGenPageViewModel CreateViewModel(Item workspaceItem);
    }
}
