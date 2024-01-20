using Nightingale.Core.Workspaces.Models;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Importers
{
    public interface IPostmanImporter
    {
        Task<Item> ImportFileAsync(StorageFile storageFile);
    }
}
