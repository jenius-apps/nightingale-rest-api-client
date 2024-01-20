using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Nightingale.Core.Client.Ssl
{
    /// <summary>
    /// Interface for validating SSL certificates.
    /// </summary>
    public interface ISslValidator
    {
        /// <summary>
        /// Modifies SSL validation policy
        /// based on user settings.
        /// </summary>
        bool Validate(
            HttpRequestMessage s,
            X509Certificate2 ce,
            X509Chain ch,
            SslPolicyErrors e);
    }
}
