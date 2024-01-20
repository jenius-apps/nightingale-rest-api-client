using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Importers
{
    public interface IInsomniaImporter
    {
        Task<IList<Workspace>> ImportFileAsync(StorageFile storageFile);
    }
}
