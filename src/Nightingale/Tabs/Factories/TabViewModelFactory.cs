using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using Nightingale.Navigation;
using Nightingale.Tabs.Services;
using Nightingale.ViewModels;
using Nightingale.Views.NavigationParameters;
using System;
using System.Threading.Tasks;

namespace Nightingale.Tabs.Factories
{
    /// <summary>
    /// Factory for tab view models.
    /// </summary>
    public class TabViewModelFactory : ITabViewModelFactory
    {
        private readonly IWorkspaceItemNavigationService _navService;
        private readonly IServiceProvider _scope;

        public TabViewModelFactory(IServiceProvider scope, IWorkspaceItemNavigationService navService)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            _navService = navService ?? throw new ArgumentNullException(nameof(navService));
        }

        /// <inheritdoc/>
        public async Task<RequestViewModel> Create(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item can't be null");
            }

            var navigationParameters = await _navService.GetNavParams(item);
            var result = new RequestViewModel(
                navigationParameters as RequestPageParameters,
                _scope.GetRequiredService<IWorkspaceTreeModifier>(),
                _scope.GetRequiredService<ITabCollectionContainer>(),
                _scope.GetRequiredService<ITelemetry>());
            return result;
        }
    }
}
