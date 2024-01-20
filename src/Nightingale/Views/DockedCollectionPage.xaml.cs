using Nightingale.ViewModels;
using Nightingale.Views.NavigationParameters;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DockedCollectionPage : Page
    {
        private CollectionViewModel ViewModel;
        private AuthControlViewModel AuthVm;

        public DockedCollectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is CollectionPageParameters param)
            {
                ViewModel = param.CollectionViewModel;
                AuthVm = param.AuthControlViewModel;
            }
            else
            {
                throw new ArgumentException("Incorrect " +
                    "navigation parameter for collection page");
            }
        }
    }
}
