using System;

namespace Nightingale.VisualState
{
    public interface IVisualStatePublisher
    {
        event EventHandler SideBarVisibilityChanged;
        event EventHandler PaneLayoutToggled;

        /// <summary>
        /// Returns the current sidebar
        /// visibility state.
        /// </summary>
        bool GetSideBarVisibility();

        void SetSideBarVisibility(bool value);

        void SetPaneLayoutSideBySide(bool value);

        void SetTempSinglePane(bool value);

        bool IsLayoutTwoPane();
    }
}
