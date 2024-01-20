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
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Nightingale.Auth
{
    public sealed partial class WebAuthDialog : ContentDialog
    {
        public WebAuthDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Authorization URI.
        /// </summary>
        public Uri AuthUri { get; set; }

        /// <summary>
        /// Callback URI.
        /// </summary>
        public Uri CallbackUri { get; set; }

        /// <summary>
        /// The URL returned after successful
        /// authorization.
        /// </summary>
        public string ReturnUrl { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        private void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            if (AuthUri == null)
            {
                throw new ArgumentNullException(nameof(AuthUri), "AuthUri needs initialization.");
            }
            if (CallbackUri == null)
            {
                throw new ArgumentNullException(nameof(CallbackUri), "CallbackUri needs initialization.");
            }

            // Delete cookies to prevent logging in with previous account.
            ClearCookies(new Uri(AuthUri.Scheme + "://" + AuthUri.Host));

            if (sender is WebView w)
            {
                w.Navigate(AuthUri);
            }
        }

        private void ClearCookies(Uri uri)
        {
            HttpBaseProtocolFilter myFilter = new HttpBaseProtocolFilter();
            HttpCookieManager cookieManager = myFilter.CookieManager;
            HttpCookieCollection cookieCollection = cookieManager.GetCookies(uri);
            foreach (HttpCookie cookie in cookieCollection)
            {
                cookieManager.DeleteCookie(cookie);
            }
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.Host == CallbackUri.Host)
            {
                ReturnUrl = args.Uri.AbsoluteUri;
                this.Hide();
            }
        }
    }
}
