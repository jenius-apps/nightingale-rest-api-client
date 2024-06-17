using Nightingale.Core.Settings;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Handlers
{
    /// <summary>
    /// Class for setting and retrieving user settings.
    /// </summary>
    public class UserSettings : IUserSettings
    {
        public void Set<T>(string settingKey, object value)
        {
            ApplicationData.Current.LocalSettings.Values[settingKey] = (T)value;
        }

        public T Get<T>(string settingKey)
        {
            object result = ApplicationData.Current.LocalSettings.Values[settingKey];
            return result == null ? (T)SettingsConstants.Defaults[settingKey] : (T)result;
        }

        /// <summary>
        /// Sets user setting for SyncEnabled.
        /// </summary>
        public static void SetSyncEnabled(bool value)
        {
            ApplicationData.Current.LocalSettings.Values[SettingsConstants.SyncEnabledKey] = value;
        }

        /// <summary>
        /// Returns user setting for
        /// SyncEnabled. Default is false.
        /// </summary>
        /// <returns>bool</returns>
        public static bool GetSyncEnabled()
        {
            object result = ApplicationData.Current.LocalSettings.Values[SettingsConstants.SyncEnabledKey];
            return result == null ? false : (bool)result;
        }

        /// <summary>
        /// Sets user setting for selected theme.
        /// </summary>
        public static void SetTheme(SelectedTheme theme)
        {
            ApplicationData.Current.LocalSettings.Values[SettingsConstants.SelectedThemeKey] = (int)theme;
        }

        /// <summary>
        /// Returns user setting for selected theme.
        /// Default is dark theme.
        /// </summary>
        public static SelectedTheme GetTheme()
        {
            object result = ApplicationData.Current.LocalSettings.Values[SettingsConstants.SelectedThemeKey];
            return result == null ? SelectedTheme.Dark : (SelectedTheme)result;
        }

        /// <inheritdoc/>
        public Task SetAsync<T>(string settingKey, T value)
        {
            ApplicationData.Current.LocalSettings.Values[settingKey] = value;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<T> GetAsync<T>(string settingKey)
        {
            object value = ApplicationData.Current.LocalSettings.Values[settingKey];
            var result = value == null ? (T)SettingsConstants.Defaults[settingKey] : (T)value;

            return Task.FromResult(result);
        }
    }
}
