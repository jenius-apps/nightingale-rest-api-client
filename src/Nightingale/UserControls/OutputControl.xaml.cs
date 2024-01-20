using Nightingale.Utilities;
using Nightingale.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class OutputControl : ObservableUserControl
    {
        public event EventHandler SaveBodyClicked;
        public event EventHandler ToggleHtmlPreviewClicked;
        public event EventHandler CopyClicked;

        public OutputControl()
        {
            this.InitializeComponent();
        }

        public int BodyTypeIndex
        {
            get => (int)GetValue(BodyTypeIndexProperty);
            set => SetValueDp(BodyTypeIndexProperty, value);
        }

        public bool ContentVisible
        {
            get => (bool)GetValue(ContentVisibleProperty);
            set => SetValueDp(ContentVisibleProperty, value);
        }

        public bool IsTextIndex
        {
            get => (bool)GetValue(IsTextIndexProperty);
            set => SetValueDp(IsTextIndexProperty, value);
        }

        public bool IsJsonIndex
        {
            get => (bool)GetValue(IsJsonIndexProperty);
            set => SetValueDp(IsJsonIndexProperty, value);
        }

        public bool IsXmlIndex
        {
            get => (bool)GetValue(IsXmlIndexProperty);
            set => SetValueDp(IsXmlIndexProperty, value);
        }

        public bool IsHtmlIndex
        {
            get => (bool)GetValue(IsHtmlIndexProperty);
            set => SetValueDp(IsHtmlIndexProperty, value);
        }

        public bool IsBytesIndex
        {
            get => (bool)GetValue(IsBytesIndexProperty);
            set => SetValueDp(IsBytesIndexProperty, value);
        }

        public bool IsImageIndex
        {
            get => (bool)GetValue(IsImageIndexProperty);
            set => SetValueDp(IsImageIndexProperty, value);
        }

        public bool EditorVisible
        {
            get => (bool)GetValue(EditorVisibleProperty);
            set => SetValueDp(EditorVisibleProperty, value);
        }

        public SyntaxType SyntaxType
        {
            get => (SyntaxType)GetValue(SyntaxTypeProperty);
            set => SetValueDp(SyntaxTypeProperty, value);
        }

        public string Body
        {
            get => (string)GetValue(BodyProperty);
            set => SetValueDp(BodyProperty, value);
        }

        public bool HtmlPreviewVisible
        {
            get => (bool)GetValue(HtmlPreviewVisibleProperty);
            set => UpdateHtmlPreview(value);
        }

        public static readonly DependencyProperty HtmlPreviewVisibleProperty = DependencyProperty.Register(
            nameof(HtmlPreviewVisible),
            typeof(bool),
            typeof(OutputControl),
            null);

        private async void UpdateHtmlPreview(bool value)
        {
            SetValueDp(HtmlPreviewVisibleProperty, value);
            await Task.Delay(100); // delay helps the UI load things asynchronously 

            if (value)
            {
                try
                {
                    if (!(HtmlWebViewBorder.Child is WebView))
                    {
                        // Using separate thread for improved performance.
                        // Default is on the UI thread, which can lead to UI freeze.
                        // Ref: https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.WebView?view=winrt-18362#execution-modes
                        var webview = new WebView(WebViewExecutionMode.SeparateThread);
                        HtmlWebViewBorder.Child = webview;
                    }

                    if (HtmlWebViewBorder.Child is WebView w)
                    {
                        w.NavigateToString(this.Body);
                    }
                }
                catch
                {
                    // Don't crash if body is invalid html
                }
            }
        }

        public bool RawBytesVisible
        {
            get => (bool)GetValue(RawBytesVisibleProperty);
            set => SetValueDp(RawBytesVisibleProperty, value);
        }

        public string RawBytesString
        {
            get => (string)GetValue(RawBytesStringProperty);
            set => SetValueDp(RawBytesStringProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValueDp(ImageSourceProperty, value);
        }

        public bool ErrorMessageVisible
        {
            get => (bool)GetValue(ErrorMessageVisibleProperty);
            set => SetValueDp(ErrorMessageVisibleProperty, value);
        }

        public bool NoContentMessageVisible
        {
            get => (bool)GetValue(NoContentMessageVisibleProperty);
            set => SetValueDp(NoContentMessageVisibleProperty, value);
        }

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValueDp(ErrorMessageProperty, value);
        }

        public string HtmlText
        {
            get => (string)GetValue(HtmlTextProperty);
            set => SetValueDp(HtmlTextProperty, value);
        }

        public static readonly DependencyProperty HtmlTextProperty = DependencyProperty.Register(
            "HtmlText",
            typeof(string),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource",
            typeof(ImageSource),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            "ErrorMessage",
            typeof(string),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty NoContentMessageVisibleProperty = DependencyProperty.Register(
            "NoContentMessageVisible",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty ErrorMessageVisibleProperty = DependencyProperty.Register(
            "ErrorMessageVisible",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty RawBytesStringProperty = DependencyProperty.Register(
            "RawBytesString",
            typeof(string),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty RawBytesVisibleProperty = DependencyProperty.Register(
            "RawBytesVisible",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty BodyProperty = DependencyProperty.Register(
            "Body",
            typeof(string),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty SyntaxTypeProperty = DependencyProperty.Register(
            "SyntaxType",
            typeof(SyntaxType),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty EditorVisibleProperty = DependencyProperty.Register(
            "EditorVisible",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty IsTextIndexProperty = DependencyProperty.Register(
            "IsTextIndex",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty IsJsonIndexProperty = DependencyProperty.Register(
            "IsJsonIndex",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty IsXmlIndexProperty = DependencyProperty.Register(
            "IsXmlIndex",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty IsHtmlIndexProperty = DependencyProperty.Register(
            "IsHtmlIndex",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty IsBytesIndexProperty = DependencyProperty.Register(
            "IsBytesIndex",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty IsImageIndexProperty = DependencyProperty.Register(
            "IsImageIndex",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty ContentVisibleProperty = DependencyProperty.Register(
            "ContentVisible",
            typeof(bool),
            typeof(OutputControl),
            null);

        public static readonly DependencyProperty BodyTypeIndexProperty = DependencyProperty.Register(
            "BodyTypeIndex",
            typeof(int),
            typeof(OutputControl),
            null);

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                BodyTypeIndex = rb.GetIndexWithinGroup();
            }
        }

        private void SaveBody() => SaveBodyClicked?.Invoke(this, new EventArgs());

        private void ToggleHtmlPreview() => ToggleHtmlPreviewClicked?.Invoke(this, new EventArgs());

        private void CopyOutput() => CopyClicked?.Invoke(this, new EventArgs());

        private void ImageView_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (sender is Image i)
            {
                ImageFlyout?.ShowAt(i, e.GetPosition(i));
            }
        }

        private void ImageView_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            if (sender is Image i)
            {
                ImageFlyout?.ShowAt(i, e.GetPosition(i));
            }
        }
    }
}
