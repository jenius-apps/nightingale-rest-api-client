using Nightingale.Core.Interfaces;
using Nightingale.Core.Workspaces.Extensions;
using System.Collections.ObjectModel;

namespace Nightingale.Core.Models
{
    public class Environment : ObservableBase, IEnvironment, IDeepCloneable
    {
        private string _name;
        private bool _isActive;

        public Environment()
        {
        }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Icon { get; set; }

        public EnvType EnvironmentType { get; set; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Parameter> Variables { get; } = new ObservableCollection<Parameter>();

        public object DeepClone()
        {
            var other = new Environment
            {
                Name = this.Name,
                EnvironmentType = EnvType.Sub,
                IsActive = false,
            };
            other.Variables.DeepClone(this.Variables);
            return other;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public enum EnvType
    {
        Sub,
        Base
    }
}
