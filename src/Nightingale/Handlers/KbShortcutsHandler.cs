using Microsoft.AppCenter.Analytics;
using Nightingale.Tabs.Services;
using Nightingale.Utilities;
using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Core;

namespace Nightingale.Handlers
{
    /// <summary>
    /// Class that handles all app-wide
    /// keyboard shortcuts. Designed to be used as
    /// singleton for the lifetime scope.
    /// </summary>
    public class KbShortcutsHandler
    {
        public event EventHandler RenameRequested;

        private readonly ITabCollectionContainer _tabCollectionContainer;

        public KbShortcutsHandler(ITabCollectionContainer tabCollectionContainer)
        {
            _tabCollectionContainer = tabCollectionContainer
                ?? throw new ArgumentNullException(nameof(tabCollectionContainer));

            CoreWindow.GetForCurrentThread().KeyDown += KbShortcutsHandler_KeyDown;
        }

        private void KbShortcutsHandler_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (KeyboardState.IsCtrlKeyPressed() && args.VirtualKey == VirtualKey.T)
            {
                args.Handled = true;
                _tabCollectionContainer.AddTempTab();
                Analytics.TrackEvent(Telemetry.CtrlT);
            }
            else if (KeyboardState.IsCtrlKeyPressed() && args.VirtualKey == VirtualKey.W)
            {
                args.Handled = true;
                _tabCollectionContainer.RemoveCurrentTab();
                Analytics.TrackEvent(Telemetry.CtrlW);
            }
            else if (KeyboardState.IsCtrlKeyPressed() && !KeyboardState.IsShiftKeyPressed() && args.VirtualKey == VirtualKey.Tab)
            {
                args.Handled = true;
                _tabCollectionContainer.SelectNextTab();
                Analytics.TrackEvent(Telemetry.CtrlTab);
            }
            else if (KeyboardState.IsCtrlKeyPressed() && KeyboardState.IsShiftKeyPressed() && args.VirtualKey == VirtualKey.Tab)
            {
                args.Handled = true;
                _tabCollectionContainer.SelectPreviousTab();
                Analytics.TrackEvent(Telemetry.CtrlShiftTab);
            }
            else if (args.VirtualKey == VirtualKey.F2)
            {
                RenameRequested?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
