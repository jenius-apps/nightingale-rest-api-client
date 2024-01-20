using Xunit;
using Moq;
using Nightingale.Core.Client.Builders;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreTests.Client
{
    public class BodyBuilderTests
    {
        private BodyBuilder GetBodyBuilder()
        {
            var resolverMock = new Mock<IVariableResolver>();
            var formDataBuilderMock = new Mock<IFormDataBuilder>();
            var fileReaderMock = new Mock<IStorageFileReader>();

            fileReaderMock.Setup(
                x => x.GetBytesAsync(It.IsAny<string>()))
                .ReturnsAsync(new byte[1]);

            var builder = new BodyBuilder(
                resolverMock.Object,
                fileReaderMock.Object,
                formDataBuilderMock.Object);

            return builder;
        }

        [Theory]
        [InlineData("test.json", "application/json")]
        [InlineData("test.png", "image/png")]
        [InlineData("test.ncf", "application/octet-stream")]
        public async Task BinaryFileAutoContentTypeTest(string filepath, string expectedContentType)
        {
            var builder = GetBodyBuilder();
            var message = new HttpRequestMessage();
            var body = new RequestBody
            {
                BodyType = RequestBodyType.Binary,
                BinaryFilePath = filepath
            };

            await builder.SetBody(body, message, null, null);
            Assert.Equal(expectedContentType, message.Content.Headers.ContentType.MediaType);

            await builder.SetBody(body, message, null, "");
            Assert.Equal(expectedContentType, message.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task BinaryFileUserContentTypeTest()
        {
            var builder = GetBodyBuilder();
            var message = new HttpRequestMessage();
            var body = new RequestBody
            {
                BodyType = RequestBodyType.Binary,
                BinaryFilePath = "test.ncf"
            };

            await builder.SetBody(body, message, null, "application/json");
            Assert.Equal("application/json", message.Content.Headers.ContentType.MediaType);
        }
    }
}
