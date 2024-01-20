using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.Connectivity;
using Nightingale.Core.Extensions;
using Nightingale.Core.Helpers;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Settings;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Extensions;
using Nightingale.CustomEventArgs;
using Nightingale.Handlers;
using Nightingale.Utilities;
using Nightingale.VisualState;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Nightingale.Mock;
using Windows.System;
using JeniusApps.Nightingale.Converters.Curl;

namespace Nightingale.ViewModels
{
    public class RequestControlViewModel : ViewModelBase
    {
        public event EventHandler ResponseReceived;

        private readonly IRequestSender _requestSender;
        private readonly IResponseFileWriter _fileWriter;
        private readonly IResponseValueExtractor _responseValueExtractor;
        public readonly IEnvironmentContainer EnvironmentContainer;
        private readonly IVariableResolver _variableResolver;
        private readonly IVisualStatePublisher _visualStatePublisher;
        private readonly IMessageBus _messageBus;
        private readonly ICurlConverter _curlConverter;
        private readonly ResourceLoader _resourceLoader;

        private Item _request;
        private CancellationTokenSource _cts;
        private bool _loading;
        private bool _areTestsRunning;
        private bool _noRequestSentYet = true;

        public RequestControlViewModel(
            IRequestSender sender,
            IResponseFileWriter fileWriter,
            IResponseValueExtractor responseValueExtractor,
            IEnvironmentContainer environmentContainer,
            IVariableResolver variableResolver,
            IVisualStatePublisher visualStatePublisher,
            IMessageBus messageBus,
            ICurlConverter curlConverter)
        {
            _cts = new CancellationTokenSource();
            _requestSender = sender ?? throw new ArgumentNullException(nameof(sender));
            _curlConverter = curlConverter ?? throw new ArgumentNullException(nameof(curlConverter));
            _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
            _responseValueExtractor = responseValueExtractor ?? throw new ArgumentNullException(nameof(responseValueExtractor));
            EnvironmentContainer = environmentContainer ?? throw new ArgumentNullException(nameof(environmentContainer));
            _variableResolver = variableResolver ?? throw new ArgumentNullException(nameof(variableResolver));
            _visualStatePublisher = visualStatePublisher ?? throw new ArgumentNullException(nameof(visualStatePublisher));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _resourceLoader = ResourceLoader.GetForCurrentView();

            _visualStatePublisher.PaneLayoutToggled += VisualStatePublisher_PaneLayoutToggled;
        }

        public MockDataViewModel MockDataViewModel { get; set; }

