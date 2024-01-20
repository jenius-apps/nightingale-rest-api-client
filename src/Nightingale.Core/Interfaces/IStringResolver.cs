using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Core.Interfaces
{
    public interface IStringResolver
    {
        /// <summary>
        /// Replaces variable keys in the string with their
        /// corresponding values.
        /// </summary>
        /// <param name="raw">The string to resolve.</param>
        /// <param name="variables">List of <see cref="IParameter"/> to use for resolving.</param>
        /// <returns>The string with all the variables resolved.</returns>
        string ResolveString(string raw, IList<IParameter> variables);

        /// <summary>
        /// Replaces variable keys in the string with their
        /// corresponding values.
        /// </summary>
        /// <param name="raw">The string to resolve.</param>
        /// <param name="variables">List of <see cref="IParameter"/> to use for resolving.</param>
        /// <returns>The string with all the variables resolved.</returns>
        Task<string> ResolveStringAsync(string raw, IList<IParameter> variables);
    }
}
