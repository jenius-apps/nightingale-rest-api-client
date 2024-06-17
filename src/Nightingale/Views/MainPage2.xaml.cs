using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Settings;
using Nightingale.Dialogs;
using Nightingale.Handlers;
using Nightingale.Navigation;
using Nightingale.Utilities;
using Nightingale.ViewModels;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Nightingale.Views;

public sealed partial class MainPage2 : Page
{
    public MainPageViewModel ViewModel { get; set; }

    public MainPage2()
    {
        this.InitializeComponent();
        App.Current.UnhandledException += Current_UnhandledException;

        // Set title bar. Required for interactive elements in bar.
        // Ref: https://docs.microsoft.com/en-us/windows/uwp/design/shell/title-bar#interactive-content
        var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;
        // Set XAML element as a draggable region.
        Window.Current.SetTitleBar(AppTitleBar);

        InitializeBackground();

        ThemeController.BackgroundImageChanged += ChangeBackground;
        ThemeController.ThemeChanged += ChangeBackgroundImageTheme;

        ViewModel = App.Services.GetRequiredService<MainPageViewModel>();
    }

    private void ChangeBackgroundImageTheme(object sender, EventArgs e)
    {
        var imageName = App.Services.GetRequiredService<IUserSettings>().Get<string>(SettingsConstants.BackgroundImage);
        if (!string.IsNullOrWhiteSpace(imageName))
        {
            var isDarkTheme = App.RootFrame.ActualTheme == ElementTheme.Dark;

            // Assume image names look like "dark_plant.png"
            var nameSplit = imageName.Split('_');
            imageName = $"{(isDarkTheme ? "dark" : "light")}_{nameSplit[1]}";
        }
        ThemeController.ChangeBackgroundImage(imageName);
    }

    private void ChangeBackground(object sender, CustomEventArgs.AddedItemArgs<string> e)
    {
        SetBackground(e.AddedItem);
    }

    private void InitializeBackground()
    {
        var imageName = App.Services.GetRequiredService<IUserSettings>().Get<string>(SettingsConstants.BackgroundImage);
        SetBackground(imageName);
    }

    private void SetBackground(string imageName)
    {
        if (string.IsNullOrWhiteSpace(imageName))
        {
            MainGrid.Background = new SolidColorBrush(Colors.Transparent);
        }
        else
        {
            MainGrid.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Backgrounds/" + imageName)),
                Stretch = Stretch.UniformToFill
            };
        }

        private async void OnClick(object sender, RoutedEventArgs e)
        {
            //var x = await App.TestAppService.SumAsync(1, 2);
            //Debug.WriteLine(x);
        }
    }

    /// <summary>
    /// Required for interactive elements in bar.
    /// </summary>
    /// <remarks>
    /// Ref: https://docs.microsoft.com/en-us/windows/uwp/design/shell/title-bar#interactive-content
    /// </remarks>
    private void TitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
        AppTitleBar.Height = sender.Height;
    }

    private async void Current_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        var saveTask = ViewModel.SaveWorkspaceAsync();

        App.Services.GetRequiredService<ITelemetry>().TrackError(e.Exception, new Dictionary<string, string>
        {
            { "message", e.Message },
            { "Call stack", e.Exception.StackTrace }
        });

        var resourceLoader = ResourceLoader.GetForCurrentView();

        var dialog = new ContentDialog()
        {
            Title = resourceLoader.GetString("ErrorTitle"),
            Content = resourceLoader.GetString("ErrorContent")
                + Environment.NewLine
                + Environment.NewLine
                + e.Message
                + Environment.NewLine
                + e.Exception.StackTrace,
            CloseButtonText = resourceLoader.GetString("Okay"),
            CornerRadius = new CornerRadius(8)
        };

        if (!BaseDialogService.IsDialogActive)
        {
            BaseDialogService.IsDialogActive = true;
            await dialog.ShowAsync();
            BaseDialogService.IsDialogActive = false;
        }

        await saveTask;
        App.Current.Exit();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        var storage = App.Services.GetRequiredService<IStorage>();
        ViewModel.ActiveFileName = storage.GetFileName();

        // Configure navigation service to point to the proper frame.
        var navigationService = App.Services.GetRequiredService<IWorkspaceNavigationService>();
        navigationService.SetFrame(this.WorkspaceFrame);

        // Set task bar title
        if (!string.IsNullOrWhiteSpace(ViewModel.ActiveFileName))
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = storage.GetFileName();
        }

        // Resolve the kb shortcut handler because it functions as a singleton.
        App.Services.GetRequiredService<KbShortcutsHandler>();
        await ViewModel.LoadAsync();
    }

    private async void Workspace_Deleted(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (((Button)sender).DataContext is Core.Models.Workspace w)
        {
            await ViewModel.DeleteWorkspaceAsync(w);
        }
    }

    private async void WorkspaceName_Edit(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (((Button)sender).DataContext is Core.Models.Workspace w)
        {
            await ViewModel.RenameWorkspaceAsync(w);
        }
    }

    /// <summary>
    /// Helper method for collapsing the menu when user
    /// taps a header twice.
    /// </summary>
    private void MenuHeaderTextBlockTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        if (MenuPivot?.SelectedItem is PivotItem currentItem
            && sender is TextBlock headerTextBlock)
        {

            bool collapsed;
            if (headerTextBlock.Tag.ToString() == currentItem.Tag.ToString())
            {
                // Toggle visibility of the pivot item contents.
                currentItem.Visibility = currentItem.Visibility == Visibility.Collapsed
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                collapsed = currentItem.Visibility == Visibility.Collapsed;
            }
            else
            {
                // If user clicks on a different pivot item,
                // ensure that the contents are visible.
                currentItem.Visibility = Visibility.Visible;
                collapsed = false;
            }

            App.Services.GetRequiredService<ITelemetry>().TrackEvent(Telemetry.MenuHeaderClicked, new Dictionary<string, string>
            {
                { "headerClicked", headerTextBlock.Tag.ToString() },
                { "collapsed", collapsed.ToString() }
            });
        }
        else
        {
            throw new InvalidOperationException(
                "Error when casting the menu pivot item or textblock. " +
                "Are you sure the pivot xaml is configured correctly?");
        }
    }

    /// <summary>
    /// Shows whats new flyout
    /// </summary>
    private void OpenWhatsNewFlyout(object sender, RoutedEventArgs e)
    {
        App.Services.GetRequiredService<ITelemetry>().TrackEvent(Telemetry.EmailDev);
        whatsNewFlyout.ShowAt(sender as FrameworkElement);
    }
}
