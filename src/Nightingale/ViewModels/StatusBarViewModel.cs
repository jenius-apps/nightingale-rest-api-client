using Microsoft.AppCenter.Analytics;
using Nightingale.Core.Models;

namespace Nightingale.ViewModels
{
    public class StatusBarViewModel : ViewModelBase
    {
        private WorkspaceResponse _responseStatus;

        public WorkspaceResponse ResponseStatus
        {
            get => _responseStatus;
            set
            {
                _responseStatus = value;
                RaisePropertyChanged(string.Empty);
            }
        }

        public bool? TestsAllPass
        {
            get => ResponseStatus?.TestsAllPass;
        }

        public string StatusString
        {
            get
            {
                if (ResponseStatus == null)
                {
                    return "--";
                }

                return $"{ResponseStatus?.StatusCode ?? 0} {ResponseStatus?.StatusDescription ?? string.Empty}";
            }
        }

        public string StatusCode
        {
            get
            {
                if (ResponseStatus == null)
                {
                    return "--";
                }

                return ResponseStatus.StatusCode.ToString();
            }
        }

        public bool? ResponseSuccessful
        {
            get => ResponseStatus?.Successful;
        }

        public string TimeElapsedString
        {
            get
            {
                if (ResponseStatus == null)
                {
                    return "0 ms";
                }

                return ResponseStatus.TimeElapsed > 1000
                    ? $"{ResponseStatus.TimeElapsed * 0.001} s"
                    : $"{ResponseStatus.TimeElapsed} ms";
            }
        }

        public string SizeString
        {
            get => GetSizeString();
        }

        public void OpenFlyout() => Analytics.TrackEvent("Show status flyout clicked");

        private string GetSizeString()
        {
            if (ResponseStatus?.RawBytes == null)
            {
                return "0 KB";
            }

            return ((double)ResponseStatus.RawBytes.Length / 1000).ToString() + " KB";
        }
    }
}
