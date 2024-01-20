using Nightingale.Core.Extensions;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nightingale.Core.Workspaces.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Item"/> class.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Returns all headers for an item with distinct
        /// keys. Child headers override parent headers.
        /// </summary>
        /// <param name="item">The item whose headers will be retrieved.</param>
        /// <param name="activeOnly">Filters for active headers if true.</param>
        /// <returns>List of all distinct direct and inherited headers.</returns>
        public static IList<Parameter> GetHeaders(this Item item, bool activeOnly = true)
        {
            if (item == null)
            {
                return new List<Parameter>();
            }

            var result = activeOnly
                ? item.Headers.GetActive().ToList()
                : item.Headers.ToList();

            var inheritedHeaders = item.GetInheritedHeaders(activeOnly);

            foreach (var h in inheritedHeaders)
            {
                if (result.Any(x => x.Key == h.Key))
                {
                    continue;
                }

                result.Add(h);
            }

            return result;
        }

        /// <summary>
        /// Returns all inherited headers for the given item.
        /// </summary>
        /// <param name="item">The item to process.</param>
        /// <param name="activeOnly">Filters for active headers if true.</param>
        /// <returns>List of all inherited headers.</returns>
        public static IList<Parameter> GetInheritedHeaders(this Item item, bool activeOnly = true)
        {
            if (item?.Parent == null)
            {
                return new List<Parameter>();
            }

            var parentHeaders = activeOnly
                ? item.Parent.Headers.GetActive().ToList()
                : item.Parent.Headers.ToList();

            var ancestorHeaders = item.Parent.GetInheritedHeaders(activeOnly);
            if (ancestorHeaders != null && ancestorHeaders.Count > 0)
            {
                parentHeaders.AddRange(ancestorHeaders);
            }

            return parentHeaders;
        }

        /// <summary>
        /// Returns all inherited queries for the given item.
        /// </summary>
        /// <param name="item">The item to process.</param>
        /// <param name="activeOnly">Filters for active queries if true.</param>
        /// <returns>List of all inherited queries.</returns>
        public static IList<Parameter> GetInheritedQueries(this Item item, bool activeOnly = true)
        {
            if (item?.Parent == null)
            {
                return new List<Parameter>();
            }

            List<Parameter> parentQueries = activeOnly
                ? item.Parent.Url.Queries.GetActive().ToList()
                : item.Parent.Url.Queries.ToList();

            var ancestorQueries = item.Parent.GetInheritedQueries(activeOnly: activeOnly);
            if (ancestorQueries != null && ancestorQueries.Count > 0)
            {
                parentQueries.AddRange(ancestorQueries);
            }

            return parentQueries;
        }

        /// <summary>
        /// Returns all queries for an item with distinct
        /// keys. Child queries override parent queries.
        /// </summary>
        /// <param name="item">The item whose queries will be retrieved.</param>
        /// <param name="activeOnly">Filters for active queries if true.</param>
        /// <returns>List of all distinct direct and inherited queries.</returns>
        public static IList<Parameter> GetDistinctQueries(this Item item, bool activeOnly = true)
        {
            if (item == null)
            {
                return new List<Parameter>();
            }

            List<Parameter> result = activeOnly
                ? item.Url.Queries.GetActive().ToList()
                : item.Url.Queries.ToList();

            var inheritedQueries = item.GetInheritedQueries(activeOnly: activeOnly);

            foreach (var q in inheritedQueries)
            {
                if (result.Any(x => x.Key == q.Key))
                {
                    continue;
                }

                result.Add(q);
            }

            return result;
        }

        /// <summary>
        /// Returns the value of the given header key.
        /// Does not resolve variables in the value. Returns null
        /// if header not found.
        /// </summary>
        public static string TryGetHeader(this Item item, string key)
        {
            if (item?.Headers == null || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            Parameter header = item.Headers
                .GetActive()
                .FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            return header?.Value;
        }

        /// <summary>
        /// Determines if the given item
        /// is in fact an ancestor of the descendant.
        /// If descendant equals ancestor, false is returned.
        /// If either descendant or ancestor are null, false is returned.
        /// </summary>
        /// <param name="descendant">The item whose ancestor will be checked.</param>
        /// <param name="ancestor">The item that may be the ancestor.</param>
        public static bool IsChildOf(this Item descendant, Item ancestor)
        {
            if (descendant == null || ancestor == null || descendant == ancestor)
            {
                return false;
            }

            // Scout helps break out of loops
            Item scout = descendant.Parent?.Parent;
            Item current = descendant.Parent;

            while (current != null && current != scout)
            {
                if (current == ancestor)
                {
                    return true;
                }

                scout = scout?.Parent?.Parent;
                current = current.Parent;
            }

            return false;
        }

        /// <summary>
        /// Searches for the top-level auth in the inheritance tree.
        /// </summary>
        /// <param name="item">The item whose auth will be determined.</param>
        /// <returns>The inherited authentication.</returns>
        public static Authentication GetAuthInheritance(this Item item)
        {
            if (item?.Auth == null)
            {
                return null;
            }

            Item currentItem = item;
            while (currentItem?.Auth?.AuthType == AuthType.InheritParent
                && currentItem.Parent != null)
            {
                currentItem = currentItem.Parent;
            }

            return currentItem?.Auth;
        }
    }
}
