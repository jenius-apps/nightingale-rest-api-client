using Nightingale.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CodeGeneratorPage : Page
    {
        public CodeGeneratorPage()
        {
            this.InitializeComponent();
        }

        public CodeGenPageViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is CodeGenPageViewModel vm)
            {
                ViewModel = vm;
                ViewModel.GenerateCode();
            }
        }
    }
}
