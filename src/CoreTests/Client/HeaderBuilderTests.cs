using Xunit;
using Moq;
using System.Collections.Generic;
using Nightingale.Core.Client.Builders;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Models;
using System.Net.Http;
using Nightingale.Core.Workspaces.Models;

namespace CoreTests.Client
{
    public class HeaderBuilderTests
    {
        private HeaderBuilder GetHeaderBuilder()
        {
            var resolverMock = new Mock<IVariableResolver>();

            // Return the same input.
            resolverMock.Setup(
                x => x.ResolveVariable(It.IsAny<string>(), It.IsAny<IList<Parameter>>(), It.IsAny<bool>()))
                .Returns((string s, IList<Parameter> p, bool b) => s);

            var builder = new HeaderBuilder(
                resolverMock.Object);

            return builder;
        }

        [Theory]
        [InlineData("Authorization", "Bearer StandardAuth")]
        [InlineData("Authorization", "NonStandardAuth=foobar")]
        public void AuthHeader_ShouldAllowNonStandard(string key, string value)
        {
            var builder = GetHeaderBuilder();
            var msg = new HttpRequestMessage();
            var item = new Item();
            item.Headers.Add(new Parameter
            {
                Enabled = true,
                Key = key,
                Value = value
            });

            builder.SetHeaders(item, msg);

            Assert.True(msg.Headers.Contains("Authorization"));
        }
    }
}
