using Nightingale.Core.Auth;
using Nightingale.Core.Models;
using System.Threading.Tasks;

namespace Nightingale.Core.Interfaces
{
    public interface IOAuth2TokenRetriever
    {
        Task<Token> GetOAuth2Token(Authentication oauth2Data, ILogger logger, bool force = true);
    }
}
