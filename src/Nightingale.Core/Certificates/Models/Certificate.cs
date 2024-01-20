using Newtonsoft.Json;
using Nightingale.Core.Certificates.Services;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nightingale.Core.Certificates.Models
{
    /// <summary>
    /// Class representing a certificate entry
    /// in Nightingale.
    /// </summary>
    public class Certificate : ObservableBase
    {
        private readonly ICertPassphraseService _passphraseService;
        private bool _passphraseVisible;

        /// <summary>
        /// Initializes a <see cref="Certificate"/> instance.
        /// </summary>
        /// <param name="passphraseService">The <see cref="ICertPassphraseService"/> instance to use for handling the passphrase.</param>
        public Certificate(ICertPassphraseService passphraseService)
        {
            _passphraseService = passphraseService ?? throw new ArgumentNullException(nameof(passphraseService));
        }

        /// <summary>
        /// Gets or sets the Id of this certificate.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the domain associated
        /// with the certificate.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets expiration date for certificate.
        /// </summary>
        public string ExpirationDateString { get; set; }

        /// <summary>
        /// Gets or sets the port number associated
        /// with the host for this certificate.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Gets the hostname associated with this certificate.
        /// </summary>
        [JsonIgnore]
        public string Host => $"https://{Domain ?? ""}{(string.IsNullOrWhiteSpace(Port) ? "" : ":" + Port)}";

        /// <summary>
        /// Gets or sets the PFX file path.
        /// </summary>
        public string PfxFilePath { get; set; }

        /// <summary>
        /// Gets the protected version of passphrase.
        /// </summary>
        [JsonIgnore]
        public string PassphraseProtected
        {
            get
            {
                int length = Passphrase?.Trim().Length ?? 0;
                return length > 0 ? string.Concat(Enumerable.Repeat("•", length)) : "";
            }
        }

        /// <summary>
        /// Gets or sets the passphrase for this certificate from
        /// the credential locker.
        /// </summary>
        [JsonIgnore]
        public string Passphrase
        {
            get => _passphraseService.GetPassphrase(this.Id);
            set => _passphraseService.SetPassphrase(this.Id, value);
        }

        /// <summary>
        /// Gets or sets the visibility for passphrase.
        /// </summary>
        [JsonIgnore]
        public bool PassphraseVisible
        {
            get => _passphraseVisible;
            set
            {
                _passphraseVisible = value;
                RaisePropertyChanged("PassphraseVisible");
                RaisePropertyChanged("PassphraseNotVisible");
            }
        }

        /// <summary>
        /// Gets or sets cached pfx file path.
        /// </summary>
        [JsonIgnore]
        public string CachedPfxFilePath { get; set; }

        /// <summary>
        /// Gets or sets the visibility for passphrase.
        /// </summary>
        [JsonIgnore]
        public bool PassphraseNotVisible => !PassphraseVisible;

        /// <summary>
        /// Toggles the visibility of passphrase.
        /// </summary>
        public void TogglePassphraseVisibility() => PassphraseVisible = !PassphraseVisible;

        public void Dispose()
        {
            _passphraseService.DeletePassphrase(this.Id, this.Passphrase);
        }
    }
}
