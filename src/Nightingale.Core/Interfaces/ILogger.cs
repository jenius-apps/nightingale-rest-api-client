namespace Nightingale.Core.Interfaces
{
    public interface ILogger
    {
        void Log(string information);

        string FlushLogs();
    }
}
