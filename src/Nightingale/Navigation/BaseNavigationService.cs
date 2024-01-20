using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Navigation
{
    public abstract class BaseNavigationService
    {
        protected Frame Frame;

        public void GoBack()
        {
            EnsureFrameNotNull();

            if (Frame.CanGoBack && Frame.BackStackDepth > 0)
            {
                Frame.GoBack();
            }
        }

        public void SetFrame(Frame frame)
        {
            Frame = frame;
        }

        protected void EnsureFrameNotNull()
        {
            if (Frame == null)
            {
                throw new ArgumentNullException(nameof(Frame), "Frame in WorkspaceNavigationService is null.");
            }
        }
    }
}
