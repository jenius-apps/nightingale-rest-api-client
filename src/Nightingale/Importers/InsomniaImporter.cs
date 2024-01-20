using Insomnia.NET.Models;
using JeniusApps.Nightingale.Converters.Insomnia;
using Newtonsoft.Json;
using Nightingale.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Importers
{
    public class InsomniaImporter : IInsomniaImporter
    {
        private readonly IInsomniaConverter _insomniaConverter;

        public InsomniaImporter(IInsomniaConverter insomniaConverter)
        {
            _insomniaConverter = insomniaConverter
                ?? throw new ArgumentNullException(nameof(insomniaConverter));
        }

        public async Task<IList<Core.Models.Workspace>> ImportFileAsync(StorageFile storageFile)
        {
            if (storageFile == null)
            {
                return null;
            }

            if (storageFile.ContentType != "application/json")
            {
                throw new UnsupportedFileException("Unsupported file type", storageFile.ContentType, storageFile.Name);
            }

            string content = await FileIO.ReadTextAsync(storageFile);
            ExportDoc insomniaExportFile = JsonConvert.DeserializeObject<ExportDoc>(content);

            var dtoWorkspace = _insomniaConverter.Convert(insomniaExportFile);
            var localWorkspace = JsonConvert.DeserializeObject<Core.Models.Workspace[]>(JsonConvert.SerializeObject(dtoWorkspace));
            return localWorkspace;
        }
    }
}
