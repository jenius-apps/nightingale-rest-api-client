using Nightingale.Core.Models;
using System.Collections.Generic;

namespace Nightingale.Core.Http
{
    /// <summary>
    /// Interface for storing a reference
    /// to the active workspace's list of HTTP
    /// methods. This allows any other class to
    /// access the methods of the workspace.
    /// </summary>
    public interface IMethodsContainer
    {
        /// <summary>
        /// Returns reference to list of methods configured
        /// for the workspace.
        /// </summary>
        IList<string> GetMethods();

        /// <summary>
        /// Sets the method list reference in the container
        /// using the method list of the given workspace.
        /// If the workspace's methods list is empty,
        /// it will be populated with the detaults.
        /// </summary>
        /// <param name="w">The workspace whose method list will be used.</param>
        void SetMethods(Workspace w);

        /// <summary>
        /// Updates the list in-place.
        /// </summary>
        /// <param name="methods">List of HTTP methods.</param>
        void UpdateMethods(IList<string> methods);

        /// <summary>
        /// Safely retrieve first method
        /// in list. Returns null if list is
        /// uninitialized or empty.
        /// </summary>
        /// <returns>An HTTP method.</returns>
        string GetFirst();
    }
}
