using JeniusApps.Nightingale.Converters.OData;
using Microsoft.Toolkit.Diagnostics;
using Newtonsoft.Json;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Importers
{
    /// <summary>
    /// Imports XML OData files.
    /// </summary>
    public class ODataImporter : IODataImporter
    {
        private readonly IODataConverter _converter;

        public ODataImporter(IODataConverter converter)
        {
            Guard.IsNotNull(converter, nameof(converter));

            _converter = converter;
        }

        /// <inheritdoc/>
        public async Task<Item> ImportFileAsync(StorageFile file)
        {
            if (file == null)
            {
                return null;
            }

            string content = await FileIO.ReadTextAsync(file);
            var dtoItem = _converter.ConvertCollection(content);
            var localItem = JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(dtoItem));
            return localItem;
        }
    }
}
