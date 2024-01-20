using Nightingale.Core.Interfaces;
using System.Text;

namespace Nightingale.Core.Logging
{
    /// <summary>
    /// Class for building a log. The log
    /// is stored in a StringBuilder 
    /// inside this instance.
    /// </summary>
    public class LogBuilder : ILogger
    {
        private readonly StringBuilder _builder = new StringBuilder();

        /// <inheritdoc/>
        public string FlushLogs()
        {
            var result = _builder.ToString();
            _builder.Clear();
            return result;
        }

        /// <inheritdoc/>
        public void Log(string information)
        {
            _builder.AppendLine(information);
        }
    }
}
