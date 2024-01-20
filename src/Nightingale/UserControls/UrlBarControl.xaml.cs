using Nightingale.Core.Settings;
using Nightingale.CustomEventArgs;
using Nightingale.Handlers;
using Nightingale.Utilities;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class UrlBarControl : ObservableUserControl
    {
        public event EventHandler UrlTextBoxLostFocus;
        public event EventHandler UrlTextBoxEnterKeyPressed;
        public event EventHandler<AddedItemArgs<string>> UrlTextBoxSuggestionSelected;
        public event EventHandler SendButtonClicked;
        public event EventHandler SendAndDownloadButtonClicked;
        public event EventHandler<AddedItemArgs<string>> MethodChanged;
        public event EventHandler<AutoSuggestBoxTextChangedEventArgs> UrlTextBoxTextChanged;

        public UrlBarControl()
        {
            this.InitializeComponent();

            if (UserSettings.Get<bool>(SettingsConstants.AlwaysWrapURL))
            {
                VisualStateManager.GoToState(this, "Focused", false);
            }
        }

        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValueDp(ItemsSourceProperty, value);
        }

        public string SelectedMethod
        {
            get => (string)GetValue(SelectedMethodProperty);
            set => SetValueDp(SelectedMethodProperty, value);
        }

        public string BaseUrl
        {
            get => (string)GetValue(BaseUrlProperty);
            set => SetValueDp(BaseUrlProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(object),
            typeof(UrlBarControl),
            null);

        public static readonly DependencyProperty BaseUrlProperty = DependencyProperty.Register(
            "BaseUrl",
            typeof(string),
            typeof(UrlBarControl),
            null);

        public static readonly DependencyProperty SelectedMethodProperty = DependencyProperty.Register(
            "SelectedMethod",
            typeof(string),
            typeof(UrlBarControl),
            null);

        private void UrlLostFocus()
        {
            UrlTextBoxLostFocus?.Invoke(this, new EventArgs());

            // Only revert to normal if alwaysWrapUrl = true.
            if (!UserSettings.Get<bool>(SettingsConstants.AlwaysWrapURL))
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        private void UrlEnterKeyPressed() => UrlTextBoxEnterKeyPressed?.Invoke(this, new EventArgs());

        private void UrlSuggestionSelected(string s) => UrlTextBoxSuggestionSelected?.Invoke(this, new AddedItemArgs<string>(s));

        private void Send() => SendButtonClicked?.Invoke(this, new EventArgs());

        private void SendAndDownload() => SendAndDownloadButtonClicked?.Invoke(this, new EventArgs());

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion == null)
            {
                UrlEnterKeyPressed();
            }
            else
            {
                UrlSuggestionSelected(args.ChosenSuggestion as string);
            }
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            UrlTextBoxTextChanged?.Invoke(sender, args);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = e.AddedItems.FirstOrDefault() as string;
            MethodChanged?.Invoke(sender, new AddedItemArgs<string>(selection));
        }

        private void AutoSuggestUrlBox_GotFocus(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Focused", false);
        }
    }
}
