using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Threading.Tasks;
using Nightingale.Core.Auth;
using Nightingale.Core.Http;
using System.Net.Http;
using CoreTests.Helpers;
using System.Net;
using Newtonsoft.Json;

namespace CoreTests.Auth.OAuth2
{
    public class TokenRetrieverTests
    {
        /// <summary>
        /// Tests that an access token can be returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RefreshTokenTest()
        {
            var returnToken = new Token
            {
                AccessToken = "1234",
                RefreshToken = "6789"
            };
            var content = new StringContent(JsonConvert.SerializeObject(returnToken));

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(
                x => x.GetClient())
                .Returns(new HttpClient(new MockHandler(HttpStatusCode.OK, content)));

            var retriever = new OAuth2TokenRetriever(factoryMock.Object);
            Token refreshToken = await retriever.RefreshAccessToken("https://www.example.com", "foobar");
            Assert.Equal(returnToken.AccessToken, refreshToken.AccessToken);
        }
    }
}
