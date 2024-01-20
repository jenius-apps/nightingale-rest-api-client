using System;
using Autofac;
using Nightingale.Core.Workspaces.Models;

namespace Nightingale.ViewModels.Factories
{
    public class CodeGeneratorViewModelFactory : ICodeGeneratorViewModelFactory
    {
        private readonly ILifetimeScope _scope;

        public CodeGeneratorViewModelFactory(ILifetimeScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public CodeGenPageViewModel CreateViewModel(Item workspaceItem)
        {
            var vm = _scope.Resolve<CodeGenPageViewModel>();
            vm.WorkspaceItem = workspaceItem ?? throw new ArgumentNullException(nameof(workspaceItem));
            return vm;
        }
    }
}