        public Item Request
        {
            get => _request;
            set
            {
                if (_request != null)
                {
                    _request.Auth.TypeChanged -= Auth_TypeChanged;
                }
                _request = value;

                if (_request != null)
                {
                    _request.Auth.TypeChanged += Auth_TypeChanged;
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        private void Auth_TypeChanged(object sender, EventArgs e)
        {
            UpdateAuthHeaderErrorMessage();
        }

        public bool ShowAuthWarning
        {
            get => _showAuthWarning;
            set
            {
                if (_showAuthWarning != value)
                {
                    _showAuthWarning = value;
                    RaisePropertyChanged();
                }
            }
        }
        private bool _showAuthWarning;

        public bool PaneLayoutSideBySide
        {
            get => _visualStatePublisher.IsLayoutTwoPane();
        }

        private void VisualStatePublisher_PaneLayoutToggled(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(PaneLayoutSideBySide));
        }

        public string HeaderCount
        {
            get => Request?.Headers == null ? "" : Request.Headers.GetDisplayCount();
        }

        public string ChainCount
        {
            get => Request?.ChainingRules == null ? "" : Request.ChainingRules.GetDisplayCount();
        }

        public string QueryCount
        {
            get => Request?.Url?.Queries == null ? "" : Request?.Url?.Queries.GetDisplayCount();
        }

        public WorkspaceResponse Response
        {
            get => Request?.Response;
            set
            {
                if (Request != null)
                {
                    Request.Response = value;
                    RaisePropertyChanged("Response");
                    RaisePropertyChanged("ResponseHeaders");
                    RaisePropertyChanged("ResponseCookies");
                    RaisePropertyChanged("ApiTestResults");
                    RaisePropertyChanged("ResponseLog");
                    RaisePropertyChanged("ResponseHeadersCount");
                    RaisePropertyChanged("ResponseCookiesCount");
                    RaisePropertyChanged("ApiTestResultsCount");
                    RaisePropertyChanged(nameof(IsErrorResponse));
                    RaisePropertyChanged(nameof(ShowResponsePivot));
                    IsStackTraceVisible = false; // reset to hidden
                    ResponseReceived?.Invoke(this, new EventArgs());
                }
            }
        }

        public bool IsErrorResponse => Response?.StatusDescription == "Error";

        public bool ShowResponsePivot => !IsErrorResponse;

        public bool IsStackTraceVisible
        {
            get => _isStackTraceVisible;
            set
            {
                _isStackTraceVisible = value;
                RaisePropertyChanged();
            }
        }
        private bool _isStackTraceVisible;

        public bool NoRequestSentYet
        {
            get => _noRequestSentYet && Response == null;
            set
            {
                _noRequestSentYet = value;
                RaisePropertyChanged("NoRequestSentYet");
            }
        }

        public string ResponseHeadersCount
        {
            get => Response?.Headers == null ? "" : (Response?.Headers.Count == 0 ? "" : Response?.Headers.Count.ToString());
        }

        public string ResponseCookiesCount
        {
            get => Response?.Cookies == null ? "" : (Response?.Cookies.Count == 0 ? "" : Response?.Cookies.Count.ToString());
        }

        public bool AreTestsRunning
        {
            get => _areTestsRunning;
            set
            {
                if (_areTestsRunning != value)
                {
                    _areTestsRunning = value;
                    RaisePropertyChanged("AreTestsRunning");
                }
            }
        }

        public string BaseUrl
        {
            get => Request?.Url?.ToString() ?? "";
            set
            {
                if (Request == null)
                {
                    return;
                }

                Request.Url.Set(value);
                RaisePropertyChanged("BaseUrl");
            }
        }

        public string Method
        {
            get => Request?.Method;
            set
            {
                if (Request == null)
                {
                    return;
                }

                Request.Method = value;
                RaisePropertyChanged("Method");
            }
        }

        public bool Loading
        {
            get => _loading;
            set
            {
                _loading = value;
                RaisePropertyChanged("Loading");
            }
        }

        public RequestBody Body
        {
            get => Request?.Body;
        }

        public Authentication Authentication
        {
            get => Request?.Auth;
        }

        public ObservableCollection<Parameter> ChainingRules
        {
            get => Request?.ChainingRules;
        }

        public int RequestPivotIndex
        {
            get => (int)(Request?.GetProperty<long>(ItemPropertyExtensions.RequestPivotIndex) ?? 0);
            set
            {
                if (Request?.Properties != null && RequestPivotIndex != value)
                {
                    Request.SetProperty(ItemPropertyExtensions.RequestPivotIndex, (long)value);
                    RaisePropertyChanged("RequestPivotIndex");
                }
            }
        }

        public int ResponsePivotIndex
        {
            get => (int)(Request?.GetProperty<long>(ItemPropertyExtensions.ResponsePivotIndex) ?? 0);
            set
            {
                if (Request?.Properties != null && ResponsePivotIndex != value)
                {
                    Request.SetProperty(ItemPropertyExtensions.ResponsePivotIndex, (long)value);
                    RaisePropertyChanged("ResponsePivotIndex");
                }
            }
        }

        public IList<KeyValuePair<string, string>> ResponseHeaders
        {
            get
            {
                if (Response?.Headers == null || Response.Headers.Count == 0)
                {
                    return new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(
                             _resourceLoader.GetString("Headers/Text"),
                            _resourceLoader.GetString("NoneFound"))
                    };
                }
                else
                {
                    return Response.Headers;
                }
            }
        }

        public IList<KeyValuePair<string, string>> ResponseCookies
        {
            get
            {
                if (Response?.Cookies == null || Response.Cookies.Count == 0)
                {
                    return new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(
                             _resourceLoader.GetString("Cookies/Text"), 
                            _resourceLoader.GetString("NoneFound"))
                    };
                }
                else
                {
                    return Response.Cookies;
                }
            }
        }

        public string ResponseLog
        {
            get => Response?.Log;
            set
            {
                if (Response == null)
                {
                    return;
                }

                Response.Log = value;
                RaisePropertyChanged("ResponseLog");
            }
        }

        public void PasteCurl(string curlString)
        {
            var dtoItem = _curlConverter.Convert(curlString);
            if (dtoItem == null)
            {
                return;
            }

            BaseUrl = dtoItem.Url.Base;
            Method = dtoItem.Method;
            Request.Headers.Clear();
            foreach (var h in dtoItem.Headers)
            {
                Request.Headers.Add(new Parameter(h.Enabled, h.Key, h.Value, ParamType.Header));
            }

            Body.BodyType = (RequestBodyType)(int)dtoItem.Body.BodyType;
            Body.JsonBody = dtoItem.Body.JsonBody;
            Body.TextBody = dtoItem.Body.TextBody;
            Body.XmlBody = dtoItem.Body.XmlBody;

            ClearResponse();
            Analytics.TrackEvent(Telemetry.CurlPaste);
        }

        public void QueryValuesUpdated()
        {
            RaisePropertyChanged("BaseUrl");
            RaisePropertyChanged("QueryCount");
        }

        public void HeaderValuesUpdated()
        {
            RaisePropertyChanged(nameof(HeaderCount));
            UpdateAuthHeaderErrorMessage();
        }

