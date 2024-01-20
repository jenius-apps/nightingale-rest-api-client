using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IStorageMigrator
    {
        Task MigrateAsync();
    }
}
