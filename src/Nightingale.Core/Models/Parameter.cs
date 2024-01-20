using Nightingale.Core.Interfaces;

namespace Nightingale.Core.Models
{
    public class Parameter : ObservableBase, IParameter, IStorageItem, IDeepCloneable
    {
        private string _key;
        private string _value;
        private bool _enabled;

        public Parameter()
        {
        }

        public Parameter(bool isEnabled = true, string key = "", string value = "", ParamType type = ParamType.Parameter)
        {
            Enabled = isEnabled;
            Key = key;
            Value = value;
            Type = type;
            Status = ModifiedStatus.New;
        }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Key
        {
            get => _key;
            set
            {
                if (_key != value)
                {
                    _key = value;
                    RaisePropertyChanged("Key");
                }
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    RaisePropertyChanged("Enabled");
                }
            }
        }

        public bool Private
        {
            get => _private;
            set
            {
                if (_private != value)
                {
                    _private = value;
                    RaisePropertyChanged();
                }
            }
        }
        private bool _private;

        public ParamType Type { get; set; }

        public object DeepClone()
        {
            var result = new Parameter
            {
                Key = this.Key,
                Value = this.Value,
                Enabled = this.Enabled,
                Type = this.Type,
                Private = this.Private,
                Status = ModifiedStatus.New
            };

            return result;
        }

        public override string ToString()
        {
            return $"{this.Key}: {this.Value}";
        }
    }

    public enum ParamType
    {
        Parameter,
        Header,
        FormEncodedData,
        EnvVariable,
        ChainingRule
    }
}
