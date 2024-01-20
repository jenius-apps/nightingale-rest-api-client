using Nightingale.Core.Settings;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Nightingale.Core.Client.Ssl
{
    /// <summary>
    /// Class for validating SSL certificates.
    /// </summary>
    public class SslValidator : ISslValidator
    {
        private readonly IUserSettings _userSettings;

        public SslValidator(IUserSettings userSettings)
        {
            _userSettings = userSettings
                ?? throw new ArgumentNullException(nameof(userSettings));
        }

        /// <inheritdoc/>
        public bool Validate(
            HttpRequestMessage s,
            X509Certificate2 ce,
            X509Chain ch,
            SslPolicyErrors e)
        {
            bool validateSsl = _userSettings.GetAsync<bool>(SettingsConstants.SslValidationKey).GetAwaiter().GetResult();
            if (!validateSsl)
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, err) => true;
                return true;
            }
            else
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, err) => err == SslPolicyErrors.None;
                return e == SslPolicyErrors.None;
            }
        }

    }
}
