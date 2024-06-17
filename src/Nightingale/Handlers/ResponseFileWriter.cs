using JeniusApps.Common.Telemetry;
using Nightingale.Core.Interfaces;
using Nightingale.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Nightingale.Handlers;

public class ResponseFileWriter : IResponseFileWriter
{
    private readonly string _defaultFileName = "document";
    private readonly ITelemetry _telemetry;
    private readonly Dictionary<string, KeyValuePair<string, string>> _supportedFileTypes = new()
    {
        { "application/pdf", new KeyValuePair<string, string>("PDF", ".pdf") },
        { "application/json", new KeyValuePair<string, string>("JSON", ".json") },
        { "text/html", new KeyValuePair<string, string>("HTML", ".html") },
        { "text/plain", new KeyValuePair<string, string>("TXT", ".txt") },
        { "xml", new KeyValuePair<string, string>("XML", ".xml") },
    };

    public ResponseFileWriter(ITelemetry telemetry)
    {
        _telemetry = telemetry;
    }

    /// <inheritdoc/>
    public async Task<string> WriteImageAsync(byte[] image)
    {
        if (image == null)
        {
            return null;
        }

        // Ref: https://edi.wang/post/2015/12/22/windows-10-uwp-how-to-save-image
        var wbm = new WriteableBitmap(1, 1);
        using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
        {
            // Writes the image byte array in an InMemoryRandomAccessStream
            // that is needed to set the source of BitmapImage.
            using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
            {
                writer.WriteBytes(image);
                await writer.StoreAsync();
            }

            await wbm.SetSourceAsync(ms);
        }

        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.Downloads,
            SuggestedFileName = "image"
        };
        savePicker.FileTypeChoices.Add("PNG", new[] { ".png" });
        savePicker.FileTypeChoices.Add("JPEG", new[] { ".jpeg" });

        StorageFile file = await savePicker.PickSaveFileAsync();
        if (file == null)
        {
            return null;
        }

        using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
        {
            Guid encoderGuid = file.FileType == ".png" ? BitmapEncoder.PngEncoderId : BitmapEncoder.JpegEncoderId;
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderGuid, fileStream);
            Stream pixelStream = wbm.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Ignore,
                (uint)wbm.PixelWidth,
                (uint)wbm.PixelHeight,
                96.0,
                96.0,
                pixels);
            await encoder.FlushAsync();
        }

        return file.Path;
    }

    /// <inheritdoc/>
    public async Task WriteFileAsync(IWorkspaceResponse response)
    {
        if (response == null || response.ContentType == null || response.Body == null)
        {
            return;
        }

        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.Desktop
        };

        KeyValuePair<string, string> selectedType = new KeyValuePair<string, string>("TXT", ".txt");

        foreach (var pair in _supportedFileTypes)
        {
            if (response.ContentType.Contains(pair.Key))
            {
                selectedType = pair.Value;
                break;
            }
        }

        savePicker.FileTypeChoices.Add(selectedType.Key, new List<string>() { selectedType.Value });
        savePicker.SuggestedFileName = _defaultFileName;

        StorageFile file = await savePicker.PickSaveFileAsync();

        if (file == null)
        {
            return;
        }

        try
        {
            await FileIO.WriteTextAsync(file, response.Body);
            _telemetry.TrackEvent(Telemetry.SaveBodyClicked, new Dictionary<string, string>
            {
                { "Body type", selectedType.Key }
            });
        }
        catch (Exception e)
        {
            _telemetry.TrackEvent(Telemetry.SaveBodyError, new Dictionary<string, string>
            {
                { "Error message", e.Message }
            });
        }
    }
}
