using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Nightingale.Core.Models;
using Nightingale.Utilities;
using Nightingale.CustomEventArgs;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Windows.UI.Xaml.Controls.Primitives;
using Nightingale.Core.Helpers.Interfaces;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class ParameterListView : ObservableUserControl
    {
        public event EventHandler ValuesUpdated;
        public event EventHandler<DeletedItemArgs<Parameter>> ParameterDeleted;
        public event EventHandler<AddedItemArgs<Parameter>> AddVariableClicked;
        private bool _creatingNew;
        private TextBox _ctrlSpaceContext;

        public ParameterListView()
        {
            this.InitializeComponent();
        }

        public IEnvironmentContainer EnvironmentConatiner
        {
            get => (IEnvironmentContainer)GetValue(EnvironmentConatinerProperty);
            set => SetValueDp(EnvironmentConatinerProperty, value);
        }

        public static readonly DependencyProperty EnvironmentConatinerProperty = DependencyProperty.Register(
            nameof(EnvironmentConatiner),
            typeof(IEnvironmentContainer),
            typeof(ParameterListView),
            null);

        public ObservableCollection<Parameter> ItemsSource
        {
            get => (ObservableCollection<Parameter>)GetValue(ItemsSourceProperty);
            set
            {
                SetValueDp(ItemsSourceProperty, value);
            }
        }

        public ParamType ParameterType
        {
            get => (ParamType)GetValue(ParameterTypeProperty);
            set => SetValueDp(ParameterTypeProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(ObservableCollection<Parameter>),
            typeof(ParameterListView),
            null);

        public static readonly DependencyProperty ParameterTypeProperty = DependencyProperty.Register(
            "ParameterType",
            typeof(ParamType),
            typeof(ParameterListView),
            new PropertyMetadata(ParamType.Parameter));


        private async void DeleteParameter_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is Parameter p)
            {
                ItemsSource.Remove(p);
                await Task.Delay(1);
                ParameterDeleted?.Invoke(this, new DeletedItemArgs<Parameter>(p));
                TriggerUpdate();
            }
        }

        private void CheckBox_Modified(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).DataContext is Parameter p)
            {
                p.Enabled = !p.Enabled;
            }
            TriggerUpdate();
        }

        private void TriggerUpdate() => ValuesUpdated?.Invoke(this, new EventArgs());

        /// <summary>
        /// Searches through the list view
        /// for the newly added item and attempts
        /// to focus on the textbox containing the new text.
        /// </summary>
        /// <param name="focusOnKeyTextBox">
        /// If true, attempts to focus on the key textbox.
        /// If false, then value textbox.
        /// </param>
        /// <param name="newItem">
        /// Pointer to the newly created parameter item.
        /// Used to search through the parameter list's visual tree.
        /// </param>
        private void FocusOnNewText(bool focusOnKeyTextBox, object newItem)
        {
            // Search for the newly created list view item.
            // Note that the new item is not necessary in the last position
            // of the children. This is because the visual tree will contain
            // elements that have been deleted from view, but have yet to be
            // removed from the tree.
            var listViewItem = ParamList
                .ItemsPanelRoot
                .Children
                .Where(x => x is ListViewItem i && i.Content == newItem)
                .FirstOrDefault();

            // Focus on the textbox
            if (listViewItem is ListViewItem item
                && item.ContentTemplateRoot is Grid g
                && g.Children.ElementAtOrDefault(focusOnKeyTextBox ? 1 : 2) is TextBox t)
            {
                // Ensure focus is on the textbox.
                t.Focus(FocusState.Keyboard);

                // Ensure the cursor is at the end of the string.
                t.Select(t.Text.Length, 0);
            }
        }

        private async void NewKeyAdded(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = true;
            await NewTextChanging(true, args.NewText);
        }

        private async void NewValueAdded(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = true;
            await NewTextChanging(false, args.NewText);
        }

        private async Task NewTextChanging(bool isKey, string newText)
        {
            if (_creatingNew)
            {
                // This helps ensure
                // synchronous operation.
                return;
            }

            _creatingNew = true;
            var newItem = new Parameter(true, isKey ? newText : "", isKey ? "" : newText, ParameterType);
            ItemsSource.Add(newItem);

            // This delay allows the item to be rendered
            // in the UI so that it can be focused.
            await Task.Delay(1);

            // Focus on the new textbox.
            FocusOnNewText(isKey, newItem);
            TriggerUpdate();
            _creatingNew = false;
        }

        private void KeyValueChanged(object sender, TextChangedEventArgs e)
        {
            TriggerUpdate();
        }

        private void AddVariableClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is Parameter p)
            {
                AddVariableClicked?.Invoke(this, new AddedItemArgs<Parameter>(p));
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox t)
            {
                t.TextWrapping = TextWrapping.Wrap;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox t)
            {
                t.TextWrapping = TextWrapping.NoWrap;
            }
        }

        private void PrivateButtonClicked(object sender, RoutedEventArgs e)
        {
            Analytics.TrackEvent(Telemetry.PrivateClicked);
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (EnvironmentConatiner != null
                && KeyboardState.IsCtrlKeyPressed()
                && e.Key == Windows.System.VirtualKey.Space)
            {
                e.Handled = true;
                _ctrlSpaceContext = sender as TextBox;
                var flyout = FlyoutBase.GetAttachedFlyout(sender as FrameworkElement);
                var test = flyout as MenuFlyout;
                test.Items.Clear();

                var variables = EnvironmentConatiner.GetActiveVariables();
                variables.Add(new CustomInsertParameter
                {
                    Key = "Insert UTC DateTime",
                    CustomInsertString = DateTime.UtcNow.ToString("s") + "Z"
                });
                variables.Add(new CustomInsertParameter
                {
                    Key = "Insert local DateTime",
                    CustomInsertString = DateTime.Now.ToString("s") + "Z"
                });
                variables.Add(new CustomInsertParameter
                {
                    Key = "Random GUID",
                    CustomInsertString = Guid.NewGuid().ToString()
                });

                foreach (var v in variables)
                {
                    var item = new MenuFlyoutItem()
                    {
                        Text = v.Key,
                        DataContext = v
                    };
                    item.Click += VariableClicked;
                    test.Items.Add(item);
                }

                FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            }
        }

        private void VariableClicked(object sender, RoutedEventArgs e)
        {
            if (_ctrlSpaceContext is null)
            {
                return;
            }

            var item = sender as MenuFlyoutItem;
            if (item is MenuFlyoutItem && item.DataContext is CustomInsertParameter d)
            {
                _ctrlSpaceContext.Text = d.CustomInsertString;
            }
            else if (item is MenuFlyoutItem && item.DataContext is Parameter p)
            {
                _ctrlSpaceContext.Text = "{{" + p.Key + "}}";
            }

            _ctrlSpaceContext.Select(_ctrlSpaceContext.Text.Length, 0);
            _ctrlSpaceContext = null;
        }
    }

    internal sealed class CustomInsertParameter : Parameter
    {
        public string CustomInsertString { get; set; }
    }
}
