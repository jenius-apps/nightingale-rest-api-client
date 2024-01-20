using Nightingale.Core.Client.Builders;
using Nightingale.Core.Models;
using System.Net.Http;
using Xunit;
using Moq;
using Nightingale.Core.Helpers.Interfaces;
using System.Threading.Tasks;
using System;

namespace CoreTests.Client
{
    public class FormDataBuilderTests
    {
        private Mock<IStorageFileReader> GetMockFilReader()
        {
            var fileReaderMock = new Mock<IStorageFileReader>();
            fileReaderMock.Setup(
                x => x.GetBytesAsync(It.IsAny<string>()))
                .ReturnsAsync(new byte[1]);
            return fileReaderMock;
        }

        /// <summary>
        /// Ensures that a form data list
        /// where all items have invalid
        /// keys will result in an exception.
        /// </summary>
        [Fact]
        public async Task FormDataInvalidKeysException()
        {
            var message = new HttpRequestMessage();
            var body = new RequestBody
            {
                BodyType = RequestBodyType.FormData
            };
            body.FormDataList.Add(new FormData
            {
                Enabled = true,
                ContentType = null,
                Key = "",
                FormDataType = FormDataType.File,
                FilePaths = new string[] { "test.png" },
                AutoContentType = "image/png"
            });
            body.FormDataList.Add(new FormData
            {
                Enabled = true,
                ContentType = null,
                Key = null,
                FormDataType = FormDataType.File,
                FilePaths = new string[] { "test.png" },
                AutoContentType = "image/png"
            });
            body.FormDataList.Add(new FormData
            {
                Enabled = true,
                ContentType = null,
                Key = "  ",
                FormDataType = FormDataType.File,
                FilePaths = new string[] { "test.png" },
                AutoContentType = "image/png"
            });
            
            var fileReaderMock = GetMockFilReader();
            var builder = new FormDataBuilder(fileReaderMock.Object);
            await Assert.ThrowsAsync<Exception>(() => builder.AddFormDataAsync(message, body, null, null));
        }

        /// <summary>
        /// Ensures that when form data content type is null or whitespace,
        /// an auto-detected content type is used.
        /// </summary>
        [Fact]
        public async Task AutoDetectContentType()
        {
            var message = new HttpRequestMessage();
            var body = new RequestBody
            {
                BodyType = RequestBodyType.FormData
            };
            body.FormDataList.Add(new FormData
            {
                Enabled = true,
                ContentType = null,
                Key = "asdf",
                FormDataType = FormDataType.File,
                FilePaths = new string[] { "test.png" },
                AutoContentType = "image/png"
            });
            body.FormDataList.Add(new FormData
            {
                Enabled = true,
                ContentType = "",
                Key = "asdf",
                FormDataType = FormDataType.File,
                FilePaths = new string[] { "test.jpg" },
                AutoContentType = "image/jpeg"
            });

            // set up file reader mock.
            var fileReaderMock = GetMockFilReader();

            var builder = new FormDataBuilder(fileReaderMock.Object);
            await builder.AddFormDataAsync(message, body, null, null);
            var result = await message.Content.ReadAsStringAsync();
            Assert.Contains("Content-Type: image/png", result);
            Assert.Contains("Content-Type: image/jpeg", result);
        }

        /// <summary>
        /// Ensures that user content type is respected
        /// in form data.
        /// </summary>
        [Fact]
        public async Task UserContentTypeUsed()
        {
            var message = new HttpRequestMessage();
            var body = new RequestBody
            {
                BodyType = RequestBodyType.FormData
            };
            body.FormDataList.Add(new FormData
            {
                Enabled = true,
                ContentType = "text/plain",
                AutoContentType = "asdfasfd",
                Key = "myimage",
                FormDataType = FormDataType.Text,
                Value = "test"
            });
            body.FormDataList.Add(new FormData
            {
                Enabled = true,
                ContentType = "image/png",
                AutoContentType = "asdfasfd",
                Key = "asdf",
                FormDataType = FormDataType.File,
                FilePaths = new string[] { "test.png" }
            });

            // set up file reader mock.
            var fileReaderMock = GetMockFilReader();

            var builder = new FormDataBuilder(fileReaderMock.Object);
            await builder.AddFormDataAsync(message, body, null, null);

            Assert.True(message.Content is MultipartFormDataContent);
            var result = await message.Content.ReadAsStringAsync();
            // Should look something like this:
            // --61dbca5c-c868-49ce-8304-0bbef09d1770
            // Content-Type: text/plain; charset=utf-8
            // Content-Disposition: form-data; name=myimage
            // test
            // --61dbca5c-c868-49ce-8304-0bbef09d1770
            // Content-Type: image/png
            // Content-Disposition: form-data; name=asdf; filename=test.png; filename*=utf-8''test.png
            Assert.Contains("Content-Type: text/plain", result);
            Assert.Contains("Content-Type: image/png", result);
        }
    }
}
