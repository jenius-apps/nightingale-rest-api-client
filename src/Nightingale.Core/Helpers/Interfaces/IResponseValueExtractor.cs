using Nightingale.Core.Models;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IResponseValueExtractor
    {
        string Extract(WorkspaceResponse response, string propertyExpression);
    }
}
