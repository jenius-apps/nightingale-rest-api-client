using System.Collections.Generic;

namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Interface for an item that contains
    /// authentication properties.
    /// </summary>
    public interface IAuth
    {
        /// <summary>
        /// Dictionary of authentication properties. 
        /// See <see cref="AuthConstants"/> for supported
        /// keys. See <see cref="AuthExtensions"/> for helper
        /// methods.
        /// </summary>
        Dictionary<string, string> AuthProperties { get; set; }
    }
}
