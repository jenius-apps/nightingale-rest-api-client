using Nightingale.Core.Interfaces;
using Nightingale.Core.Workspaces.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nightingale.Core.Models
{
    /// <summary>
    /// An object representing
    /// a URL string.
    /// </summary>
    public class Url : IDeepCloneable
    {
        /// <summary>
        /// A string representation of the URL without queries.
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// The queries in the URL string.
        /// This must remain synced with the
        /// raw string at all times.
        /// </summary>
        public ObservableCollection<Parameter> Queries { get; } = new ObservableCollection<Parameter>();

        /// <summary>
        /// A string representation of the full URL.
        /// </summary>
        public override string ToString()
        {
            var querystring = GetQueryString();
            Base = Base ?? "";
            if (!string.IsNullOrEmpty(querystring) && !Base.EndsWith("?"))
            {
                Base += "?";
            }
            return Base + querystring;
        }

        /// <summary>
        /// A string representation of the queries
        /// formatted like key1=value1&key2=value2.
        /// </summary>
        /// <returns>String represnting the URL's queries.</returns>
        public string GetQueryString()
        {
            if (Queries == null || Queries.Count == 0)
            {
                return "";
            }

            var queryItems = new List<string>();
            foreach (var param in Queries.Where(x => x.Enabled))
            {
                string queryItem = param.Key + (string.IsNullOrEmpty(param.Value) ? "" : $"={param.Value}");
                queryItems.Add(queryItem);
            }

            return string.Join("&", queryItems);
        }

        /// <summary>
        /// Updates the stored URL string
        /// and parses the queries into the
        /// queries collection.
        /// </summary>
        /// <param name="newUrl">The new URL string</param>
        public void Set(string newUrl)
        {
            if (string.IsNullOrEmpty(newUrl))
            {
                Base = "";
                ClearEnabledQueries();
                return;
            }

            if (!newUrl.Contains("?"))
            {
                Base = newUrl;
                ClearEnabledQueries();
                return;
            }

            string[] urlSplit = newUrl.Split(new[] { '?' }, 2);
            Base = urlSplit[0] + "?";
            var querystring = urlSplit[1];

            // Fast path for when the query
            // just looks like "?&&&&...".
            if (querystring.Trim('&') == "")
            {
                Base += querystring;
                ClearEnabledQueries();
                return;
            }

            string[] keyvalues = querystring.Split('&');
            int disabledQueryCount = 0;
            int currentQueryIndex = 0;

            // Process each new query.
            for (int newKeyValIndex = 0; newKeyValIndex < keyvalues.Length; newKeyValIndex++)
            {
                // Don't edit the queries which are disabled.
                while (Queries.Count > currentQueryIndex 
                    && !Queries[currentQueryIndex].Enabled)
                {
                    disabledQueryCount++;
                    currentQueryIndex++;
                }

                // Ensure a parameter is loaded for the index.
                while (Queries.Count < currentQueryIndex + 1)
                {
                    Queries.Add(new Parameter(isEnabled: true));
                }

                string currentKeyValue = keyvalues[newKeyValIndex];
                if (string.IsNullOrEmpty(currentKeyValue))
                {
                    Queries[currentQueryIndex].Key = "";
                    Queries[currentQueryIndex].Value = "";
                    currentQueryIndex++;
                    continue;
                }

                // Fast path for when only key is populated
                // so far.
                if (!currentKeyValue.Contains("="))
                {
                    Queries[currentQueryIndex].Key = currentKeyValue;
                    Queries[currentQueryIndex].Value = "";
                    currentQueryIndex++;
                    continue;
                }

                string[] pair = currentKeyValue.Split(new[] { '=' }, 2);

                // Apply key and value.
                // Append '=' to key if value is null or empty so the
                // full URL doesn't lose the '='.
                Queries[currentQueryIndex].Key = pair[0] + (string.IsNullOrEmpty(pair[1]) ? "=" : "");
                Queries[currentQueryIndex].Value = pair[1];
                currentQueryIndex++;
            }

            // Remove excess query entries.
            for (int c = Queries.Count - 1; c > keyvalues.Length - 1 + disabledQueryCount; c--)
            {
                Queries.RemoveAt(c);
            }
        }

        private void ClearEnabledQueries()
        {
            if (Queries == null || Queries.Count == 0)
            {
                return;
            }

            int i = Queries.Count - 1;
            while (i >= 0)
            {
                if (Queries[i].Enabled)
                {
                    Queries.RemoveAt(i);
                }

                i--;
            }
        }

        /// <inheritdoc/>
        public object DeepClone()
        {
            var other = new Url
            {
                Base = this.Base
            };
            other.Queries.DeepClone(this.Queries);
            return other;
        }
    }
}
