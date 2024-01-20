using Nightingale.ViewModels;

namespace Nightingale.Views.NavigationParameters
{
    public class RequestPageParameters
    {
        public RequestPageViewModel RequestPageViewModel { get; set; }

        public RequestControlViewModel RequestControlViewModel { get; set; }

        public UrlBarViewModel UrlBarViewModel { get; set; }

        public AuthControlViewModel AuthControlViewModel { get; set; }

        public RequestBodyViewModel RequestBodyViewModel { get; set; }

        public BodyControlViewModel BodyControlViewModel { get; set; }

        public StatusBarViewModel StatusBarViewModel { get; set; }
    }
}
