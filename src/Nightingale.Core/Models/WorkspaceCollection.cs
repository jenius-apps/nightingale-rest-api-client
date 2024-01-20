using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models.Interfaces;

namespace Nightingale.Core.Models
{
    public class WorkspaceCollection : WorkspaceItem, IWorkspaceCollection
    {
        private string _name;
        private bool _isExpanded = true;

        public WorkspaceCollection()
        {
        }

        public override string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    RaisePropertyChanged("IsExpanded");
                }
            }
        }

        [JsonIgnore]
        public ObservableCollection<WorkspaceItem> Children { get; } = new ObservableCollection<WorkspaceItem>();

        [JsonIgnore]
        public long TotalElapsedMilliseconds { get; set; }

        [JsonIgnore]
        public int TotalFailed { get; set; }

        [JsonIgnore]
        public int TotalPassed { get; set; }

        [JsonIgnore]
        public int PivotIndex { get; set; }

        public override object DeepClone()
        {
            var result = new WorkspaceCollection()
            {
                Name = this.Name,
                IsExpanded = this.IsExpanded,
                Status = ModifiedStatus.New,
                ParentId = this.ParentId,
                Authentication = this.Authentication.DeepClone() as Authentication
            };

            foreach (WorkspaceItem child in Children)
            {
                result.Children.Add(child.DeepClone() as WorkspaceItem);
            }

            return result;
        }
    }
}
