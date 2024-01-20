using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Export
{
    public interface IExportService
    {
        /// <summary>
        /// Pops export dialog that allows users to choose
        /// which workspaces to export. Performs export
        /// once user confirms.
        /// </summary>
        /// <returns>
        /// Returns path to exported file. Returns null or empty
        /// if cancelled or unsuccessful.
        /// </returns>
        Task<string> ExportAsync(IList<Workspace> workspaces);

        Task<string> ExportCollectionAsync(Item collection, ExportFormat format);
    }
}