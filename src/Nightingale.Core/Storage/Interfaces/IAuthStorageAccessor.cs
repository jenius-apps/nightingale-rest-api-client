using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IAuthStorageAccessor
    {
        Task SaveAuthenticationAsync(Authentication auth, string parentId);

        Task<Authentication> GetAuthenticationAsync(string parentId, bool createNew = true);

        Task DeleteAuthenticationAsync(Authentication auth);

        Task DeleteAllAsync(IList<string> parentIds);
    }
}
