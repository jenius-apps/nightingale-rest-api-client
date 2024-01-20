using Windows.System;
using Windows.UI.Core;

namespace Nightingale.Utilities
{
    public static class KeyboardState
    {
        public static bool IsCtrlKeyPressed()
        {
            return IsKeyPressed(VirtualKey.Control);
        }

        public static bool IsShiftKeyPressed()
        {
            return IsKeyPressed(VirtualKey.Shift);
        }

        private static bool IsKeyPressed(VirtualKey virtualKey)
        {
            var state = CoreWindow.GetForCurrentThread().GetKeyState(virtualKey);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}
