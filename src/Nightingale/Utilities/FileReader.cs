using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Nightingale.Utilities
{
    public class FileReader : IStorageFileReader
    {
        private readonly IFilePicker _filePicker;

        public FileReader(IFilePicker picker)
        {
            _filePicker = picker ?? throw new ArgumentNullException(nameof(picker));
        }

        /// <inheritdoc/>
        public async Task<byte[]> GetBytesAsync(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return null;
            }

            try
            {
                // We have to cache file to avoid access denied issues
                // in uwp apps.
                StorageFile cachedFile = await Common.CacheFileAsync(filepath);
                IBuffer buffer = await FileIO.ReadBufferAsync(cachedFile);
                return buffer.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> ReadFileAsync()
        {
            string pathOfFileToImport = await _filePicker.PickFileAsync();

            if (string.IsNullOrWhiteSpace(pathOfFileToImport))
            {
                return string.Empty;
            }

            StorageFile fileToImport = await StorageFile.GetFileFromPathAsync(pathOfFileToImport);

            if (fileToImport == null)
            {
                return string.Empty;
            }

            string result;

            try
            {
                result = await FileIO.ReadTextAsync(fileToImport);
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }
    }
}
