using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Reach.Synchronization
{
    public static class RollbackFileLogger
    {
        public static string RollbackLogFilePath { get; private set; }
        public static bool RollbackLogFileEnabled { get; private set; }

        public static void EnableLogging(string rollbackLogFilePath)
        {
            try
            {
                using (File.Create(rollbackLogFilePath)) { }
                RollbackLogFilePath = rollbackLogFilePath;
                RollbackLogFileEnabled = true;
            }
            catch
            {
                RollbackLogFilePath = null;
                RollbackLogFileEnabled = false;
            }
        }

        public static void ResetLog()
        {
            if (RollbackLogFileEnabled)
            {
                File.WriteAllText(RollbackLogFilePath, string.Empty);
            }
        }

        public static void LogIntoFile(object obj1, object obj2, CommandAction marker)
        {
            if (RollbackLogFileEnabled)
            {
                var sb = new StringBuilder();
                sb.AppendLine(marker.ToString());
                sb.AppendLine(obj1.GetType().Name + " : " + JsonConvert.SerializeObject(obj1));
                if (obj2 != null)
                {
                    sb.AppendLine(obj1.GetType().Name + " : " + JsonConvert.SerializeObject(obj2));
                }
                sb.AppendLine("***");

                File.AppendAllText(RollbackLogFilePath, sb.ToString());
            }
        }
    }

    public enum CommandAction
    {
        Ignored,
        Failed,
        Allowed,
        SyncData
    }
}
