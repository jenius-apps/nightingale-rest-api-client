using JeniusApps.Common.Telemetry;
using Nightingale.Tabs.Services;
using Nightingale.Utilities;
using System;
using Windows.System;
using Windows.UI.Core;

namespace Nightingale.Handlers;

/// <summary>
/// Class that handles all app-wide
/// keyboard shortcuts. Designed to be used as
/// singleton for the lifetime scope.
/// </summary>
public class KbShortcutsHandler
{
    public event EventHandler RenameRequested;

    private readonly ITabCollectionContainer _tabCollectionContainer;
    private readonly ITelemetry _telemetry;

    public KbShortcutsHandler(
        ITabCollectionContainer tabCollectionContainer,
        ITelemetry telemetry)
    {
        _tabCollectionContainer = tabCollectionContainer;
        _telemetry = telemetry;

        CoreWindow.GetForCurrentThread().KeyDown += KbShortcutsHandler_KeyDown;
    }

    private void KbShortcutsHandler_KeyDown(CoreWindow sender, KeyEventArgs args)
    {
        if (KeyboardState.IsCtrlKeyPressed() && args.VirtualKey == VirtualKey.T)
        {
            args.Handled = true;
            _tabCollectionContainer.AddTempTab();
            _telemetry.TrackEvent(Telemetry.CtrlT);
        }
        else if (KeyboardState.IsCtrlKeyPressed() && args.VirtualKey == VirtualKey.W)
        {
            args.Handled = true;
            _tabCollectionContainer.RemoveCurrentTab();
            _telemetry.TrackEvent(Telemetry.CtrlW);
        }
        else if (KeyboardState.IsCtrlKeyPressed() && !KeyboardState.IsShiftKeyPressed() && args.VirtualKey == VirtualKey.Tab)
        {
            args.Handled = true;
            _tabCollectionContainer.SelectNextTab();
            _telemetry.TrackEvent(Telemetry.CtrlTab);
        }
        else if (KeyboardState.IsCtrlKeyPressed() && KeyboardState.IsShiftKeyPressed() && args.VirtualKey == VirtualKey.Tab)
        {
            args.Handled = true;
            _tabCollectionContainer.SelectPreviousTab();
            _telemetry.TrackEvent(Telemetry.CtrlShiftTab);
        }
        else if (args.VirtualKey == VirtualKey.F2)
        {
            RenameRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
