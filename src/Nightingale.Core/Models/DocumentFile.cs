using Newtonsoft.Json;
using Nightingale.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Nightingale.Core.Models
{
    public class DocumentFile
    {
        public string ActiveWorkspaceId { get; set; }

        private IList<Workspace> _workspaces;
        public IList<Workspace> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new List<Workspace>();
                }

                return _workspaces;
            }
            set => _workspaces = value;
        }

        private IList<WorkspaceRequest> _requests;
        public IList<WorkspaceRequest> Requests
        {
            get
            {
                if (_requests == null)
                {
                    _requests = new List<WorkspaceRequest>();
                }

                return _requests;
            }
            set => _requests = value;
        }

        private IList<HistoryRequest> _historyRequests;
        public IList<HistoryRequest> HistoryRequests
        {
            get
            {
                if (_historyRequests == null)
                {
                    _historyRequests = new List<HistoryRequest>();
                }

                return _historyRequests;
            }
            set => _historyRequests = value;
        }

        private IList<WorkspaceCollection> _collections;
        public IList<WorkspaceCollection> Collections
        {
            get
            {
                if (_collections == null)
                {
                    _collections = new List<WorkspaceCollection>();
                }

                return _collections;
            }
            set => _collections = value;
        }

        private IList<HistoryCollection> _historyCollections;
        public IList<HistoryCollection> HistoryCollections
        {
            get
            {
                if (_historyCollections == null)
                {
                    _historyCollections = new List<HistoryCollection>();
                }

                return _historyCollections;
            }
            set => _historyCollections = value;
        }

        private IList<RequestBody> _requestBodies;
        public IList<RequestBody> RequestBodies
        {
            get
            {
                if (_requestBodies == null)
                {
                    _requestBodies = new List<RequestBody>();
                }

                return _requestBodies;
            }
            set => _requestBodies = value;
        }

        private IList<Environment> _environments;
        public IList<Environment> Environments
        {
            get
            {
                if (_environments == null)
                {
                    _environments = new List<Environment>();
                }

                return _environments;
            }
            set => _environments = value;
        }

        private IList<ApiTest> _apiTests;
        public IList<ApiTest> ApiTests
        {
            get
            {
                if (_apiTests == null)
                {
                    _apiTests = new List<ApiTest>();
                }

                return _apiTests;
            }
            set => _apiTests = value;
        }

        private IList<Authentication> _authentications;
        public IList<Authentication> Authentications
        {
            get
            {
                if (_authentications == null)
                {
                    _authentications = new List<Authentication>();
                }

                return _authentications;
            }
            set => _authentications = value;
        }

        private IList<Parameter> _parameters;
        public IList<Parameter> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new List<Parameter>();
                }

                return _parameters;
            }
            set => _parameters = value;
        }

        private IList<RecentUrl> _recentUrls;
        public IList<RecentUrl> RecentUrls
        {
            get
            {
                if (_recentUrls == null)
                {
                    _recentUrls = new List<RecentUrl>();
                }

                return _recentUrls;
            }
            set => _recentUrls = value;
        }

        public IList<T> GetList<T>() where T : IStorageItem
        {
            IList<T> list;

            if (typeof(T) == typeof(Workspace))
            {
                list = Workspaces as IList<T>;
            }
            else if (typeof(T) == typeof(WorkspaceRequest))
            {
                list = Requests as IList<T>;
            }
            else if (typeof(T) == typeof(HistoryRequest))
            {
                list = HistoryRequests as IList<T>;
            }
            else if (typeof(T) == typeof(WorkspaceCollection))
            {
                list = Collections as IList<T>;
            }
            else if (typeof(T) == typeof(HistoryCollection))
            {
                list = HistoryCollections as IList<T>;
            }
            else if (typeof(T) == typeof(RequestBody))
            {
                list = RequestBodies as IList<T>;
            }
            else if (typeof(T) == typeof(Environment))
            {
                list = Environments as IList<T>;
            }
            else if (typeof(T) == typeof(ApiTest))
            {
                list = ApiTests as IList<T>;
            }
            else if (typeof(T) == typeof(Authentication))
            {
                list = Authentications as IList<T>;
            }
            else if (typeof(T) == typeof(Parameter))
            {
                list = Parameters as IList<T>;
            }
            else if (typeof(T) == typeof(RecentUrl))
            {
                list = RecentUrls as IList<T>;
            }
            else
            {
                throw new NotSupportedException("This type is not supported: " + typeof(T));
            }

            return list;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this).ToString();
        }
    }
}
