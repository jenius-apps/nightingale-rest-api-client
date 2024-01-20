using Nightingale.CustomEventArgs;
using Nightingale.Utilities;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class EnvironmentsControl : ObservableUserControl
    {
        public event EventHandler ExitButtonClicked;
        public event EventHandler AddEnvironmentClicked;
        public event EventHandler<DeletedItemArgs<Core.Models.Parameter>> ParameterDeleted;
        public event EventHandler<DeletedItemArgs<object>> EnvironmentDeleted;
        public event EventHandler<AddedItemArgs<(string, string)>> EmojiChanged;
        public event EventHandler<AddedItemArgs<object>> EnvActivated;
        public event EventHandler<AddedItemArgs<object>> CloneEnvClicked;

        public EnvironmentsControl()
        {
            this.InitializeComponent();
        }

        public bool EnvQuickEditOn
        {
            get => (bool)GetValue(EnvQuickEditOnProperty);
            set => SetValueDp(EnvQuickEditOnProperty, value);
        }

        public string NewEnvName
        {
            get => (string)GetValue(NewEnvNameProperty);
            set => SetValueDp(NewEnvNameProperty, value);
        }

        public object EnvironmentList
        {
            get => GetValue(EnvironmentListProperty);
            set => SetValueDp(EnvironmentListProperty, value);
        }

        public object SelectedEnvironment
        {
            get => GetValue(SelectedEnvironmentProperty);
            set => SetValueDp(SelectedEnvironmentProperty, value);
        }

        public bool AddButtonEnabled
        {
            get => (bool)GetValue(AddButtonEnabledProperty);
            set => SetValueDp(AddButtonEnabledProperty, value);
        }

        public bool IsPremiumTipOpen
        {
            get => (bool)GetValue(IsPremiumTipOpenProperty);
            set => SetValueDp(IsPremiumTipOpenProperty, value);
        }

        public static readonly DependencyProperty EnvQuickEditOnProperty = DependencyProperty.Register(
            "EnvQuickEditOn",
            typeof(bool),
            typeof(SettingsControl),
            null);

        public static readonly DependencyProperty IsPremiumTipOpenProperty = DependencyProperty.Register(
            "IsPremiumTipOpen",
            typeof(bool),
            typeof(EnvironmentsControl),
            null);

        public static readonly DependencyProperty AddButtonEnabledProperty = DependencyProperty.Register(
            "AddButtonEnabled",
            typeof(bool),
            typeof(EnvironmentsControl),
            null);

        public static readonly DependencyProperty SelectedEnvironmentProperty = DependencyProperty.Register(
            "SelectedEnvironment",
            typeof(object),
            typeof(EnvironmentsControl),
            null);

        public static readonly DependencyProperty EnvironmentListProperty = DependencyProperty.Register(
            "EnvironmentList",
            typeof(object),
            typeof(EnvironmentsControl),
            null);

        public static readonly DependencyProperty NewEnvNameProperty = DependencyProperty.Register(
            "NewEnvName",
            typeof(string),
            typeof(EnvironmentsControl),
            null);

        private void ExitButton_Clicked(object sender, RoutedEventArgs e)
        {
            ExitButtonClicked?.Invoke(this, new EventArgs());
        }

        private void AddEnv_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AddEnvironment();
                e.Handled = true;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is ComboBoxItem item)
            {
                string content = item.Content?.ToString();
                string tag = item.Tag?.ToString();
                EmojiChanged?.Invoke(sender, new AddedItemArgs<(string, string)>((content, tag)));
            }
        }

        private void EnvDelete_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                var flyout = b.FindName("DeleteFlyout") as Flyout;
                flyout?.Hide();
            }

            if (((Button)sender).DataContext is object forDeletion)
            {
                EnvironmentDeleted?.Invoke(sender, new DeletedItemArgs<object>(forDeletion));
            }
        }

        private void AddEnvironment() => AddEnvironmentClicked?.Invoke(this, new EventArgs());

        private void ParameterListView_ParameterDeleted(object sender, DeletedItemArgs<Core.Models.Parameter> e)
        {
            ParameterDeleted?.Invoke(sender, e);
        }

        private void MakeActiveClicked(object sender, RoutedEventArgs e)
        {
            EnvActivated?.Invoke(this, new AddedItemArgs<object>(SelectedEnvironment));
        }

        private void CloneEnv(object sender, RoutedEventArgs e)
        {
            CloneEnvClicked?.Invoke(this, new AddedItemArgs<object>(SelectedEnvironment));
        }
    }
}
