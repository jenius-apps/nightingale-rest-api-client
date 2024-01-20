using Nightingale.Core.Http;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Extensions;
using Nightingale.Core.Workspaces.Models;
using System;

namespace Nightingale.Core.Workspaces.Factories
{
    /// <summary>
    /// Class for creating <see cref="Item"/>
    /// objects.
    /// </summary>
    public class ItemFactory : IItemFactory
    {
        private readonly IMethodsContainer _methodsContainer;

        public ItemFactory(
            IMethodsContainer methodsContainer)
        {
            _methodsContainer = methodsContainer
                ?? throw new ArgumentNullException(nameof(methodsContainer));
        }

        /// <inheritdoc/>
        public Item Create(
            ItemType type,
            string name,
            Item parent = null,
            bool? childrenObservable = null,
            bool isTemp = false)
        {
            if (childrenObservable.HasValue)
            {
                var i = new Item(childrenObservable.Value)
                {
                    Name = name?.Trim(),
                    Type = type,
                    Parent = parent,
                    IsTemporary = isTemp,
                    Method = _methodsContainer.GetFirst()
                };

                return i;
            }

            var item = new Item
            {
                Name = name?.Trim(),
                Type = type,
                Parent = parent,
                IsTemporary = isTemp,
                Method = _methodsContainer.GetFirst()
            };

            return item;
        }

        /// <inheritdoc/>
        public Item Migrate(WorkspaceItem item)
        {
            if (item == null)
            {
                return null;
            }

            Item newItem = null;

            if (item is WorkspaceRequest r)
            {
                newItem = Migrate(r);
            }
            else if (item is WorkspaceCollection c)
            {
                newItem = Migrate(c);
            }

            return newItem;
        }

        private Item Migrate(WorkspaceRequest r)
        {
            if (r == null)
            {
                return null;
            }

            var newItem = new Item
            {
                Name = r.Name,
                Auth = r.Authentication.DeepClone() as Authentication,
                Body = r.Body.DeepClone() as RequestBody,
                Url = r.Url.DeepClone() as Url,
                Method = r.Method,
                Type = ItemType.Request
            };
            newItem.Headers.DeepClone(r.Headers);
            newItem.ChainingRules.DeepClone(r.ChainingRules);

            return newItem;
        }

        private Item Migrate(WorkspaceCollection c)
        {
            if (c == null)
            {
                return null;
            }

            var newItem = new Item
            {
                Name = c.Name,
                Auth = c.Authentication.DeepClone() as Authentication,
                Type = ItemType.Collection,
                IsExpanded = c.IsExpanded
            };

            foreach (var child in c.Children)
            {
                var newChild = Migrate(child);
                if (newChild != null)
                {
                    newItem.Children.Add(newChild);
                }
            }

            return newItem;
        }
    }
}
