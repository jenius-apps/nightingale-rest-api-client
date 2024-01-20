using Microsoft.AppCenter.Analytics;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Nightingale.Utilities
{
    public class Common
    {
        public static void Copy(string content)
        {
            var dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy,
            };
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);
        }

        /// <summary>
        /// Adds the image from the byte array to clipboard.
        /// </summary>
        /// <param name="image">An image in byte[] form.</param>
        public static async Task CopyImage(byte[] image)
        {
            if (image == null)
            {
                return;
            }

            var dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };

            try
            {
                using (var ms = new InMemoryRandomAccessStream())
                using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(image);
                    await writer.StoreAsync();

                    // Ref: https://docs.microsoft.com/en-us/archive/blogs/going_metro/working-with-streams-creating-randomaccessstreamreference-from-image-downloaded-from-web
                    var streamReference = RandomAccessStreamReference.CreateFromStream(ms);
                    dataPackage.SetBitmap(streamReference);
                    Clipboard.SetContent(dataPackage);

                    // Ref: https://stackoverflow.com/a/31832826/10953422
                    Clipboard.Flush();
                }
            }
            catch
            {
                Analytics.TrackEvent("Copy image failed");
            }
        }

        /// <summary>
        /// Saves give file 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public static async Task<StorageFile> CacheFileAsync(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
                return null;

            StorageFile copy = null;

            try
            {
                StorageFile original = await StorageFile.GetFileFromPathAsync(sourcePath);
                copy = await original.CopyAsync(ApplicationData.Current.LocalCacheFolder, original.Name, NameCollisionOption.ReplaceExisting);
            }
            catch (Exception e)
            {
                var msg = new MessageDialog(e.Message, "Unable to open file");
                await msg.ShowAsync();
            }

            return copy;
        }
    }
}
