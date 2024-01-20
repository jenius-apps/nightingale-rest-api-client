using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class WhatsNewControl : UserControl
    {
        public WhatsNewControl()
        {
            this.InitializeComponent();
        }

        private string AppVersion
        {
            get
            {
                var version = SystemInformation.ApplicationVersion;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        private readonly string Changelog = @"
- Fixed issues with environment variables when importing Insomnia (GitHub #123). Thanks to contributor **@jamesmcroft**.
- Added support for exporting collections to Postman files. Try it by right clicking on a collection. (GitHub #146). Thanks to contributor **@Ombrelin**.
- Added support for importing OData files. Thanks to contributor **@paule96**.
";
    }
}