        private void UpdateAuthHeaderErrorMessage()
        {
            if (Request?.Headers != null 
                && Request.Headers.Any(x => x.Key.Trim().Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                && Request.Auth.AuthType != AuthType.None)
            {
                ShowAuthWarning = true;
            }
            else
            {
                ShowAuthWarning = false;
            }
        }

        public void ChainValuesUpdated() => RaisePropertyChanged("ChainCount");

        public async void SendRequestNoDownload()
        {
            if (Loading)
            {
                return;
            }

            await SendRequest(false);
        }

        public async void SendRequestAndDownload()
        {
            if (Loading)
            {
                return;
            }

            await SendRequest(true);
            Analytics.TrackEvent("Send and download button clicked");
        }

        private async Task SendRequest(bool downloadResponse)
        {
            if (Request == null || Loading)
            {
                return;
            }

            Loading = true;
            NoRequestSentYet = false;

            // Allow loading animation to begin
            await Task.Delay(1);

            string baseUrl = _variableResolver.ResolveVariable(BaseUrl);
            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable 
                && !baseUrl.Contains("localhost") 
                && !baseUrl.Contains("127.0.0.1"))
            {
                var dialog = new Windows.UI.Xaml.Controls.ContentDialog()
                {
                    Title = _resourceLoader.GetString("ErrorTitle"),
                    Content = _resourceLoader.GetString("InternetUnavailable"),
                    CloseButtonText = _resourceLoader.GetString("Okay")
                };
                await dialog.ShowAsync();
                Loading = false;
                return;
            }

            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://"))
            {
                BaseUrl = $"http://{BaseUrl}";
            }

            if (baseUrl.Contains("localhost") || baseUrl.Contains("127.0.0.1"))
            {
                // trigger banner message
                _messageBus.Publish(new Message("localhost"));
            }
            else
            {
                // resets banner message
                _messageBus.Publish(new Message(null));
            }

            WorkspaceResponse response = await _requestSender.SendRequestAsync(
                Request,
                _cts.Token,
                UserSettings.Get<bool>(SettingsConstants.HistoryEnabled));

            if (Request is HistoryItem)
            {
                Analytics.TrackEvent("History request sent");
            }
            else
            {
                Analytics.TrackEvent("Request sent");
            }

            if (downloadResponse)
            {
                await _fileWriter.WriteFileAsync(response);
            }

            Response = response;

            Loading = false;

            ExecuteChainingRules();
        }

        public void MethodChanged(object sender, AddedItemArgs<string> args)
        {
            if (args == null || args.AddedItem == "CUSTOM")
            {
                // Re-assign the previous method
                this.Method = this.Method;
                return;
            }

            this.Method = args.AddedItem;
        }

        public void ExecuteChainingRules()
        {
            if (ChainingRules == null || ChainingRules.Count == 0)
            {
                return;
            }

            for (int i = 0; i < ChainingRules.Count; i++)
            {
                Parameter rule = ChainingRules[i];

                if (!rule.Enabled || string.IsNullOrWhiteSpace(rule.Key) || string.IsNullOrWhiteSpace(rule.Value))
                {
                    continue;
                }

                try
                {
                    string newValue = _responseValueExtractor.Extract(this.Response, rule.Value);
                    EnvironmentContainer.UpdateVariableValue(rule.Key, newValue, shallow: false);
                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Chaing rule extraction failed", new Dictionary<string, string>
                    {
                        { "error", e.Message },
                        { "chain rule property path", rule.Value }
                    });
                    ResponseLog += $"> Could not execute chain rule for path {rule.Value}. Error: {e.Message}.";
                }
            }

            int chainRulesExecuted = ChainingRules
                .Where(x => x.Enabled
                    && !string.IsNullOrWhiteSpace(x.Key)
                    && !string.IsNullOrWhiteSpace(x.Value))
                .Count();

            if (chainRulesExecuted > 0)
            {
                Analytics.TrackEvent("Chain rules executed", new Dictionary<string, string>
                {
                    { "count", chainRulesExecuted.ToString() }
                });
            }
        }

        public void ClearResponse()
        {
            Response = null;
            Analytics.TrackEvent("Response cleared");
        }

        public void Cancel()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }
        }

        public void CopyResponseHeaders()
        {
            CopyList(Response?.Headers);
            Analytics.TrackEvent("Headers copied");
        }

        public void CopyResponseCookies()
        {
            CopyList(Response?.Cookies);
            Analytics.TrackEvent("Cookies copied");
        }

        private void CopyList(IList<KeyValuePair<string,string>> list)
        {
            if (list == null || list.Count == 0)
            {
                Common.Copy("");
                return;
            }

            var result = string.Join(
                System.Environment.NewLine,
                list.Select(x => $"{x.Key}: {x.Value}"));

            Common.Copy(result);
        }

        public async void Troubleshoot()
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri("https://github.com/jenius-apps/nightingale-rest-api-client/blob/master/docs/tsg.md"));
            }
            catch
            {
            }
        }

        public void ToggleStackTrace()
        {
            IsStackTraceVisible = !IsStackTraceVisible;
        }
    }
}
