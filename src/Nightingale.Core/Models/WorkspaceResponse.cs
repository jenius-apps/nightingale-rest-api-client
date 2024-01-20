using Newtonsoft.Json;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Workspaces.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Nightingale.Core.Models
{
    public class WorkspaceResponse : ObservableBase, IWorkspaceResponse, IDeepCloneable
    {
        private bool? _testsAllPass;

        public bool Successful { get; set; }

        public string RequestBaseUrl { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public long TimeElapsed { get; set; }

        [JsonIgnore]
        public byte[] RawBytes { get; set; }

        public bool? TestsAllPass
        {
            get => _testsAllPass;
            set
            {
                if (_testsAllPass == value)
                {
                    return;
                }

                _testsAllPass = value;
                RaisePropertyChanged("TestsAllPass");
            }
        }

        public string Body { get; set; }

        public string ContentType { get; set; }

        public IList<KeyValuePair<string, string>> Cookies { get; } = new List<KeyValuePair<string, string>>();

        public IList<KeyValuePair<string, string>> Headers { get; } = new List<KeyValuePair<string, string>>();

        public string Log { get; set; }

        public object DeepClone()
        {
            var other = new WorkspaceResponse
            {
                Log = this.Log,
                ContentType = this.ContentType,
                Body = this.Body,
                RawBytes = this.RawBytes?.ToArray(),
                StatusDescription = this.StatusDescription,
                RequestBaseUrl = this.RequestBaseUrl,
                StatusCode = this.StatusCode,
                Successful = this.Successful,
                TestsAllPass = this.TestsAllPass,
                TimeElapsed = this.TimeElapsed
            };
            other.Cookies.DeepCloneStructs(this.Cookies);
            other.Headers.DeepCloneStructs(this.Headers);
            return other;
        }
    }
}
