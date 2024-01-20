using Nightingale.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Storage.Interfaces
{
    public interface IParameterStorageAccessor
    {
        Task SaveParameterAsync(Parameter parameter, string parentId);

        Task<IList<Parameter>> GetParametersAsync(string parentId);

        Task DeleteParameterAsync(Parameter parameter);

        Task DeleteAllAsync(IList<string> parentIds);
    }
}
