using Nightingale.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class RequestControl : UserControl
    {
        public RequestControl()
        {
            this.InitializeComponent();
        }

        public RequestPageViewModel PageViewModel
        {
            get => (RequestPageViewModel)GetValue(PageViewModelProperty);
            set => SetValue(PageViewModelProperty, value);
        }

        public static DependencyProperty PageViewModelProperty = DependencyProperty.Register(
            nameof(PageViewModel),
            typeof(RequestPageViewModel),
            typeof(RequestControl),
            null);

        public RequestControlViewModel ViewModel
        {
            get => (RequestControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(RequestControlViewModel),
            typeof(RequestControl),
            null);

        public UrlBarViewModel UrlBarViewModel
        {
            get => (UrlBarViewModel)GetValue(UrlBarViewModelProperty);
            set => SetValue(UrlBarViewModelProperty, value);
        }

        public static DependencyProperty UrlBarViewModelProperty = DependencyProperty.Register(
            nameof(UrlBarViewModel),
            typeof(UrlBarViewModel),
            typeof(RequestControl),
            null);

        public AuthControlViewModel AuthControlViewModel
        {
            get => (AuthControlViewModel)GetValue(AuthControlViewModelProperty);
            set => SetValue(AuthControlViewModelProperty, value);
        }

        public static DependencyProperty AuthControlViewModelProperty = DependencyProperty.Register(
            nameof(AuthControlViewModel),
            typeof(AuthControlViewModel),
            typeof(RequestControl),
            null);

        public RequestBodyViewModel RequestBodyViewModel
        {
            get => (RequestBodyViewModel)GetValue(RequestBodyViewModelProperty);
            set => SetValue(RequestBodyViewModelProperty, value);
        }

        public static DependencyProperty RequestBodyViewModelProperty = DependencyProperty.Register(
            nameof(RequestBodyViewModel),
            typeof(RequestBodyViewModel),
            typeof(RequestControl),
            null);

        public BodyControlViewModel BodyControlViewModel
        {
            get => (BodyControlViewModel)GetValue(BodyControlViewModelProperty);
            set => SetValue(BodyControlViewModelProperty, value);
        }

        public static DependencyProperty BodyControlViewModelProperty = DependencyProperty.Register(
            nameof(BodyControlViewModel),
            typeof(BodyControlViewModel),
            typeof(RequestControl),
            null);

        public StatusBarViewModel StatusBarViewModel
        {
            get => (StatusBarViewModel)GetValue(StatusBarViewModelProperty);
            set => SetValue(StatusBarViewModelProperty, value);
        }

        public static DependencyProperty StatusBarViewModelProperty = DependencyProperty.Register(
            nameof(StatusBarViewModel),
            typeof(StatusBarViewModel),
            typeof(RequestControl),
            null);

        private void MethodChanged(object sender, CustomEventArgs.AddedItemArgs<string> e)
        {
            // request control view model 
            // handles changing the value in the request.
            this.ViewModel.MethodChanged(sender, e);

            // Url bar vm handles if "CUSTOM" was selected.
            this.UrlBarViewModel.MethodChanged(sender, e);
        }

        private void SendRequestClicked()
        {
            UrlBarViewModel.AddRecentUrl(ViewModel.BaseUrl);
            ViewModel.SendRequestNoDownload();
        }

        private void SendRequestAndDownloadClicked()
        {
            UrlBarViewModel.AddRecentUrl(ViewModel.BaseUrl);
            ViewModel.SendRequestAndDownload();
        }

        private void UrlSuggestionClicked(object sender, CustomEventArgs.AddedItemArgs<string> e)
        {
            ViewModel.BaseUrl = e.AddedItem;
        }

        private void UrlBarControl_UrlTextBoxTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender is AutoSuggestBox box)
            {
                if (box.Text.StartsWith("curl"))
                {
                    ViewModel.PasteCurl(box.Text);
                    RequestBodyViewModel.UpdateBody();
                }
                else
                {
                    box.ItemsSource = box.Text.Length < 3 ? null : UrlBarViewModel.GetSimilarUrls(box.Text);
                    ViewModel.BaseUrl = box.Text;
                }
            }
        }

        private void ClearResponse(object sender, RoutedEventArgs e)
        {
            // Remove the image from view.
            BodyControlViewModel.Dispose();

            // Delete the entire response.
            ViewModel.ClearResponse();
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void InputGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
