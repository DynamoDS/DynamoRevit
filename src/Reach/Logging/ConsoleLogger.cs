using System;

namespace Reach.Logging
{
    public class ConsoleLogger : ILogger
    {
        private const string BasicLogFormat = "{{\"date\": \"{0}\",\"message\": \"{1}\"}}";
        private const string LogFormat = "{{\"status\" : \"{0}\",\"date\": \"{1}\",\"userId\": \"{2}\", \"sessionId\": \"{3}\"}}";
        private const string TypeMessageFormat = "{{\"status\" : \"{0}\", \"date\": \"{1}\", \"userId\": \"{2}\", \"sessionId\": \"{3}\", \"type\": \"{4}\"}}";
        private const string ContentMessageFormat = "{{\"status\" : \"{0}\", \"date\": \"{1}\", \"userId\": \"{2}\", \"sessionId\": \"{3}\", \"type\": \"{4}\", \"contents\": {5}}}";
        private const string ExceptionFormat = "{{\"status\" : \"Exception\", \"date\": \"{0}\", \"machineInfo\": \"{1}\", \"hostInfo\": \"{2}\", \"details\": \"{3}\", \"stackTrace\": \"{4}\", \"type\": \"{5}\"}}";

        string Now()
        {
            return DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        }

        public void Log(string message)
        {
            Console.WriteLine(string.Format(BasicLogFormat, Now(), message));
        }

        public void Log(string status, string userId, string sessionId)
        {
            Console.WriteLine(string.Format(LogFormat, status, Now(), userId, sessionId));
        }

        public void Log(string status, string userId, string sessionId, string type, string contents = null)
        {
            if (String.IsNullOrEmpty(contents))
            {
                Console.WriteLine(string.Format(TypeMessageFormat, status, Now(), userId, sessionId, type));
            }
            else
            {
                Console.WriteLine(string.Format(ContentMessageFormat, status, Now(), userId, sessionId, type, contents));
            }
        }

        public void Log(Exception ex, string machineName, string hostInfo)
        {
            var stack = ex.StackTrace.Replace("\r\n", ";").Replace("\n", ";");
            Console.WriteLine(string.Format(ExceptionFormat, Now(), machineName,
                                            hostInfo, ex.Message, stack, ex.GetType()));
        }
    }
}
