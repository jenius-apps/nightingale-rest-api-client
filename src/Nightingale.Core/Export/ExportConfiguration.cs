using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;

namespace Nightingale.Core.Export
{
    public class ExportConfiguration
    {
        public Item CollectionToExport { get; set; }

        public Workspace[] WorkspacesToExport { get; set; }

        public ExportFormat Format { get; set; }

        public ExportScope Scope { get; set; }
    }

    public enum ExportScope
    {
        Workspace,
        Collection
    }

    public enum ExportFormat
    {
        Nightingale,
        Postman
    }
}
