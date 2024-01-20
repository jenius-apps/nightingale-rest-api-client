using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Nightingale.Utilities
{
    /// <summary>
    /// Used to perform control overrides on pivot.
    /// </summary>
    public class PivotOverride : Pivot
    {
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            // Normally, Ctrl+Tab will change the selected pivot item. But
            // Ctrl+Tab should change the selected tab in the tab collection,
            // so we need to supress the pivot's handling of this key combo.
            if (KeyboardState.IsCtrlKeyPressed() && e.Key == VirtualKey.Tab)
            {
                e.Handled = false;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }
    }
}
