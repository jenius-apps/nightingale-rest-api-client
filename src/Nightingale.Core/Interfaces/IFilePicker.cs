using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nightingale.Core.Interfaces
{
    public interface IFilePicker
    {
        Task<string> PickFileAsync();

        Task<IList<string>> PickFilesAsync();
    }
}
