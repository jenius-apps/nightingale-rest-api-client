using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Settings;
using Nightingale.CustomEventArgs;
using Nightingale.Handlers;
using System;
using Windows.UI.Xaml;

namespace Nightingale.Utilities
{
    public class ThemeController
    {
        public static event EventHandler ThemeChanged;
        public static event EventHandler<AddedItemArgs<string>> BackgroundImageChanged;

        public static ElementTheme GetTheme()
        {
            SelectedTheme theme = UserSettings.GetTheme();
            return (ElementTheme)theme;
        }

        public static void ChangeTheme(SelectedTheme selectedTheme)
        {
            UserSettings.SetTheme(selectedTheme);
            ThemeChanged?.Invoke(new ThemeController(), new EventArgs());
        }

        public static void ChangeBackgroundImage(string imageName)
        {
            App.Services.GetRequiredService<IUserSettings>().Set<string>(SettingsConstants.BackgroundImage, imageName);
            BackgroundImageChanged?.Invoke(new ThemeController(), new AddedItemArgs<string>(imageName));
        }
    }
}
