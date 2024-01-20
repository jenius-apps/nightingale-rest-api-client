using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IStorageExporter
    {
        Task<string> ExportAsync();
    }
}
