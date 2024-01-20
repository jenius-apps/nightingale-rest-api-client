using Nightingale.Core.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Nightingale.Utilities
{
    public class FileWriter : IStorageFileWriter
    {
        /// <inheritdoc/>
        public async Task<string> WriteTextAsync(string content, string filename, string fileDescriptor, string fileType)
        {
            StorageFile targetLocation = await GetFileTargetLocation(filename, fileDescriptor, fileType);

            if (targetLocation != null)
            {
                await FileIO.WriteTextAsync(targetLocation, content);
                return targetLocation.Path;
            }

            return string.Empty;
        }

        public static async Task<StorageFile> GetFileTargetLocation(string filename, string fileDescriptor, string fileType)
        {
            var filePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                SuggestedFileName = filename
            };
            filePicker.FileTypeChoices.Add(fileDescriptor, new List<string>() { fileType });

            return await filePicker.PickSaveFileAsync();
        }
    }
}
