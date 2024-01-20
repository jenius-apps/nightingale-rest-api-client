using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nightingale.Core.Client.Builders
{
    /// <summary>
    /// Interface for configuring the form data content
    /// of an http message.
    /// </summary>
    public interface IFormDataBuilder
    {
        /// <summary>
        /// Adds the form data from the <see cref="RequestBody"/>
        /// into the <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="HttpRequestMessage"/> to edit.</param>
        /// <param name="body">The source of the form data.</param>
        /// <param name="resolver">An instance of <see cref="IVariableResolver"/> to resolve variables.</param>
        /// <param name="logger">An optional instance of <see cref="ILogger"/>.</param>
        Task AddFormDataAsync(
            HttpRequestMessage message,
            RequestBody body,
            IVariableResolver resolver,
            ILogger logger = null);
    }
}