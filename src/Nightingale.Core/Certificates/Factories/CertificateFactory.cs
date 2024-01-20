using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Nightingale.Core.Certificates.Models;
using Nightingale.Core.Certificates.Services;

namespace Nightingale.Core.Certificates.Factories
{
    /// <summary>
    /// Factory for creating <see cref="Certificate"/>
    /// instances. This is necessary in order to
    /// inject <see cref="ICertPassphraseService"/>
    /// into each certificate.
    /// </summary>
    public class CertificateFactory : ICertificateFactory
    {
        private readonly ICertPassphraseService _certPassphraseService;

        /// <summary>
        /// Initializes instance of <see cref="CertificateFactory"/>.
        /// </summary>
        /// <param name="certPassphraseService">The <see cref="ICertPassphraseService"/> to be injected into certificates.</param>
        public CertificateFactory(ICertPassphraseService certPassphraseService)
        {
            _certPassphraseService = certPassphraseService ?? throw new ArgumentNullException(nameof(certPassphraseService));
        }

        /// <summary>
        /// Generates new certificates.
        /// </summary>
        /// <param name="domain">Domain of certificate.</param>
        /// <param name="port">Port number of domain of certificate.</param>
        /// <param name="pfxFilePath">Path to pfx file.</param>
        /// <param name="passphrase">Passphrase for pfx file.</param>
        /// <returns>Returns a new <see cref="Certificate"/>.</returns>
        public Certificate CreateCert(string domain, string port, string pfxFilePath, string passphrase)
        {
            try
            {
                var cert = new Certificate(_certPassphraseService)
                {
                    Domain = domain,
                    Port = port,
                    PfxFilePath = pfxFilePath,
                    Id = Guid.NewGuid().ToString(),
                    Passphrase = passphrase
                };
                return cert;
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public IList<Certificate> DeserializeList(string deserializedCertList)
        {
            if (string.IsNullOrWhiteSpace(deserializedCertList))
            {
                return new List<Certificate>();
            }

            var list = new List<Certificate>();
            JArray array = JArray.Parse(deserializedCertList);

            foreach (var x in array)
            {
                var cert = new Certificate(_certPassphraseService)
                {
                    Domain = x["Domain"].ToString(),
                    Port = x["Port"].ToString(),
                    PfxFilePath = x["PfxFilePath"].ToString(),
                    Id = x["Id"].ToString(),
                    ExpirationDateString = x["ExpirationDateString"].ToString()
                };

                list.Add(cert);
            }

            return list;
        }
    }
}
