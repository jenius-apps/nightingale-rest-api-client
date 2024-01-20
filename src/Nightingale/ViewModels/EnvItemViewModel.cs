using Nightingale.Core.Models;
using System;
using Environment = Nightingale.Core.Models.Environment;
using Windows.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Nightingale.ViewModels
{
    /// <summary>
    /// View model that wraps around
    /// <see cref="Environment"/> to provide
    /// functions that operate solely on the UI
    /// of this environment.
    /// </summary>
    /// <remarks>
    /// This class was originally created to support
    /// opening a rename flyout for the environment
    /// and closing the flyout all within this viewmodel.
    /// </remarks>
    public class EnvItemViewModel : ObservableBase
    {
        private object _flyout;

        public EnvItemViewModel(Environment environment)
        {
            Env = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        /// <summary>
        /// The environment that this view model
        /// wraps around.
        /// </summary>
        public Environment Env { get; }

        public string NewName
        {
            get => _newName;
            set
            {
                if (_newName != value)
                {
                    _newName = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _newName;

        /// <summary>
        /// Captures a reference to the rename flyout
        /// so it can be closed programmatically later.
        /// </summary>
        public void RenameFlyoutOpened(object sender, object e)
        {
            _flyout = sender;
            NewName = Env.Name;
        }

        /// <summary>
        /// Handles the key down event of the rename textbox.
        /// </summary>
        public void RenameKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                ConfirmRename();
            }
            else if (e.Key == VirtualKey.Escape)
            {
                CancelRename();
            }
        }

        /// <summary>
        /// Places text cursor at the end of a textbox.
        /// </summary>
        public void PlaceCursorAtEnd(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox t)
            {
                t.Select(t.Text.Length, 0);
            }
        }

        /// <summary>
        /// Saves the new name for environment
        /// and closes flyout.
        /// </summary>
        public void ConfirmRename()
        {
            Env.Name = NewName;
            if (_flyout is Flyout f) f.Hide();
        }

        /// <summary>
        /// Closes flyout and does not save
        /// new name.
        /// </summary>
        public void CancelRename()
        {
            if (_flyout is Flyout f) f.Hide();
        }
    }
}
