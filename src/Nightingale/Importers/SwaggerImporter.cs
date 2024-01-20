using System;
using System.Threading.Tasks;
using JeniusApps.Nightingale.Converters.Swagger;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using Nightingale.Core.Exceptions;
using Nightingale.Core.Workspaces.Models;
using Windows.Storage;

namespace Nightingale.Importers
{
    public class SwaggerImporter : ISwaggerImporter
    {
        private readonly ISwaggerConverter _swaggerConverter;

        public SwaggerImporter(ISwaggerConverter swaggerConverter)
        {
            _swaggerConverter = swaggerConverter ?? throw new ArgumentNullException(nameof(swaggerConverter));
        }

        public async Task<Item> ImportFileAsync(StorageFile storageFile)
        {
            if (storageFile == null)
            {
                return null;
            }

            if (storageFile.FileType != ".yaml")
            {
                throw new UnsupportedFileException("Extension not supported", storageFile.FileType, storageFile.Name);
            }

            string content = await FileIO.ReadTextAsync(storageFile);
            OpenApiDocument document;

            try
            {
                var diagnostic = new OpenApiDiagnostic();
                document = new OpenApiStringReader().Read(content, out diagnostic);
            }
            catch
            {
                throw new UnsupportedFileException("Unable to parse yaml file", null, storageFile.Name);
            }

            if (document == null)
            {
                return null;
            }

            var dtoItem = _swaggerConverter.ConvertSwaggerDoc(document);
            var localCollection = JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(dtoItem));
            return localCollection;
        }
    }
}
