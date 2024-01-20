using System.Threading.Tasks;

namespace Nightingale.Core.Settings
{
    /// <summary>
    /// Interface for getting and setting
    /// user settings.
    /// </summary>
    public interface IUserSettings
    {
        /// <summary>
        /// Saves settings into persistent local
        /// storage.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="settingKey">The settings key, generally found in <see cref="SettingsConstants"/>.</param>
        /// <param name="value">The value to save.</param>
        Task SetAsync<T>(string settingKey, T value);

        /// <summary>
        /// Retrieves the value for the desired settings key.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="settingKey">The settings key, generally found in <see cref="SettingsConstants"/>.</param>
        /// <returns>The desired value or returns the default value.</returns>
        Task<T> GetAsync<T>(string settingKey);
    }
}
