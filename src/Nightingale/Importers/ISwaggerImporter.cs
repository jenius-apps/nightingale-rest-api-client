using Nightingale.Core.Workspaces.Models;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Importers
{
    public interface ISwaggerImporter
    {
        Task<Item> ImportFileAsync(StorageFile storageFile);
    }
}
