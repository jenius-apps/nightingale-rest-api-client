using System.Collections.Generic;

namespace Nightingale.Core.Http
{
    /// <summary>
    /// Class containing default values
    /// for HTTP Methods.
    /// </summary>
    public class Method
    {
        /// <summary>
        /// List of default supported HTTP methods.
        /// </summary>
        public static string[] Defaults = new string[]
        {
            "GET",
            "POST",
            "PUT",
            "DELETE",
            "HEAD",
            "OPTIONS",
            "PATCH",
            "MERGE",
            "COPY"
        };

        public static Dictionary<string, string> ShortNameMap = new Dictionary<string, string>
        {
            { "GET", "GET" },
            { "POST", "POST" },
            { "PUT", "PUT" },
            { "DELETE", "DEL" },
            { "HEAD", "HEAD" },
            { "OPTIONS", "OPT" },
            { "PATCH", "PATC" },
            { "MERGE", "MRG" },
            { "COPY", "COPY" },
        };

        /// <summary>
        /// Returns the short name of the given
        /// method. If the method is not in the default set,
        /// the first three characters will be returned.
        /// </summary>
        /// <param name="method">The method to shorten.</param>
        /// <returns>A shortened name for the given HTTP method.</returns>
        public static string GetShortName(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                return "--";
            }

            if (ShortNameMap.ContainsKey(method))
            {
                return ShortNameMap[method];
            }

            return method.Substring(0, 3);
        }
    }
}
