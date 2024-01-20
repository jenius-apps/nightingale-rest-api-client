using Nightingale.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace Nightingale.Handlers
{
    public class StorageFilePicker : IFilePicker
    {
        private readonly FileOpenPicker _picker;

        public StorageFilePicker()
        {
            _picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.Desktop
            };

            _picker.FileTypeFilter.Add("*");
        }

        public async Task<string> PickFileAsync()
        {
            StorageFile file = await _picker.PickSingleFileAsync();

            if (file != null)
            {
                StorageApplicationPermissions.FutureAccessList.Add(file);
                return file.Path;
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<IList<string>> PickFilesAsync()
        {
            var files = await _picker.PickMultipleFilesAsync();

            if (files == null || files.Count == 0)
            {
                return new List<string>();
            }

            foreach (var file in files)
            {
                StorageApplicationPermissions.FutureAccessList.Add(file);
            }

            return files.Select(x => x.Path).ToList();
        }
    }
}
