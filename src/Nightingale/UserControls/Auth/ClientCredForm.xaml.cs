using Nightingale.Utilities;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls.Auth
{
    public sealed partial class ClientCredForm : ObservableUserControl
    {
        public ClientCredForm()
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
            typeof(ClientCredForm),
            null);

        public string OAuth2ClientId
        {
            get => (string)GetValue(OAuth2ClientIdProperty);
            set => SetValueDp(OAuth2ClientIdProperty, value);
        }

        public static readonly DependencyProperty OAuth2ClientIdProperty = DependencyProperty.Register(
            nameof(OAuth2ClientId),
            typeof(string),
            typeof(ClientCredForm),
            null);

        public string OAuth2ClientSecret
        {
            get => (string)GetValue(OAuth2ClientSecretProperty);
            set => SetValueDp(OAuth2ClientSecretProperty, value);
        }

        public static readonly DependencyProperty OAuth2ClientSecretProperty = DependencyProperty.Register(
            nameof(OAuth2ClientSecret),
            typeof(string),
            typeof(ClientCredForm),
            null);

        public string OAuth2Scope
        {
            get => (string)GetValue(OAuth2ScopeProperty);
            set => SetValueDp(OAuth2ScopeProperty, value);
        }

        public static readonly DependencyProperty OAuth2ScopeProperty = DependencyProperty.Register(
            nameof(OAuth2Scope),
            typeof(string),
            typeof(ClientCredForm),
            null);
    }
}
