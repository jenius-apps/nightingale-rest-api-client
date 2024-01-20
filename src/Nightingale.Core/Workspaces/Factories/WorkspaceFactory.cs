using Nightingale.Core.Http;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nightingale.Core.Workspaces.Factories
{
    /// <summary>
    /// Class for generating workspace objects.
    /// </summary>
    public class WorkspaceFactory : IWorkspaceFactory
    {
        private readonly IEnvFactory _envFactory;
        private readonly IItemFactory _itemFactory;

        public WorkspaceFactory(
            IEnvFactory envFactory,
            IItemFactory itemFactory)
        {
            _envFactory = envFactory ??
                throw new ArgumentNullException(nameof(envFactory));
            _itemFactory = itemFactory ??
                throw new ArgumentNullException(nameof(itemFactory));
        }

        /// <inheritdoc/>
        public Workspace Create(string name)
        {
            var w = new Workspace
            {
                Id = Guid.NewGuid().ToString(),
                Name = name?.Trim() ?? "Untitled",
                ParentId = "root",
            };

            w.Environments.Add(_envFactory.CreateBase(true));
            w.Methods = new List<string>(Method.Defaults);
            
            // Initialize prepopulated request item
            var initialItem = _itemFactory.Create(Models.ItemType.Request, "My request");
            initialItem.Method = w.Methods.FirstOrDefault();
            w.Items.Add(initialItem);

            return w;
        }
    }
}
