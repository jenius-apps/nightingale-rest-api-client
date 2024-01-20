using Nightingale.Core.Settings;
using Nightingale.Handlers;
using System;

namespace Nightingale.VisualState
{
    public class VisualStatePublisher : IVisualStatePublisher
    {
        public event EventHandler SideBarVisibilityChanged;
        public event EventHandler PaneLayoutToggled;

        private bool _tempLayoutSinglePane = false;

        /// <inheritdoc/>
        public bool GetSideBarVisibility()
        {
            return UserSettings.Get<bool>(SettingsConstants.SideBarVisible);
        }

        public void SetSideBarVisibility(bool value)
        {
            UserSettings.Set<bool>(SettingsConstants.SideBarVisible, value);
            SideBarVisibilityChanged?.Invoke(this, new EventArgs());
        }

        public void SetPaneLayoutSideBySide(bool value)
        {
            UserSettings.Set<bool>(SettingsConstants.PaneLayoutSideBySide, value);
            PaneLayoutToggled?.Invoke(this, new EventArgs());
        }

        public void SetTempSinglePane(bool value)
        {
            _tempLayoutSinglePane = value;
            PaneLayoutToggled?.Invoke(this, new EventArgs());
        }

        public bool IsLayoutTwoPane()
        {
            if (_tempLayoutSinglePane)
            {
                return false;
            }

            return UserSettings.Get<bool>(SettingsConstants.PaneLayoutSideBySide);
        }
    }
}
