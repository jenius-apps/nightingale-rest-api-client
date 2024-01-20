using Nightingale.Core.Auth;
using Nightingale.Core.Models;
using System.Collections.Generic;
using Xunit;

namespace CoreTests
{
    public class AuthExtensionTests
    {
        /// <summary>
        /// Ensures that the GetEnumProp method
        /// correctly parses the auth properties dictionary.
        /// </summary>
        [Fact]
        public void EnumTest()
        {
            var auth = new Authentication()
            {
                AuthProperties = new Dictionary<string, string>()
            };

            auth.SetProp(AuthConstants.OAuth2GrantType, GrantType.authorization_code.ToString());
            GrantType result = auth.GetEnumProp<GrantType>(AuthConstants.OAuth2GrantType);
            Assert.Equal(GrantType.authorization_code, result);
        }

        /// <summary>
        /// Ensures the GetEnumProp method
        /// correctly handles an empty dictionary.
        /// </summary>
        [Fact]
        public void EnumEmptyTest()
        {
            var auth = new Authentication()
            {
                AuthProperties = new Dictionary<string, string>()
            };

            GrantType result = auth.GetEnumProp<GrantType>(AuthConstants.OAuth2GrantType);
            Assert.Equal(GrantType.client_credentials, result);
        }

        /// <summary>
        /// Ensures that the GetProp method
        /// correctly deletes a property if it's null
        /// or empty, but allows for whitespace.
        /// </summary>
        [Fact]
        public void SetEmptyPropTest()
        {
            var auth = new Authentication();
            auth.SetProp(AuthConstants.BearerToken, "test");
            Assert.True(auth.AuthProperties.ContainsValue("test"));

            auth.SetProp(AuthConstants.BearerToken, "");
            Assert.Empty(auth.AuthProperties);

            auth.SetProp(AuthConstants.BearerToken, "test");
            Assert.True(auth.AuthProperties.ContainsValue("test"));

            auth.SetProp(AuthConstants.BearerToken, null);
            Assert.Empty(auth.AuthProperties);

            auth.SetProp(AuthConstants.BearerToken, "   ");
            Assert.True(auth.AuthProperties.ContainsValue("   "));
        }
    }
}
