using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nightingale.Core.Exceptions;
using Nightingale.Core.Workspaces.Models;
using PST = Postman.NET.Collections.Models;
using Windows.Storage;
using JeniusApps.Nightingale.Converters.Postman;

namespace Nightingale.Importers
{
    /// <summary>
    /// Class for importing postman collections into Nightingale.
    /// </summary>
    public class PostmanImporter : IPostmanImporter
    {
        private readonly IPostmanConverter _postmanConverter;

        public PostmanImporter(IPostmanConverter postmanConverter)
        {
            _postmanConverter = postmanConverter ?? throw new ArgumentNullException(nameof(postmanConverter));
        }

        /// <inheritdoc/>
        public async Task<Item> ImportFileAsync(StorageFile cachedFile)
        {
            if (cachedFile == null)
            {
                return null;
            }

            if (cachedFile.ContentType != "application/json")
            {
                throw new UnsupportedFileException("Unsupported file type", cachedFile.ContentType, cachedFile.Name);
            }

            string content = await FileIO.ReadTextAsync(cachedFile);
            PST.Collection postmanCollection = JsonConvert.DeserializeObject<PST.Collection>(content);

            if (!PostmanConverter.SupportedSchemas.Any(x => x == postmanCollection?.Info?.Schema))
            {
                throw new UnsupportedFileException("Unsupported schema", postmanCollection?.Info?.Schema, cachedFile.Name);
            }

            var dtoCollection = _postmanConverter.ConvertCollection(postmanCollection);
            var localCollection = JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(dtoCollection));
            return localCollection;
        }
    }
}
