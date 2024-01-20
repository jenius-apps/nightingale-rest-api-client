using Nightingale.Core.Workspaces.Models;
using System.Collections.Generic;

namespace Nightingale.Core.Workspaces.Services
{
    /// <summary>
    /// Class with methods for searching for items
    /// within the item tree.
    /// </summary>
    public class ItemFinder
    {
        /// <summary>
        /// Retrieves items in the tree using the given list of Ids.
        /// </summary>
        /// <param name="idsToFind">Ids of items to find.</param>
        /// <param name="rootItems">Root list of items to search through.</param>
        /// <returns></returns>
        public static IList<Item> GetItemsFromTree(
            IList<string> idsToFind,
            IList<Item> rootItems)
        {
            var queue = new Queue<Item>(rootItems);
            return BfsFind(queue, idsToFind, new List<Item>());
        }

        private static IList<Item> BfsFind(Queue<Item> queue, IList<string> idsToFind, IList<Item> resultList)
        {
            if (queue == null
                || queue.Count == 0
                || idsToFind.Count == resultList.Count)
            {
                return resultList;
            }

            var current = queue.Dequeue();
            if (idsToFind.Contains(current.Id))
            {
                resultList.Add(current);
            }

            if (idsToFind.Count == resultList.Count)
            {
                return resultList;
            }

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }

            return BfsFind(queue, idsToFind, resultList);
        }
    }
}
