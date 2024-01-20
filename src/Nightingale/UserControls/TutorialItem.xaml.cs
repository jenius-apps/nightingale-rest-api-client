using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class TutorialItem : UserControl
    {
        public TutorialItem()
        {
            this.InitializeComponent();
        }

        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            nameof(Image),
            typeof(ImageSource),
            typeof(TutorialItem),
            null);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(TutorialItem),
            null);

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            nameof(Caption),
            typeof(string),
            typeof(TutorialItem),
            null);
    }
}
