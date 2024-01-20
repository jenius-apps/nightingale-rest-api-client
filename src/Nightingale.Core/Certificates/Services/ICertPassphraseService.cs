using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Certificates.Services
{
    /// <summary>
    /// A service that accesses a secure storage 
    /// entity to get and set the passphrase
    /// for a <see cref="Certificates.Models.Certificate"/>.
    /// </summary>
    public interface ICertPassphraseService
    {
        string GetPassphrase(string certificateId);

        void SetPassphrase(string certificateId, string passphrase);

        void DeletePassphrase(string certificateId, string passphrase);
    }
}
