using MimeMapping;
using Nightingale.Core.Extensions;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Core.Client.Builders
{
    /// <summary>
    /// Class for configuring the body of
    /// a request message.
    /// </summary>
    public class BodyBuilder : IBodyBuilder
    {
        private readonly IVariableResolver _varRes;
        private readonly IStorageFileReader _fileReader;
        private readonly IFormDataBuilder _formDataBuilder;

        public BodyBuilder(
            IVariableResolver variableResolver,
            IStorageFileReader storageFileReader,
            IFormDataBuilder formDataBuilder)
        {
            _varRes = variableResolver
                ?? throw new ArgumentNullException(nameof(variableResolver));
            _fileReader = storageFileReader
                ?? throw new ArgumentNullException(nameof(storageFileReader));
            _formDataBuilder = formDataBuilder
                ?? throw new ArgumentNullException(nameof(formDataBuilder));
        }

        /// <inheritdoc/>
        public async Task SetBody(
            RequestBody body,
            HttpRequestMessage message,
            ILogger logger = null,
            string bodyContentType = null)
        {
            if (body == null
                || body == null
                || body.BodyType == RequestBodyType.None)
            {
                return;
            }

            // This makes it easier to use ?? operator
            bodyContentType = string.IsNullOrWhiteSpace(bodyContentType) ? null : bodyContentType;

            switch (body.BodyType)
            {
                case RequestBodyType.Text:
                    var textBody = _varRes.ResolveVariable(body.TextBody, useCache: true);
                    message.Content = new StringContent(textBody ?? "", Encoding.Default, bodyContentType ?? "text/plain");
                    logger?.Log("Body: " + System.Environment.NewLine + textBody);
                    break;
                case RequestBodyType.Json:
                    var jsonBody = _varRes.ResolveVariable(body.JsonBody, useCache: true);
                    message.Content = new StringContent(jsonBody ?? "", Encoding.Default, bodyContentType ?? "application/json");
                    logger?.Log("Body: " + System.Environment.NewLine + jsonBody);
                    break;
                case RequestBodyType.Xml:
                    var xmlBody = _varRes.ResolveVariable(body.XmlBody, useCache: true);
                    message.Content = new StringContent(xmlBody ?? "", Encoding.Default, bodyContentType ?? "application/xml");
                    logger?.Log("Body: " + System.Environment.NewLine + xmlBody);
                    break;
                case RequestBodyType.Binary:
                    byte[] filebytes = await _fileReader.GetBytesAsync(body.BinaryFilePath);
                    var content = new ByteArrayContent(filebytes);
                    content.Headers.ContentType = new MediaTypeHeaderValue(bodyContentType ?? MimeUtility.GetMimeMapping(body.BinaryFilePath));
                    message.Content = content;
                    logger?.Log("Body: File - " + body.BinaryFilePath);
                    break;
                case RequestBodyType.FormEncoded:
                    var formEncodedContent = new Dictionary<string, string>();
                    logger?.Log("Body: x-www-form-urlencoded");
                    foreach (var item in body.FormEncodedData.GetActive())
                    {
                        var key = _varRes.ResolveVariable(item.Key, useCache: true);
                        var value = _varRes.ResolveVariable(item.Value, useCache: true);
                        formEncodedContent.Add(key, value);
                        logger?.Log(key + ": " + value);
                    }
                    message.Content = new FormUrlEncodedContent(formEncodedContent);
                    break;
                case RequestBodyType.FormData:
                    logger?.Log("Body: multipart/form-data");
                    await _formDataBuilder.AddFormDataAsync(message, body, _varRes, logger);
                    break;
                default:
                    throw new NotImplementedException("Unknown body type: " + body.BodyType.ToString());
            }
        }
    }
}
