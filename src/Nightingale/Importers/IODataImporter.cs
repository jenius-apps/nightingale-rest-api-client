using Nightingale.Core.Workspaces.Models;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Importers
{
    /// <summary>
    /// Interface for importing and converting
    /// OData files into Nightingale.
    /// </summary>
    public interface IODataImporter
    {
        /// <summary>
        /// Converts the content of the file from xml odata
        /// to Nightingale data and returns the converted <see cref="Item"/>.
        /// </summary>
        Task<Item> ImportFileAsync(StorageFile file);
    }
}