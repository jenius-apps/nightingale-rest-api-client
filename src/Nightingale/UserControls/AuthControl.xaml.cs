using Nightingale.Utilities;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class AuthControl : ObservableUserControl
    {
        public event EventHandler FetchTokenClicked;
        public event EventHandler RefreshTokenClicked;
        public event EventHandler FetchOauth1TokenClicked;

        public AuthControl()
        {
            this.InitializeComponent();
        }

        public bool IsNoAuth
        {
            get => (bool)GetValue(IsNoAuthProperty);
            set => SetValueDp(IsNoAuthProperty, value);
        }

        public bool IsBasicAuth
        {
            get => (bool)GetValue(IsBasicAuthProperty);
            set => SetValueDp(IsBasicAuthProperty, value);
        }

        public bool IsOauth1
        {
            get => (bool)GetValue(IsOauth1Property);
            set => SetValueDp(IsOauth1Property, value);
        }

        public bool IsOauth2
        {
            get => (bool)GetValue(IsOauth2Property);
            set => SetValueDp(IsOauth2Property, value);
        }

        public bool IsBearerAuth
        {
            get => (bool)GetValue(IsBearerAuthProperty);
            set => SetValueDp(IsBearerAuthProperty, value);
        }

        public string BasicUsername
        {
            get => (string)GetValue(BasicUsernameProperty);
            set => SetValueDp(BasicUsernameProperty, value);
        }

        public string BasicPassword
        {
            get => (string)GetValue(BasicPasswordProperty);
            set => SetValueDp(BasicPasswordProperty, value);
        }

        public string OAuth1ConsumerKey
        {
            get => (string)GetValue(OAuth1ConsumerKeyProperty);
            set => SetValueDp(OAuth1ConsumerKeyProperty, value);
        }

        public string OAuth1ConsumerSecret
        {
            get => (string)GetValue(OAuth1ConsumerSecretProperty);
            set => SetValueDp(OAuth1ConsumerSecretProperty, value);
        }

        public string OAuth1CallbackUrl
        {
            get => (string)GetValue(OAuth1CallbackUrlProperty);
            set => SetValueDp(OAuth1CallbackUrlProperty, value);
        }

        public string OAuth1RequestTokenUrl
        {
            get => (string)GetValue(OAuth1RequestTokenUrlProperty);
            set => SetValueDp(OAuth1RequestTokenUrlProperty, value);
        }

        public string OAuth1AccessTokenUrl
        {
            get => (string)GetValue(OAuth1AccessTokenUrlProperty);
            set => SetValueDp(OAuth1AccessTokenUrlProperty, value);
        }

        public string OAuth1AuthorizationUrl
        {
            get => (string)GetValue(OAuth1AuthorizationUrlProperty);
            set => SetValueDp(OAuth1AuthorizationUrlProperty, value);
        }

        public string OAuth1AccessToken
        {
            get => (string)GetValue(OAuth1AccessTokenProperty);
            set => SetValueDp(OAuth1AccessTokenProperty, value);
        }

        public string OAuth1TokenSecret
        {
            get => (string)GetValue(OAuth1TokenSecretProperty);
            set => SetValueDp(OAuth1TokenSecretProperty, value);
        }

        public int OAuth2GrantTypeIndex
        {
            get => (int)GetValue(OAuth2GrantTypeIndexProperty);
            set => SetValueDp(OAuth2GrantTypeIndexProperty, value);
        }

        public bool IsAuthorizationCode
        {
            get => (bool)GetValue(IsAuthorizationCodeProperty);
            set => SetValueDp(IsAuthorizationCodeProperty, value);
        }

        public string OAuth2AccessTokenUrl
        {
            get => (string)GetValue(OAuth2AccessTokenUrlProperty);
            set => SetValueDp(OAuth2AccessTokenUrlProperty, value);
        }

        public string OAuth2ClientId
        {
            get => (string)GetValue(OAuth2ClientIdProperty);
            set => SetValueDp(OAuth2ClientIdProperty, value);
        }

        public string OAuth2ClientSecret
        {
            get => (string)GetValue(OAuth2ClientSecretProperty);
            set => SetValueDp(OAuth2ClientSecretProperty, value);
        }

        public string OAuth2Scope
        {
            get => (string)GetValue(OAuth2ScopeProperty);
            set => SetValueDp(OAuth2ScopeProperty, value);
        }

        public string OAuth2CallbackUrl
        {
            get => (string)GetValue(OAuth2CallbackUrlProperty);
            set => SetValueDp(OAuth2CallbackUrlProperty, value);
        }

        public string OAuth2AuthUrl
        {
            get => (string)GetValue(OAuth2AuthUrlProperty);
            set => SetValueDp(OAuth2AuthUrlProperty, value);
        }

        public string OAuth2AccessToken
        {
            get => (string)GetValue(OAuth2AccessTokenProperty);
            set => SetValueDp(OAuth2AccessTokenProperty, value);
        }

        public string Log
        {
            get => (string)GetValue(LogProperty);
            set => SetValueDp(LogProperty, value);
        }

        public string BearerToken
        {
            get => (string)GetValue(BearerTokenProperty);
            set => SetValueDp(BearerTokenProperty, value);
        }

        public int AuthTypeIndex
        {
            get => (int)GetValue(AuthTypeIndexProperty);
            set => SetValueDp(AuthTypeIndexProperty, value);
        }

        public bool IsDigestAuth
        {
            get => (bool)GetValue(IsDigestAuthProperty);
            set => SetValueDp(IsDigestAuthProperty, value);
        }

        public bool IsParentAuth
        {
            get => (bool)GetValue(IsParentAuthProperty);
            set => SetValueDp(IsParentAuthProperty, value);
        }

        public string DigestUsername
        {
            get => (string)GetValue(DigestUsernameProperty);
            set => SetValueDp(DigestUsernameProperty, value);
        }

        public string DigestPassword
        {
            get => (string)GetValue(DigestPasswordProperty);
            set => SetValueDp(DigestPasswordProperty, value);
        }

        public static readonly DependencyProperty DigestPasswordProperty = DependencyProperty.Register(
            "DigestPassword",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty DigestUsernameProperty = DependencyProperty.Register(
            "DigestUsername",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty IsParentAuthProperty = DependencyProperty.Register(
            "IsParentAuth",
            typeof(bool),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty IsDigestAuthProperty = DependencyProperty.Register(
            "IsDigestAuth",
            typeof(bool),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty AuthTypeIndexProperty = DependencyProperty.Register(
            "AuthTypeIndex",
            typeof(int),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty BearerTokenProperty = DependencyProperty.Register(
            "BearerToken",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty LogProperty = DependencyProperty.Register(
            "Log",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth2AccessTokenProperty = DependencyProperty.Register(
            "OAuth2AccessToken",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth2AuthUrlProperty = DependencyProperty.Register(
            "OAuth2AuthUrl",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth2CallbackUrlProperty = DependencyProperty.Register(
            "OAuth2CallbackUrl",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth1RequestTokenUrlProperty = DependencyProperty.Register(
            "OAuth1RequestTokenUrl",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth1AccessTokenUrlProperty = DependencyProperty.Register(
            "OAuth1AccessTokenUrl",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth1AuthorizationUrlProperty = DependencyProperty.Register(
            "OAuth1AuthorizationUrl",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth2ClientSecretProperty = DependencyProperty.Register(
            "OAuth2ClientSecret",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth2ClientIdProperty = DependencyProperty.Register(
            "OAuth2ClientId",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth2AccessTokenUrlProperty = DependencyProperty.Register(
            "OAuth2AccessTokenUrl",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty IsAuthorizationCodeProperty = DependencyProperty.Register(
            "IsAuthorizationCode",
            typeof(bool),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth2GrantTypeIndexProperty = DependencyProperty.Register(
            "OAuth2GrantTypeIndex",
            typeof(int),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth1CallbackUrlProperty = DependencyProperty.Register(
            "OAuth1CallbackUrl",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth2ScopeProperty = DependencyProperty.Register(
            "OAuth2Scope",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth1TokenSecretProperty = DependencyProperty.Register(
            "OAuth1TokenSecret",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth1AccessTokenProperty = DependencyProperty.Register(
            "OAuth1AccessToken",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth1ConsumerSecretProperty = DependencyProperty.Register(
            "OAuth1ConsumerSecret",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty OAuth1ConsumerKeyProperty = DependencyProperty.Register(
            "OAuth1ConsumerKey",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty BasicPasswordProperty = DependencyProperty.Register(
            "BasicPassword",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty BasicUsernameProperty = DependencyProperty.Register(
            "BasicUsername",
            typeof(string),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty IsNoAuthProperty = DependencyProperty.Register(
            "IsNoAuth",
            typeof(bool),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty IsBasicAuthProperty = DependencyProperty.Register(
            "IsBasicAuth",
            typeof(bool),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty IsOauth1Property = DependencyProperty.Register(
            "IsOauth1",
            typeof(bool),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty IsOauth2Property = DependencyProperty.Register(
            "IsOauth2",
            typeof(bool),
            typeof(AuthControl),
            null);

        public static readonly DependencyProperty IsBearerAuthProperty = DependencyProperty.Register(
            "IsBearerAuth",
            typeof(bool),
            typeof(AuthControl),
            null);

        public bool IsClientCred
        {
            get => (bool)GetValue(IsClientCredProperty);
            set => SetValueDp(IsClientCredProperty, value);
        }

        public static readonly DependencyProperty IsClientCredProperty = DependencyProperty.Register(
            nameof(IsClientCred),
            typeof(bool),
            typeof(AuthControl),
            null);

        public bool IsImplicit
        {
            get => (bool)GetValue(IsImplicitProperty);
            set => SetValueDp(IsImplicitProperty, value);
        }

        public static readonly DependencyProperty IsImplicitProperty = DependencyProperty.Register(
            nameof(IsImplicit),
            typeof(bool),
            typeof(AuthControl),
            null);

        public bool IsAuthCode
        {
            get => (bool)GetValue(IsAuthCodeProperty);
            set => SetValueDp(IsAuthCodeProperty, value);
        }

        public static readonly DependencyProperty IsAuthCodeProperty = DependencyProperty.Register(
            nameof(IsAuthCode),
            typeof(bool),
            typeof(AuthControl),
            null);

        public string OAuth2State
        {
            get => (string)GetValue(OAuth2StateProperty);
            set => SetValueDp(OAuth2StateProperty, value);
        }

        public static readonly DependencyProperty OAuth2StateProperty = DependencyProperty.Register(
            nameof(OAuth2State),
            typeof(string),
            typeof(AuthControl),
            null);

        public string OAuth2RefreshToken
        {
            get => (string)GetValue(OAuth2RefreshTokenProperty);
            set => SetValueDp(OAuth2RefreshTokenProperty, value);
        }

        public static readonly DependencyProperty OAuth2RefreshTokenProperty = DependencyProperty.Register(
            nameof(OAuth2RefreshToken),
            typeof(string),
            typeof(AuthControl),
            null);

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                AuthTypeIndex = rb.GetIndexWithinGroup();
            }
        }

        private void FetchToken() => FetchTokenClicked?.Invoke(this, new EventArgs());

        private void RefreshToken() => RefreshTokenClicked?.Invoke(this, new EventArgs());

        private void FetchOauth1Token() => FetchOauth1TokenClicked?.Invoke(this, new EventArgs());

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Using selection changed + oneway index instead of TwoWay index
            // because the tab switching functionality does not play well with TwoWay binding.
            // With TwoWay, the index is forced to change to whatever was used by the
            // previous tab. So we use oneWay to avoid this.

            if (e.AddedItems.FirstOrDefault() is ComboBoxItem c)
            {
                int index;
                switch ((string)c.Tag)
                {
                    case "clientcred":
                        index = 0;
                        break;
                    case "authcode":
                        index = 1;
                        break;
                    case "implicitflow":
                        index = 2;
                        break;
                    default:
                        index = 0;
                        break;
                }

                OAuth2GrantTypeIndex = index;
            }
        }
    }
}
