using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IRequestBodyStorageAccessor
    {
        Task SaveBodyAsync(RequestBody body, string parentId);

        Task<RequestBody> GetBodyAsync(string parentId);

        Task DeleteBodyAsync(RequestBody body);

        Task DeleteAllAsync(IList<string> parentIds);
    }
}
