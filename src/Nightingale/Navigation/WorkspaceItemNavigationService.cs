using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Mock.Services;
using Nightingale.Core.Workspaces.Models;
using Nightingale.ViewModels;
using Nightingale.Views;
using Nightingale.Views.NavigationParameters;

namespace Nightingale.Navigation
{
    public sealed class WorkspaceItemNavigationService : BaseNavigationService, IWorkspaceItemNavigationService
    {
        private readonly IServiceProvider _scope;

        public WorkspaceItemNavigationService(IServiceProvider scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        /// <inheritdoc/>
        public async Task<object> GetNavParams(Item item)
        {
            if (item == null)
            {
                return null;
            }

            object result = null;
            if (item.Type == ItemType.Request)
            {
                // Configure navigation parameters
                var urlBarViewModel = _scope.GetRequiredService<UrlBarViewModel>();
                var requestControlViewModel = _scope.GetRequiredService<RequestControlViewModel>();
                var authViewModel = _scope.GetRequiredService<AuthControlViewModel>();
                var requestBodyViewModel = _scope.GetRequiredService<RequestBodyViewModel>();
                var bodyControlViewModel = _scope.GetRequiredService<BodyControlViewModel>();
                var statusBarViewModel = _scope.GetRequiredService<StatusBarViewModel>();

                await urlBarViewModel.Initialize();
                requestControlViewModel.Request = item;
                requestControlViewModel.MockDataViewModel = new Mock.MockDataViewModel(
                    item.MockData,
                    _scope.GetRequiredService<IDeployService>());
                authViewModel.ActiveAuthModel = item.Auth;
                requestBodyViewModel.RequestBody = item.Body;
                bodyControlViewModel.WorkspaceResponse = item.Response;
                statusBarViewModel.ResponseStatus = item.Response;

                result = new RequestPageParameters
                {
                    RequestPageViewModel = _scope.GetRequiredService<RequestPageViewModel>(),
                    RequestControlViewModel = requestControlViewModel,
                    UrlBarViewModel = urlBarViewModel,
                    AuthControlViewModel = authViewModel,
                    RequestBodyViewModel = requestBodyViewModel,
                    BodyControlViewModel = bodyControlViewModel,
                    StatusBarViewModel = statusBarViewModel
                };
            }
            else if (item.Type == ItemType.Collection)
            {
                var vm = _scope.GetRequiredService<CollectionViewModel>();
                var authVm = _scope.GetRequiredService<AuthControlViewModel>();
                authVm.ActiveAuthModel = item.Auth;
                vm.SelectedCollection = item;
                result = new CollectionPageParameters
                {
                    CollectionViewModel = vm,
                    AuthControlViewModel = authVm
                };
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task NavigateTo(Item item)
        {
            EnsureFrameNotNull();

            if (item == null)
            {
                base.Frame.BackStack?.Clear();
                return;
            }

            Type pageType;
            object parameters = await GetNavParams(item);

            if (item.Type == ItemType.Collection)
            {
                pageType = typeof(DockedCollectionPage);
            }
            else
            {
                // We used to support request type, but
                // not since tabs were added.
                // If-else structure preserved in case future
                // types are added.
                return;
            }

            base.Frame.Navigate(pageType, parameters);
            base.Frame.BackStack?.Clear();
        }
    }
}
