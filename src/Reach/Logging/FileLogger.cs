using System;
using System.IO;
using System.Threading;

namespace Reach.Logging
{
    public class FileLogger : ILogger
    {
        private string path;
        private static ReaderWriterLockSlim readWriteLock;
        private const string LogFormat = "{0} [\r\n    date: {1},\r\n    userId: {2},\r\n    sessionId: {3}\r\n]";
        private const string TypeMessageFormat = "{0} [\r\n    date: {1},\r\n     userId: {2},\r\n     sessionId: {3},\r\n    type: {4}\r\n ]";
        private const string ContentMessageFormat = "{0} [\r\n    date: {1},\r\n    userId: {2},\r\n    sessionId: {3},\r\n    type: {4},\r\n    contents: {5}\r\n]";
        private const string ExceptionFormat = "exception [\r\n    date: {0},\r\n    machineInfo: {1},\r\n    hostInfo: {2},\r\n    details: {3},\r\n    stackTrace: {4}\r\n    type: {5}\r\n]";

        string Now()
        {
            return DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
        }
        
        void SafelyLog(string message)
        {
            readWriteLock.EnterWriteLock();

            try
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine(message);
                    sw.Close();
                }
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }

        public FileLogger(string path)
        {
            this.path = path;
            readWriteLock = new ReaderWriterLockSlim();
        }

        public void Log(string message)
        {
            SafelyLog(message);
        }

        public void Log(string status, string userId, string sessionId)
        {
            SafelyLog(string.Format(LogFormat, status, userId, sessionId));
        }

        public void Log(string status, string userId, string sessionId, string type, string contents = null)
        {
            if (String.IsNullOrEmpty(contents))
            {
                SafelyLog(string.Format(TypeMessageFormat, status, Now(), userId, sessionId, type));
            }
            else
            {
                SafelyLog(string.Format(ContentMessageFormat, status, Now(), userId, sessionId, type, contents));
            }
        }

        public void Log(Exception ex, string machineName, string hostInfo) 
        {
            SafelyLog(string.Format(ExceptionFormat, Now(), machineName, hostInfo, ex.Message, ex.StackTrace, ex.GetType()));
        }
    }
}
