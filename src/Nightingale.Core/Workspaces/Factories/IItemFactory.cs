using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;

namespace Nightingale.Core.Workspaces.Factories
{
    /// <summary>
    /// Class for creating <see cref="Item"/>
    /// objects.
    /// </summary>
    public interface IItemFactory
    {
        /// <summary>
        /// Creates an <see cref="Item"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="parent">Optional. The parent of item.</param>
        /// <param name="childrenObservable">Optional. If false, this creates an item
        /// which will not observe changes in its own children items. If null or true,
        /// a regular item will be created.</param>
        Item Create(
            ItemType type,
            string name,
            Item parent = null,
            bool? childrenObservable = null,
            bool isTemp = false);

        /// <summary>
        /// Migrates old request object to new item type.
        /// </summary>
        Item Migrate(WorkspaceItem request);
    }
}
