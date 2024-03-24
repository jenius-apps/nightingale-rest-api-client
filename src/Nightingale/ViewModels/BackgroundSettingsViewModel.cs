using Nightingale.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Nightingale.ViewModels;

/// <summary>
/// ViewModel for managing the background
/// image settings for the app.
/// </summary>
public class BackgroundSettingsViewModel
{
    private const string NoneFileName = "none.png";
    private const int ImageHeight = 100;
    private const string ImgDir = "ms-appx:///Assets/Backgrounds/";

    /// <remarks>
    /// This class was designed to
    /// be instantiated once per lifetime scope.
    /// </remarks>
    public BackgroundSettingsViewModel()
    {
        LoadImages();
        ThemeController.ThemeChanged += ReloadImages;
    }

    private void ReloadImages(object sender, EventArgs e)
    {
        Images.Clear();
        LoadImages();
    }

    /// <summary>
    /// List of images to display on screen.
    /// </summary>
    public ObservableCollection<Image> Images { get; set; } = new ObservableCollection<Image>();

    private async void LoadImages()
    {
        StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
        StorageFolder backgrounds = await assets.GetFolderAsync("Backgrounds");
        var isDarkTheme = App.RootFrame.ActualTheme == ElementTheme.Dark;
        var files = await backgrounds.GetFilesAsync();

        // Only retrieve the theme-specific images
        var themefiles = isDarkTheme
            ? files.Where(x => x.Name.StartsWith("dark") || x.Name == NoneFileName)
            : files.Where(x => x.Name.StartsWith("light") || x.Name == NoneFileName);

        foreach (var file in themefiles)
        {
            Image img = new Image
            {
                Height = ImageHeight,
                Source = new BitmapImage(new Uri(ImgDir + file.Name))
            };

            Images.Add(img);
        }
    }

    /// <summary>
    /// Updates the selected background.
    /// </summary>
    public void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.FirstOrDefault() is Image i &&
            i.Source is BitmapImage b &&
            b.UriSource?.Segments?.Length is int l &&
            l > 0)
        {
            var fileName = b.UriSource.Segments[l - 1];
            ThemeController.ChangeBackgroundImage(fileName == NoneFileName ? "" : fileName);
        }
    }
}
