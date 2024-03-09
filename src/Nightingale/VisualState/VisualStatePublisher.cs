using Nightingale.Core.Settings;
using Nightingale.Handlers;
using System;

#nullable enable

namespace Nightingale.VisualState;

public class VisualStatePublisher : IVisualStatePublisher
{
    private readonly IUserSettings _userSettings;
    private bool _tempLayoutSinglePane = false;

    public event EventHandler? SideBarVisibilityChanged;
    public event EventHandler? PaneLayoutToggled;

    public VisualStatePublisher(IUserSettings userSettings)
    {
        _userSettings = userSettings;
    }

    /// <inheritdoc/>
    public bool GetSideBarVisibility()
    {
        return _userSettings.Get<bool>(SettingsConstants.SideBarVisible);
    }

    public void SetSideBarVisibility(bool value)
    {
        _userSettings.Set<bool>(SettingsConstants.SideBarVisible, value);
        SideBarVisibilityChanged?.Invoke(this, new EventArgs());
    }

    public void SetPaneLayoutSideBySide(bool value)
    {
        _userSettings.Set<bool>(SettingsConstants.PaneLayoutSideBySide, value);
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

        return _userSettings.Get<bool>(SettingsConstants.PaneLayoutSideBySide);
    }
}
