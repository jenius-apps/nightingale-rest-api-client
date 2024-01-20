using Nightingale.Core.Interfaces;
using System.Collections.Generic;

namespace Nightingale.Core.Workspaces.Extensions
{
    /// <summary>
    /// Extensions for deep copy.
    /// </summary>
    public static class DeepCloneExtension
    {
        /// <summary>
        /// Deep copies list of structs.
        /// </summary>
        /// <typeparam name="T">A struct.</typeparam>
        /// <param name="target">List to copy items into.</param>
        /// <param name="source">List to copy items from.</param>
        public static void DeepCloneStructs<T>(this IList<T> target, IList<T> source) 
            where T : struct
        {
            if (target == null || source == null || source.Count == 0)
            {
                return;
            }

            foreach (var item in source)
            {
                var copy = item;
                target.Add(copy);
            }
        }

        /// <summary>
        /// Deep copies the contents of a source list
        /// to the target list. This assumes that the
        /// target list is empty.
        /// </summary>
        /// <typeparam name="T">A class that implements <see cref="IDeepCloneable"/>.</typeparam>
        /// <param name="target">List to copy items into.</param>
        /// <param name="source">List to copy items from.</param>
        public static void DeepClone<T>(this IList<T> target, IList<T> source)
            where T : IDeepCloneable
        {
            if (target == null || source == null || source.Count == 0)
            {
                return;
            }

            foreach (var item in source)
            {
                var copy = item.DeepClone();

                if (copy is T t)
                {
                    target.Add(t);
                }
            }
        }
    }
}
