using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IStorageImporter
    {
        Task ImportAsync(string contents);
    }
}
