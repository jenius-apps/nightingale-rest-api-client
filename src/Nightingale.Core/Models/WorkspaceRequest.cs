using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Nightingale.Core.Interfaces;

namespace Nightingale.Core.Models
{
    public class WorkspaceRequest : WorkspaceItem, IWorkspaceRequest
    {
        private string _name;
        private int _methodIndex;
        private string _baseUrl = string.Empty;
        private WorkspaceResponse _response;
        private string _method;

        public WorkspaceRequest()
        {
        }

        /// <summary>
        /// The selected HTTP method of
        /// the request.
        /// </summary>
        public string Method
        {
            get => _method;
            set
            {
                _method = value;
                RaisePropertyChanged("Method");
            }
        }

        public int MethodIndex
        {
            get => _methodIndex;
            set
            {
                if (_methodIndex != value)
                {
                    _methodIndex = value;
                    RaisePropertyChanged("MethodIndex");
                }
            }
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

        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                if (_baseUrl != value)
                {
                    _baseUrl = value ?? string.Empty;
                    RaisePropertyChanged("BaseUrl");
                }
            }
        }

        /// <summary>
        /// The URL of the request.
        /// </summary>
        public Url Url { get; set; } = new Url();

        [JsonIgnore]
        public RequestBody Body { get; set; } = new RequestBody(isNew: true);

        [JsonIgnore]
        public WorkspaceResponse WorkspaceResponse
        {
            get => _response;
            set
            {
                _response = value;
                RaisePropertyChanged(string.Empty);
            }
        }

        [JsonIgnore]
        public ObservableCollection<Parameter> Queries { get; } = new ObservableCollection<Parameter>();

        [JsonIgnore]
        public ObservableCollection<Parameter> Headers { get; } = new ObservableCollection<Parameter>();

        [JsonIgnore]
        public ObservableCollection<ApiTest> ApiTests { get; } = new ObservableCollection<ApiTest>();

        [JsonIgnore]
        public ObservableCollection<Parameter> ChainingRules { get; } = new ObservableCollection<Parameter>();

        [JsonIgnore]
        public int RequestPivotIndex { get; set; }

        [JsonIgnore]
        public int ResponsePivotIndex { get; set; }

        [JsonIgnore]
        public bool IsHydrated { get; set; }

        public override object DeepClone()
        {
            var request = new WorkspaceRequest()
            {
                Name = this.Name,
                Url = new Url
                {
                    Base = this.Url.Base
                },
                Method = this.Method,
                ParentId = this.ParentId,
                Body = this.Body.DeepClone() as RequestBody,
                Authentication = this.Authentication.DeepClone() as Authentication,
                Status = ModifiedStatus.New
            };

            foreach (Parameter p in this.Url.Queries)
            {
                request.Url.Queries.Add(p.DeepClone() as Parameter);
            }

            foreach (Parameter h in this.Headers)
            {
                request.Headers.Add(h.DeepClone() as Parameter);
            }

            foreach (ApiTest t in this.ApiTests)
            {
                request.ApiTests.Add(t.DeepClone() as ApiTest);
            }

            foreach(Parameter chainingRule in ChainingRules)
            {
                request.ChainingRules.Add(chainingRule.DeepClone() as Parameter);
            }

            return request;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
