using Nightingale.Core.Client;
using Nightingale.Core.Cookies;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nightingale.Core.Services
{
    public class RequestSender : IRequestSender
    {
        private readonly INightingaleClient _nightingaleClient;
        private readonly IHistoryListModifier _historyList;
        private readonly ICookieJar _cookieJar;

        public RequestSender(
            INightingaleClient nightingaleClient,
            IHistoryListModifier historyList,
            ICookieJar cookieJar)
        {
            _nightingaleClient = nightingaleClient ?? throw new ArgumentNullException(nameof(nightingaleClient));
            _historyList = historyList ?? throw new ArgumentNullException(nameof(historyList));
            _cookieJar = cookieJar ?? throw new ArgumentNullException(nameof(cookieJar));
        }

        /// <inheritdoc/>
        public async Task<bool> RunCollectionAsync(Item item, CancellationToken cancellationToken)
        {
            if (item?.Children == null)
            {
                return false;
            }

            foreach (Item child in item.Children)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                var response = await SendRequestAsync(child, cancellationToken, false);
                child.Response = response;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<WorkspaceResponse> SendRequestAsync(
            Item workspaceRequest,
            CancellationToken cancellationToken,
            bool addToHistory)
        {
            if (workspaceRequest?.Url.ToString() == null)
            {
                throw new ArgumentNullException("Request or its base URL cannot be null.", nameof(workspaceRequest));
            }

            WorkspaceResponse response;
            var errorStopwatch = new Stopwatch();

            try
            {
                errorStopwatch.Start();
                response = await _nightingaleClient.SendAsync(workspaceRequest, cancellationToken);
                errorStopwatch.Stop();
            }
            catch (TaskCanceledException)
            {
                errorStopwatch.Stop();

                response =  new WorkspaceResponse
                {
                    StatusDescription = "Cancelled",
                    TimeElapsed = errorStopwatch.ElapsedMilliseconds,
                    Log = "The request was canceled."
                };
            }
            catch (Exception e)
            when (e.InnerException != null
                && e.InnerException.Message.Contains("security problem occurred"))
            {
                errorStopwatch.Stop();
                response = new WorkspaceResponse
                {
                    StatusDescription = "Error",
                    TimeElapsed = errorStopwatch.ElapsedMilliseconds,
                    Body = e.InnerException.Message + "Are you connecting to localhost? Make sure to turn off SSL validation in settings.",
                    Log = $"Security error while sending request. {e.Message}"
                };
            }
            catch (Exception e)
            when (e.InnerException != null
                && e.InnerException.Message.Contains("The server name or address could not be resolved"))
            {
                errorStopwatch.Stop();
                response = new WorkspaceResponse
                {
                    StatusDescription = "Bad URL",
                    TimeElapsed = errorStopwatch.ElapsedMilliseconds,
                    Body = "The server name or address could not be resolved. Are you sure the URL is correct?",
                    Log = $"{e.InnerException.Message}"
                };
            }
            catch (Exception e)
            {
                errorStopwatch.Stop();

                response = new WorkspaceResponse
                {
                    StatusDescription = "Error",
                    TimeElapsed = errorStopwatch.ElapsedMilliseconds,
                    Body = e.Message 
                        + System.Environment.NewLine 
                        + e.StackTrace,
                    Log = $"Unexpected error while sending request. {e.Message}"
                };
            }

            if (addToHistory)
            {
                await _historyList.AddAsync(workspaceRequest, DateTime.Now);
            }

            if (response.Headers.Any(x => x.Key == "Set-Cookie")
                && Uri.IsWellFormedUriString(response.RequestBaseUrl, UriKind.Absolute))
            {
                var cookies = response.Headers.Where(h => h.Key == "Set-Cookie").Select(x => new Cookies.Models.Cookie()
                {
                    Domain = new Uri(response.RequestBaseUrl, UriKind.Absolute).Host,
                    Raw = x.Value
                }).ToList();

                _cookieJar.AddCookies(cookies);
            }

            return response;
        }
    }
}
