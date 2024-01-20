using System.Threading;
using System.Threading.Tasks;

namespace Nightingale.Core.Services
{
    public interface ICloudBackupClient
    {
        Task UploadBackupAsync(string accessToken, CancellationToken ct);

        Task RestoreBackupAsync(string accessToken, CancellationToken ct);

        string[] GetScopes();
    }
}
