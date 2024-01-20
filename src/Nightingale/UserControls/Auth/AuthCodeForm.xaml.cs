using Nightingale.Utilities;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls.Auth
{
    public sealed partial class AuthCodeForm : ObservableUserControl
    {
        public AuthCodeForm()
        {
            this.InitializeComponent();
        }

        public string OAuth2AccessTokenUrl
        {
            get => (string)GetValue(OAuth2AccessTokenUrlProperty);
            set => SetValueDp(OAuth2AccessTokenUrlProperty, value);
        }

        public static readonly DependencyProperty OAuth2AccessTokenUrlProperty = DependencyProperty.Register(
            nameof(OAuth2AccessTokenUrl),
            typeof(string),
            typeof(AuthCodeForm),
            null);

        public string OAuth2RefreshToken
        {
            get => (string)GetValue(OAuth2RefreshTokenProperty);
            set => SetValueDp(OAuth2RefreshTokenProperty, value);
        }

        public static readonly DependencyProperty OAuth2RefreshTokenProperty = DependencyProperty.Register(
            nameof(OAuth2RefreshToken),
            typeof(string),
            typeof(AuthCodeForm),
            null);

        public string OAuth2ClientId
        {
            get => (string)GetValue(OAuth2ClientIdProperty);
            set => SetValueDp(OAuth2ClientIdProperty, value);
        }

        public static readonly DependencyProperty OAuth2ClientIdProperty = DependencyProperty.Register(
            nameof(OAuth2ClientId),
            typeof(string),
            typeof(AuthCodeForm),
            null);

        public string OAuth2ClientSecret
        {
            get => (string)GetValue(OAuth2ClientSecretProperty);
            set => SetValueDp(OAuth2ClientSecretProperty, value);
        }

        public static readonly DependencyProperty OAuth2ClientSecretProperty = DependencyProperty.Register(
            nameof(OAuth2ClientSecret),
            typeof(string),
            typeof(AuthCodeForm),
            null);

        public string OAuth2CallbackUrl
        {
            get => (string)GetValue(OAuth2CallbackUrlProperty);
            set => SetValueDp(OAuth2CallbackUrlProperty, value);
        }

        public static readonly DependencyProperty OAuth2CallbackUrlProperty = DependencyProperty.Register(
            nameof(OAuth2CallbackUrl),
            typeof(string),
            typeof(AuthCodeForm),
            null);

        public string OAuth2Scope
        {
            get => (string)GetValue(OAuth2ScopeProperty);
            set => SetValueDp(OAuth2ScopeProperty, value);
        }

        public static readonly DependencyProperty OAuth2ScopeProperty = DependencyProperty.Register(
            nameof(OAuth2Scope),
            typeof(string),
            typeof(AuthCodeForm),
            null);

        public string OAuth2AuthUrl
        {
            get => (string)GetValue(OAuth2AuthUrlProperty);
            set => SetValueDp(OAuth2AuthUrlProperty, value);
        }

        public static readonly DependencyProperty OAuth2AuthUrlProperty = DependencyProperty.Register(
            nameof(OAuth2AuthUrl),
            typeof(string),
            typeof(AuthCodeForm),
            null);
    }
}
