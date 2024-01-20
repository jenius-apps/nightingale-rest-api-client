using Nightingale.Core.Certificates.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Certificates.Factories
{
    /// <summary>
    /// Factory for creating <see cref="Certificate"/>
    /// instances. This is necessary in order to
    /// inject <see cref="ICertPassphraseService"/>
    /// into each certificate.
    /// </summary>
    public interface ICertificateFactory
    {
        /// <summary>
        /// Generates new certificates.
        /// </summary>
        /// <param name="domain">Domain of certificate.</param>
        /// <param name="port">Port number of domain of certificate.</param>
        /// <param name="pfxFilePath">Path to pfx file.</param>
        /// <param name="passphrase">Passphrase for pfx file.</param>
        /// <returns>Returns a new <see cref="Certificate"/>.</returns>
        Certificate CreateCert(string domain, string port, string pfxFilePath, string passphrase);

        /// <summary>
        /// Deserializes list of <see cref="Certificate"/>.
        /// </summary>
        /// <param name="deserializedCertList">A serialized list of <see cref="Certificate"/>.</param>
        /// <returns>Returns list of <see cref="Certificate"/>.</returns>
        IList<Certificate> DeserializeList(string deserializedCertList);
    }
}
