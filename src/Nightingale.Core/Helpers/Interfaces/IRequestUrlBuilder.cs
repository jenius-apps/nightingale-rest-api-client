using Nightingale.Core.Workspaces.Models;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IRequestUrlBuilder
    {
        string GetPreviewUrl(Item request);
    }
}
