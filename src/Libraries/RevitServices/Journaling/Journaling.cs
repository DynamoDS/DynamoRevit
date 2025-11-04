using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevitServices.Journaling
{
    public static class Journaling
    {
        /// <summary>
        /// Indicates whether it is in journal replaying mode.
        /// </summary>
        /// <returns>Whether journal is replaying or not.</returns>
        public static bool IsJournalReplaying()
        {
            var method = typeof(Autodesk.Revit.UI.UIFabricationUtils).GetMethod("IsJournalReplaying", BindingFlags.NonPublic | BindingFlags.Static);
            if (method != null)
            {
                return (bool)method.Invoke(null, null);
            }
            return false;
        }

        /// <summary>
        /// Write Async Event to the journal.
        /// </summary>
        public static void WriteAsyncEvent(string eventName, Action callback, int timeOutMilliseconds = -1)
        {
            Assembly journalClient = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "AddInJournalClient");
            if (journalClient == null)
                return;

            var type = journalClient.GetType("AddInJournalClient.JournalWriter");
            if (type == null)
                return;

            var method = type.GetMethod("WriteAsyncEvent", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                return;

            var parameters = new object[] { eventName, callback, timeOutMilliseconds };
            method.Invoke(null, parameters);
        }

        /// <summary>
        /// write error to the journal.
        /// </summary>
        public static void WriteError(string journalFormat, params object[] args)
        {
            Assembly journalClient = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "AddInJournalClient");
            if (journalClient == null)
                return;

            var type = journalClient.GetType("AddInJournalClient.JournalWriter");
            if (type == null)
                return;

            var method = type.GetMethod("WriteError", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                return;

            var parameters = new object[] { journalFormat, args };
            method.Invoke(null, parameters);
        }
    }
}
