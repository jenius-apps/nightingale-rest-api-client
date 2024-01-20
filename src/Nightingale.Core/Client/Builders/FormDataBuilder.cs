using Nightingale.Core.Helpers;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Core.Client.Builders
{
    /// <summary>
    /// Class for adding form data
    /// into the http request message.
    /// </summary>
    public class FormDataBuilder : IFormDataBuilder
    {
        private readonly IStorageFileReader _fileReader;

        public FormDataBuilder(
            IStorageFileReader storageFileReader)
        {
            _fileReader = storageFileReader
                ?? throw new ArgumentNullException(nameof(storageFileReader));
        }

        /// <inheritdoc/>
        public async Task AddFormDataAsync(
            HttpRequestMessage message,
            RequestBody body,
            IVariableResolver resolver,
            ILogger logger = null)
        {
            if (message == null || body == null)
            {
                return;
            }

            var formDataContent = new MultipartFormDataContent();
            var items = body.FormDataList.Where(x => x.Enabled && !string.IsNullOrWhiteSpace(x.Key)).ToArray();
            if (items == null || items.Length == 0)
            {
                throw new Exception("No valid form data items to add. Add an item " +
                    "or make sure all items have keys and they are enabled.");
            }

            foreach (var item in items)
            {
                // Ensure the user-supplied content type is used.
                string contentType = resolver?.ResolveVariable(item.ContentType, useCache: true) ?? item.ContentType;
                string key = resolver?.ResolveVariable(item.Key, useCache: true) ?? item.Key;

                if (item.FormDataType == FormDataType.Text)
                {
                    string value = resolver?.ResolveVariable(item.Value, useCache: true) ?? item.Value;
                    logger?.Log(key + ": " + value);


                    StringContent content = string.IsNullOrWhiteSpace(contentType)
                        ? new StringContent(value) 
                        : new StringContent(value, Encoding.Default, contentType);
                    formDataContent.Add(content, key);
                }
                else if (item.FormDataType == FormDataType.File)
                {
                    foreach (var filepath in item.FilePaths)
                    {
                        // Auto detect content type if not specified by user.
                        var fileContentType = !string.IsNullOrWhiteSpace(contentType)
                            ? contentType
                            : item.AutoContentType;

                        byte[] bytes = await _fileReader.GetBytesAsync(filepath);
                        var content = new ByteArrayContent(bytes);
                        content.Headers.ContentType = new MediaTypeHeaderValue(fileContentType);
                        formDataContent.Add(content, key, filepath);
                    }

                    logger?.Log(key + ": " + string.Join(", ", item.FilePaths));
                }
            }

            message.Content = formDataContent;
        }
    }
}
