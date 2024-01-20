using Nightingale.Core.Services;
using Windows.ApplicationModel.Resources;

namespace Nightingale.Utilities
{
    public sealed class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            var resourceLoader = ResourceLoader.GetForCurrentView("appsettings");
            TelemetryApiKey = resourceLoader.GetString(nameof(TelemetryApiKey));
            ActiproLicenseKey = resourceLoader.GetString(nameof(ActiproLicenseKey));
            ActiproLicensee = resourceLoader.GetString(nameof(ActiproLicensee));
        }

        /// <inheritdoc/>
        public string TelemetryApiKey { get; }

        /// <inheritdoc/>
        public string ActiproLicenseKey { get; }

        /// <inheritdoc/>
        public string ActiproLicensee { get; }
    }
}
