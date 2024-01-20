using Nightingale.Core.Certificates.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Core.Certificates.Services
{
    /// <summary>
    /// Interface for retrieving certificates based
    /// on given parameters.
    /// </summary>
    public interface ICertRetriever
    {
        /// <summary>
        /// Retrieves certificate associated with
        /// given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">A valid <see cref="Uri"/>.</param>
        /// <returns>The <see cref="Certificate"/> associated to the <see cref="Uri"/>.</returns>
        Task<Certificate> GetCachedCertificate(Uri uri);
    }
}
