using MimeMapping;
using Newtonsoft.Json;
using Nightingale.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Nightingale.Core.Models
{
    public enum FormDataType
    {
        Text,
        File
    }
 
    public class FormData : ObservableBase, IStorageItem, IDeepCloneable
    {
        private FormDataType _formDataType;
        private IList<string> _filePaths;

        public FormData()
        {

        }

        public FormData(bool enabled)
        {
            Enabled = enabled;
        }

        public string Id { get; set; }

        public string ParentId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContentType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AutoContentType
        {
            get => _autoContentType;
            set
            {
                if (_autoContentType != value)
                {
                    _autoContentType = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string _autoContentType;

        public bool Enabled { get; set; }

        public FormDataType FormDataType
        {
            get => _formDataType;
            set
            {
                if (_formDataType != value)
                {
                    _formDataType = value;
                    RaisePropertyChanged("IsTextType");
                    RaisePropertyChanged("IsFileType");

                    if (value == FormDataType.Text)
                    {
                        AutoContentType = null;
                    }
                    else if (value == FormDataType.File && HasFiles)
                    {
                        AutoContentType = MimeUtility.GetMimeMapping(FilePaths[0]);
                    }
                }
            }
        }

        [JsonIgnore]
        public bool IsTextType => FormDataType == FormDataType.Text;

        [JsonIgnore]
        public bool IsFileType => FormDataType == FormDataType.File;

        public IList<string> FilePaths
        {
            get => _filePaths;
            set
            {
                _filePaths = value;
                RaisePropertyChanged("HasFiles");
                RaisePropertyChanged("HasNoFiles");
                RaisePropertyChanged("SelectedFiles");
            }
        }

        [JsonIgnore]
        public string SelectedFiles => HasFiles ? string.Join(", ", FilePaths) : "";

        [JsonIgnore]
        public bool HasFiles => FilePaths != null && FilePaths.Count > 0;

        [JsonIgnore]
        public bool HasNoFiles => !HasFiles;

        public object DeepClone()
        {
            return new FormData
            {
                ParentId = this.ParentId,
                Key = this.Key,
                Value = this.Value,
                ContentType = this.ContentType,
                Enabled = this.Enabled,
                FormDataType = this.FormDataType,
                FilePaths = this.FilePaths?.ToList()
            };
        }
    }
}
