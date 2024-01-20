using Nightingale.Core.Workspaces.Models;
using System.Collections.Generic;

namespace Nightingale.Core.Workspaces.Extensions
{
    /// <summary>
    /// Extension methods for getting
    /// and setting temporary properties in
    /// <see cref="Item"/>.
    /// </summary>
    public static class ItemPropertyExtensions
    {
        public const string CollectionPivotIndex = "CollectionPivotIndex";
        public const string RequestPivotIndex = "RequestPivotIndex";
        public const string ResponsePivotIndex = "ResponsePivotIndex";

        private static readonly Dictionary<string, object> Defaults = new Dictionary<string, object>
        {
            { CollectionPivotIndex, 0L }, // int32 is stored in json as int64
            { RequestPivotIndex, 0L },
            { ResponsePivotIndex, 0L },
        };

        /// <summary>
        /// Retrieves the value of the given
        /// property key. Returns default value
        /// if property does not exist yet. Does not
        /// update the property list.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="item">The item whose property list will be checked.</param>
        /// <param name="propkey">The key of the property to find.</param>
        /// <returns>Returns the value of the property key.</returns>
        public static T GetProperty<T>(this Item item, string propkey)
        {
            if (!Defaults.ContainsKey(propkey) || item?.Properties == null)
            {
                return default;
            }

            if (!item.Properties.ContainsKey(propkey))
            {
                return (T)Defaults[propkey];
            }

            return (T)item.Properties[propkey];
        }

        /// <summary>
        /// Sets the value of the given property key.
        /// </summary>
        /// <param name="item">The item whose property list will be checked.</param>
        /// <param name="propkey">The key of the property to add.</param>
        /// <param name="value">The value of the property to add.</param>
        public static void SetProperty(this Item item, string propkey, object value)
        {
            if (!Defaults.ContainsKey(propkey) || item?.Properties == null)
            {
                return;
            }

            if (item.Properties.ContainsKey(propkey))
            {
                item.Properties[propkey] = value;
            }
            else
            {
                item.Properties.Add(propkey, value);
            }
        }
    }
}
