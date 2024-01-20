using Newtonsoft.Json;
using Nightingale.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nightingale.Core.Models
{
    public enum RequestBodyType
    {
        None,
        Json,
        Xml,
        FormEncoded,
        Binary,
        FormData,
        Text
    }

    public class RequestBody : ModifiableBase, IStorageItem, IDeepCloneable
    {
        private RequestBodyType _bodyType;
        private string _jsonBody;
        private string _xmlBody;
        private string _binaryFilePath;
        private string _textBody;

        public RequestBody()
        {
        }

        public RequestBody(bool isNew)
        {
            if (isNew)
            {
                Status = ModifiedStatus.New;
            }
        }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public RequestBodyType BodyType
        {
            get => _bodyType;
            set
            {
                if (_bodyType != value)
                {
                    _bodyType = value;
                    ObjectModified();
                }
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JsonBody
        {
            get => _jsonBody;
            set
            {
                if (_jsonBody != value)
                {
                    _jsonBody = value;
                    ObjectModified();
                }
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string XmlBody
        {
            get => _xmlBody;
            set
            {
                if (_xmlBody != value)
                {
                    _xmlBody = value;
                    ObjectModified();
                }
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TextBody
        {
            get => _textBody;
            set
            {
                if (_textBody != value)
                {
                    _textBody = value;
                    ObjectModified();
                }
            }
        }

        public ObservableCollection<Parameter> FormEncodedData { get; } = new ObservableCollection<Parameter>();

        public ObservableCollection<FormData> FormDataList { get; } = new ObservableCollection<FormData>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BinaryFilePath
        {
            get => _binaryFilePath;
            set
            {
                if (_binaryFilePath != value)
                {
                    _binaryFilePath = value;
                    ObjectModified();
                }
            }
        }

        public object DeepClone()
        {
            var result = new RequestBody()
            {
                BodyType = this.BodyType,
                JsonBody = this.JsonBody,
                XmlBody = this.XmlBody,
                BinaryFilePath = this.BinaryFilePath,
                TextBody = this.TextBody,
                Status = ModifiedStatus.New
            };

            foreach (FormData formData in this.FormDataList)
            {
                result.FormDataList.Add(formData.DeepClone() as FormData);
            }

            foreach (Parameter f in this.FormEncodedData)
            {
                result.FormEncodedData.Add(f.DeepClone() as Parameter);
            }

            return result;
        }
    }
}
