using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Nightingale.Handlers;
using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Globalization;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Nightingale.Utilities;
using Windows.Foundation.Metadata;
using Nightingale.Core.Interfaces;
using Windows.Storage;
using Nightingale.Views;
using Nightingale.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Nightingale
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Frame RootFrame { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            ThemeController.ThemeChanged += ThemeController_ThemeChanged;
        }

        private void ThemeController_ThemeChanged(object sender, EventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.RequestedTheme = ThemeController.GetTheme();
                CustomizeTitleBar(rootFrame.ActualTheme == ElementTheme.Dark);
            }
        }

        private async Task<IStorage> InitializeDefaultStorageAsync()
        {
            // Get default document file
            Windows.Storage.IStorageItem documentFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync(DocumentStorage.DefaultLocalStorageFileName);

            if (documentFile == null)
            {
                // if file does not exist, create a new default document file
                documentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(DocumentStorage.DefaultLocalStorageFileName, CreationCollisionOption.ReplaceExisting);
            }

            IStorage documentStorage = await InitializeDocumentStorageAsync(documentFile);
            return documentStorage;
        }

        private async Task InitializeLocalHostPermissionAsync()
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync("ExemptGroup");
            }
        }

        private void ActivateActipro()
        {
            var appsettings = _serviceProvider?.GetRequiredService<IAppSettings>();
            if (string.IsNullOrEmpty(appsettings.ActiproLicensee) || string.IsNullOrEmpty(appsettings.ActiproLicenseKey))
            {
                return;
            }

            ActiproSoftware.Products.ActiproLicenseManager.RegisterLicense(
                appsettings.ActiproLicensee,
                appsettings.ActiproLicenseKey);
        }

        private async Task InitializeTelemetryAsync()
        {
            /// Initialize analytics
            AppCenter.SetCountryCode(new GeographicRegion().CodeTwoLetter);
            AppCenter.Start(_serviceProvider?.GetRequiredService<IAppSettings>().TelemetryApiKey, [typeof(Analytics), typeof(Crashes)]);
#if DEBUG
            bool telemetryEnabled = false;
#else
            if (_serviceProvider?.GetRequiredService<IUserSettings>() is { } settings)
            {
                bool telemetryEnabled = settings.Get<bool>(Core.Settings.SettingsConstants.TelemetryEnabledKey);
            }
#endif
            await AppCenter.SetEnabledAsync(telemetryEnabled);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;


            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                await InitializeLocalHostPermissionAsync();
                rootFrame.RequestedTheme = ThemeController.GetTheme();
                CustomizeTitleBar(rootFrame.ActualTheme == ElementTheme.Dark);
                RootFrame = rootFrame;
                RootFrame.ActualThemeChanged += rootFrame_ActualThemeChanged;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    var storage = await InitializeDefaultStorageAsync();
                    _serviceProvider = ConfigureServices(storage);
                    await InitializeTelemetryAsync();
                    ActivateActipro();
                    rootFrame.Navigate(typeof(MainPage2));
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        private void rootFrame_ActualThemeChanged(FrameworkElement sender, object args)
        {
            CustomizeTitleBar(sender.ActualTheme == ElementTheme.Dark);
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            base.OnFileActivated(args);
            DocumentStorage documentStorage = await InitializeDocumentStorageAsync(args.Files[0]);
            Frame rootFrame = Window.Current.Content as Frame;

            // If app is already open, no need to initialize. Just navigate.
            if (rootFrame != null)
            {
                Window.Current.Activate();
                return;
            }

            // If app was not open, then begin startup sequence.
            _serviceProvider = ConfigureServices(documentStorage);
            await InitializeTelemetryAsync();
            ActivateActipro();
            await InitializeLocalHostPermissionAsync();
            rootFrame = new Frame();
            Window.Current.Content = rootFrame;
            rootFrame.RequestedTheme = ThemeController.GetTheme();
            CustomizeTitleBar(rootFrame.ActualTheme == ElementTheme.Dark);
            RootFrame = rootFrame;
            RootFrame.ActualThemeChanged += rootFrame_ActualThemeChanged;

            rootFrame.Navigate(documentStorage == null ? typeof(Views.InvalidFilePage) : typeof(Views.MainPage2), documentStorage);
            rootFrame.BackStack.Clear();
            Window.Current.Activate();
            Analytics.TrackEvent(Telemetry.NcfFileLaunched);
        }

        private async Task<DocumentStorage> InitializeDocumentStorageAsync(Windows.Storage.IStorageItem file)
        {
            if (file is Windows.Storage.StorageFile storageFile)
            {
                try
                {
                    var documentStorage = new DocumentStorage(storageFile);
                    await documentStorage.InitializeAsync();
                    return documentStorage;
                }
                catch
                {
                    Analytics.TrackEvent(Telemetry.NcfFileInvalid);
                }
            }

            return null;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Removes title bar and sets title bar button backgrounds to transparent.
        /// </summary>
        private static void CustomizeTitleBar(bool darkTheme)
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            var viewTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            viewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            viewTitleBar.ButtonForegroundColor = darkTheme ? Colors.LightGray : Colors.Black;
        }

        public static async void NewWindow(Type pageType, object param)
        {
            var newWindow = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                newViewId = ApplicationView.GetForCurrentView().Id;
                var frame = new Frame();
                Window.Current.Content = frame;
                frame.RequestedTheme = ThemeController.GetTheme();
                CustomizeTitleBar(frame.ActualTheme == ElementTheme.Dark);
                frame.Navigate(pageType, param);
                Window.Current.Activate();
            });

            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }
    }
}
