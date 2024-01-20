using Nightingale.Core.Interfaces;
using System;

namespace Nightingale.Core.Helpers
{
    /// <summary>
    /// Class that wraps around an ILoggable
    /// object and appends logs to the ILoggable.
    /// </summary>
    public class OnDemandLogger : ILogger
    {
        private readonly ILoggable _loggable;

        public OnDemandLogger(ILoggable loggable)
        {
            _loggable = loggable;
        }

        public string FlushLogs()
        {
            return _loggable?.Log ?? "";
        }

        public void Log(string line)
        {
            if (_loggable == null)
            {
                return;
            }

            _loggable.Log += line + Environment.NewLine;
        }
    }
}
