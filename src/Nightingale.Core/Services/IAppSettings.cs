using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Services
{
    public interface IAppSettings
    {
        /// <summary>
        /// API key for telemetry.
        /// </summary>
        string TelemetryApiKey { get; }

        /// <summary>
        /// License key for actipro controls.
        /// </summary>
        string ActiproLicenseKey { get; }

        /// <summary>
        /// Licensee for actipro controls.
        /// </summary>
        string ActiproLicensee { get; }
    }
}
