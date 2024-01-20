namespace Nightingale.Core.Auth
{
    /// <summary>
    /// Constants used as keys
    /// for <see cref="Models.Authentication.AuthProperties"/>.
    /// </summary>
    public class AuthConstants
    {
        // Basic
        public const string BasicUsername = "BasicUsername";
        public const string BasicPassword = "BasicPassword";

        // OAuth1
        public const string OAuth1RequestTokenUrl = "OAuth1RequestTokenUrl";
        public const string OAuth1AccessTokenUrl = "OAuth1AuthorizeTokenUrl";
        public const string OAuth1AuthorizationUrl = "OAuth1AuthorizationUrl";
        public const string OAuth1ConsumerKey = "OAuth1ConsumerKey";
        public const string OAuth1ConsumerSecret = "OAuth1ConsumerSecret";
        public const string OAuth1CallbackUrl = "OAuth1CallbackUrl";
        public const string OAuth1TokenSecret = "OAuth1TokenSecret";
        public const string OAuth1AccessToken = "OAuth1AccessToken";
        public const string OAuth1Verifier = "OAuth1Verifier";

        // OAuth2
        public const string OAuth2AccessTokenUrl = "OAuth2AccessTokenUrl";
        public const string OAuth2AuthUrl = "OAuth2AuthUrl";
        public const string OAuth2ClientId = "OAuth2ClientId";
        public const string OAuth2ClientSecret = "OAuth2ClientSecret";
        public const string OAuth2Scope = "OAuth2Scope";
        public const string OAuth2AccessToken = "OAuth2AccessToken";
        public const string OAuth2CallbackUrl = "OAuth2CallbackUrl";
        public const string OAuth2GrantType = "OAuth2GrantType";
        public const string OAuth2State = "OAuth2State";
        public const string OAuth2RefreshToken = "OAuth2RefreshToken";

        // Bearer
        public const string BearerToken = "BearerToken";

        // Digest
        public const string DigestUsername = "DigestUsername";
        public const string DigestPassword = "DigestPassword";


        /// <summary>
        /// A list of supported auth properties.
        /// </summary>
        public static readonly string[] Props = new string[]
        {
            BasicUsername,
            BasicPassword,

            OAuth1RequestTokenUrl,
            OAuth1AccessTokenUrl,
            OAuth1AuthorizationUrl,
            OAuth1ConsumerKey,
            OAuth1ConsumerSecret,
            OAuth1CallbackUrl,
            OAuth1TokenSecret,
            OAuth1AccessToken,
            OAuth1Verifier,

            OAuth2AccessTokenUrl,
            OAuth2AuthUrl,
            OAuth2ClientId,
            OAuth2ClientSecret,
            OAuth2Scope,
            OAuth2AccessToken,
            OAuth2CallbackUrl,
            OAuth2GrantType,

            BearerToken,

            DigestUsername,
            DigestPassword
        };
    }
}
