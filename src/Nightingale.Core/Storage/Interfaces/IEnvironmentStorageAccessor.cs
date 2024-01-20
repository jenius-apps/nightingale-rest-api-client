using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IEnvironmentStorageAccessor
    {
        Task SaveEnvironmentAsync(Environment environment, string parentId);

        Task<IList<Environment>> GetEnvironmentsAsync(string parentId);

        Task DeleteEnvironmentAsync(Environment environment);

        Task DeleteAllAsync(IList<string> parentIds);
    }
}
