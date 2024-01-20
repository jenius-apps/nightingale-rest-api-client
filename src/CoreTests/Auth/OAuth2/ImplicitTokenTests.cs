using Xunit;
using Moq;
using System.Threading.Tasks;
using Nightingale.Core.Auth;

namespace CoreTests.Auth.OAuth2
{
    public class ImplicitTokenTests
    {
        /// <summary>
        /// Confirms that URL is built correctly
        /// and the access token can be retrieved correctly.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetTokenTest()
        {
            var webBrokerMock = new Mock<IWebAuthBroker>();
            webBrokerMock.Setup(x => 
                x.GetAuthorizationResultAsync(
                    "https://www.example.com?response_type=token&client_id=lk1j23lkj&redirect_uri=https%3A%2F%2Fwww.jeniusapps.com",
                    It.IsAny<string>(),
                    It.IsAny<object>()))
                .ReturnsAsync("https://www.jeniusapps.com/#access_token=123123123&state=xyz");

            var retriever = new OAuth2ImplicitTokenRetriever(webBrokerMock.Object);

            var token = await retriever.GetImplicitAccessToken(
                "https://www.example.com",
                "lk1j23lkj",
                "https://www.jeniusapps.com",
                null,
                null);

            Assert.Equal("123123123", token);
        }
    }
}
