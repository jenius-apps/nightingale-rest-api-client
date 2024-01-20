using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using Nightingale.Tabs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nightingale.Tabs.Factories
{
    /// <summary>
    /// Class for generating a TabCollectionView.
    /// </summary>
    public class TabCollectionViewFactory : ITabCollectionViewFactory
    {
        private readonly ITabViewModelFactory _tabVmFactory;

        public TabCollectionViewFactory(ITabViewModelFactory tabVmFactory)
        {
            _tabVmFactory = tabVmFactory
                ?? throw new ArgumentNullException(nameof(tabVmFactory));
        }

        /// <inheritdoc/>
        public async Task<TabCollectionView> Create(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }

            var tabs = new TabCollectionView(workspace);
            tabs.IgnoreChanges = true;

            var idsFromTemp = workspace.TempItems.Where(x => workspace.OpenItemIds.Contains(x.Id)).Select(x => x.Id);
            var idsFromHistory = workspace.HistoryItems.Where(x => workspace.OpenItemIds.Contains(x.Id)).Select(x => x.Id);
            var idsFromTree = workspace.OpenItemIds.Where(x => !idsFromTemp.Contains(x) && !idsFromHistory.Contains(x));
            IList<Item> itemsFromTree = ItemFinder.GetItemsFromTree(idsFromTree.ToArray(), workspace.Items);

            foreach (var id in workspace.OpenItemIds)
            {
                var tempItem = workspace.TempItems.FirstOrDefault(x => x.Id == id);
                if (tempItem != null)
                {
                    tabs.Add(await _tabVmFactory.Create(tempItem));
                    continue;
                }

                var historyItem = workspace.HistoryItems.FirstOrDefault(x => x.Id == id);
                if (historyItem != null)
                {
                    tabs.Add(await _tabVmFactory.Create(historyItem));
                    continue;
                }

                var treeItem = itemsFromTree.FirstOrDefault(x => x.Id == id);
                if (treeItem != null)
                {
                    tabs.Add(await _tabVmFactory.Create(treeItem));
                }
            }

            tabs.IgnoreChanges = false;
            return tabs;
        }
    }
}
