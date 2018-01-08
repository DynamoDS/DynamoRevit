using System;

namespace Reach.Logging
{
    public interface ILogger
    {
        void Log(string message);
        void Log(Exception ex, string machineName, string hostInfo);
        void Log(string status, string userId, string sessionId);
        void Log(string status, string userId, string sessionId, string type, string contents = null);
    }
}
