using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Models;
using Nightingale.History;
using Nightingale.Tabs.Factories;
using Nightingale.Tabs.Models;
using Nightingale.Tabs.Services;
using Nightingale.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Navigation
{
    public sealed class WorkspaceNavigationService : BaseNavigationService, IWorkspaceNavigationService
    {
        private readonly IServiceProvider _scope;
        private readonly ITabCollectionViewFactory _tabsFactory;
        private readonly ITabCollectionContainer _tabsContainer;
        private readonly Dictionary<Workspace, WorkspaceViewModel> _cache = new Dictionary<Workspace, WorkspaceViewModel>();

        public WorkspaceNavigationService(
            IServiceProvider scope,
            ITabCollectionViewFactory tabsFactory,
            ITabCollectionContainer tabsContainer)
        {
            _scope = scope 
                ?? throw new ArgumentNullException(nameof(scope));
            _tabsFactory = tabsFactory 
                ?? throw new ArgumentNullException(nameof(tabsFactory));
            _tabsContainer = tabsContainer
                ?? throw new ArgumentNullException(nameof(tabsContainer));
        }

        public async void NavigateTo(Workspace workspace)
        {
            EnsureFrameNotNull();

            if (workspace == null)
            {
                base.Frame.Navigate(typeof(Views.NoWorkspacesPage));
            }
            else
            {
                var viewModel = await GetVmAsync(workspace);
                HistoryViewModel historyViewModel = _scope.GetRequiredService<HistoryViewModel>();
                historyViewModel.Initialize(workspace);

                var args = new WorkspaceNavigationArgs
                {
                    WorkspaceViewModel = viewModel,
                    HistoryViewModel = historyViewModel
                };

                base.Frame.Navigate(typeof(Views.WorkspacePage), args);
            }
        }

        private async Task<WorkspaceViewModel> GetVmAsync(Workspace workspace)
        {
            if (_cache.ContainsKey(workspace))
            {
                var result = _cache[workspace];
                _tabsContainer.SetTabCollection(result.Tabs as TabCollectionView, result.TabChangedHandler);
                return result;
            }

            WorkspaceViewModel viewModel = _scope.GetRequiredService<WorkspaceViewModel>();
            viewModel.SelectedWorkspace = workspace;
            TabCollectionView tabs = await _tabsFactory.Create(workspace);
            viewModel.Tabs = tabs;
            _tabsContainer.SetTabCollection(tabs, viewModel.TabChangedHandler);

            if (_cache.ContainsKey(workspace))
            {
                _cache[workspace] = viewModel;
            }
            else
            {
                _cache.Add(workspace, viewModel);
            }

            return viewModel;
        }
    }
}
