using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Dialogs
{
    public sealed partial class TutorialDialog : ContentDialog
    {
        public TutorialDialog()
        {
            this.InitializeComponent();
        }

        private void Close()
        {
            this.Hide();
        }

        private void Back()
        {
            if (flipView.SelectedIndex > 0)
            {
                flipView.SelectedIndex -= 1;
            }
        }

        private void Next()
        {
            if (flipView.SelectedIndex < flipView.Items.Count - 1)
            {
                flipView.SelectedIndex += 1;
            }
        }
    }
}
