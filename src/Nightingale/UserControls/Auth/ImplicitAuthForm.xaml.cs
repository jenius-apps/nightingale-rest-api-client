using Nightingale.Utilities;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls.Auth
{
    public sealed partial class ImplicitAuthControl : ObservableUserControl
    {
        public ImplicitAuthControl()
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
            typeof(ImplicitAuthControl),
            null);

        public string OAuth2ClientId
        {
            get => (string)GetValue(OAuth2ClientIdProperty);
            set => SetValueDp(OAuth2ClientIdProperty, value);
        }

        public static readonly DependencyProperty OAuth2ClientIdProperty = DependencyProperty.Register(
            nameof(OAuth2ClientId),
            typeof(string),
            typeof(ImplicitAuthControl),
            null);

        public string OAuth2CallbackUrl
        {
            get => (string)GetValue(OAuth2CallbackUrlProperty);
            set => SetValueDp(OAuth2CallbackUrlProperty, value);
        }

        public static readonly DependencyProperty OAuth2CallbackUrlProperty = DependencyProperty.Register(
            nameof(OAuth2CallbackUrl),
            typeof(string),
            typeof(ImplicitAuthControl),
            null);

        public string OAuth2Scope
        {
            get => (string)GetValue(OAuth2ScopeProperty);
            set => SetValueDp(OAuth2ScopeProperty, value);
        }

        public static readonly DependencyProperty OAuth2ScopeProperty = DependencyProperty.Register(
            nameof(OAuth2Scope),
            typeof(string),
            typeof(ImplicitAuthControl),
            null);

        public string OAuth2State
        {
            get => (string)GetValue(OAuth2StateProperty);
            set => SetValueDp(OAuth2StateProperty, value);
        }

        public static readonly DependencyProperty OAuth2StateProperty = DependencyProperty.Register(
            nameof(OAuth2State),
            typeof(string),
            typeof(ImplicitAuthControl),
            null);
    }
}
