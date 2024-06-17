using System;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Workspaces.Models;

namespace Nightingale.ViewModels.Factories
{
    public class CodeGeneratorViewModelFactory : ICodeGeneratorViewModelFactory
    {
        private readonly IServiceProvider _scope;

        public CodeGeneratorViewModelFactory(IServiceProvider scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public CodeGenPageViewModel CreateViewModel(Item workspaceItem)
        {
            var vm = _scope.GetRequiredService<CodeGenPageViewModel>();
            vm.WorkspaceItem = workspaceItem ?? throw new ArgumentNullException(nameof(workspaceItem));
            return vm;
        }
    }
}
