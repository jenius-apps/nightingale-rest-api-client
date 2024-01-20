using Nightingale.Core.Contracts.Models;
using Nightingale.Core.Models;
using System.Collections.ObjectModel;

namespace Nightingale.Core.Interfaces
{
    public interface IWorkspaceRequest : IStorageItem, IHydratable
    {
        int MethodIndex { get; set; }

        string Method { get; set; }

        string BaseUrl { get; set; }

        RequestBody Body { get; set; }

        Url Url { get; set; }

        ObservableCollection<Parameter> Queries { get; }

        ObservableCollection<Parameter> Headers { get; }

        ObservableCollection<Parameter> ChainingRules { get; }

        Authentication Authentication { get; set; }

        ObservableCollection<ApiTest> ApiTests { get; }

        WorkspaceResponse WorkspaceResponse { get; set; }

        int RequestPivotIndex { get; set; }

        int ResponsePivotIndex { get; set; }

        int Position { get; set; }
    }
}
