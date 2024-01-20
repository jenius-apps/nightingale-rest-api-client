using Nightingale.Core.Interfaces;
using System.Text;

namespace Nightingale.Core.Services
{
    public class WorkspaceResponseLogger : ILogger
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        public string FlushLogs()
        {
            var result = stringBuilder.ToString();
            stringBuilder.Clear();
            return result;
        }

        public void Log(string information)
        {
            stringBuilder.AppendLine($"> {information}");
        }
    }
}
