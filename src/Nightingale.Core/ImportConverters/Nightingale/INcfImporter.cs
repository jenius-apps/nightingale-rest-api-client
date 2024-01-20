using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.ImportConverters.Nightingale
{
    /// <summary>
    /// Interface for importing an NCF
    /// into the existing instance.
    /// Must be implemented on the platform
    /// due to differences in IO implementation.
    /// </summary>
    public interface INcfImporter
    {
        /// <summary>
        /// Imports the NCF file provided.
        /// </summary>
        /// <param name="storageFile">The file. Usually a StorageFile in UWP.</param>
        /// <returns>If the file contains workspaces, a list of workspaces are returned.</returns>
        Task<IList<Workspace>> ImportFileAsync(object storageFile);
    }
}
