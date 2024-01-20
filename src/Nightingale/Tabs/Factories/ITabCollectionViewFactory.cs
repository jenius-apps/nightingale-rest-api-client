using Nightingale.Core.Models;
using Nightingale.Tabs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Tabs.Factories
{
    /// <summary>
    /// Interface for creating TabCollectionView.
    /// </summary>
    public interface ITabCollectionViewFactory
    {
        /// <summary>
        /// Initializes a TabCollectionView for the given workspace.
        /// </summary>
        /// <param name="workspace">The workspace used to initialize the tab collection.</param>
        Task<TabCollectionView> Create(Workspace workspace);
    }
}
