using Nightingale.Core.Certificates.Models;
using System.Collections.Generic;

namespace Nightingale.Core.Certificates.Services
{
    /// <summary>
    /// An interface for storing certificates.
    /// </summary>
    public interface ICertStorageService
    {
        /// <summary>
        /// Retrieves list of certificates from storage.
        /// </summary>
        /// <returns>Returns list of <see cref="Certificate"/>.</returns>
        IList<Certificate> GetCertificates();

        /// <summary>
        /// Updates list of certificates in storage.
        /// </summary>
        /// <param name="certificates">Updated list of <see cref="Certificate"/>.</param>
        void SetCertificates(IList<Certificate> certificates);
    }
}
