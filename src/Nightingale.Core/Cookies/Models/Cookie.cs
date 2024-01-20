using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nightingale.Core.Cookies.Models
{
    /// <summary>
    /// A class representing an HTTP cookie.
    /// </summary>
    public class Cookie : ObservableBase, IStorageItem, IDeepCloneable
    {
        /// <summary>
        /// Gets or sets the cookie's domain.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the cookie's raw string.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// Parses the raw cookie string
        /// to return the cookie's name.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Raw) || !Raw.Contains("="))
                {
                    return "";
                }

                var split = Raw.Split('=');
                return split.FirstOrDefault();
            }
        }

        /// <summary>
        /// Parses the raw cookie string to return
        /// the cookie's value.
        /// </summary>
        public string Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Raw))
                {
                    return "";
                }

                var split = Raw.Split(';');

                foreach (var segment in split)
                {
                    if (segment.Contains("="))
                    {
                        var segmentSplit = segment.Split('=');
                        return segmentSplit.LastOrDefault();
                    }
                }

                return "";
            }
        }

        /// <summary>
        /// Gets or sets the cookie's Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the cooie
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public object DeepClone()
        {
            return new Cookie
            {
                Raw = this.Raw,
                Domain = this.Domain,
            };
        }
    }
}
