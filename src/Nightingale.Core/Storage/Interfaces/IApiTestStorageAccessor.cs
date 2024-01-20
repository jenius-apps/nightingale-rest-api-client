using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IApiTestStorageAccessor
    {
        Task SaveTestAsync(ApiTest apiTest, string parentId);

        Task<IList<ApiTest>> GetApiTestsAsync(string parentId);

        Task DeleteTestAsync(ApiTest apiTest);

        Task DeleteAllAsync(IList<string> parentIds);
    }
}
