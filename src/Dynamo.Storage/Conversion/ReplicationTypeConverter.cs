using System.Collections.Generic;
using System.Linq;

namespace Dynamo.Storage.Conversion
{
    public class ReplicationTypeConverter
    {
        private static readonly Dictionary<string, string> replicationTypes;

        static ReplicationTypeConverter()
        {
            //      Flood            Dynamo
            replicationTypes = new Dictionary<string, string>
            {
                {"applyShortest",  "Shortest"},
                {"applyLongest",   "Longest"},
                {"applyCartesian", "CrossProduct"},
                {"applyDisabled",  "Disabled"},
                {"applyAuto", "Auto" }
            };
        }

        /// <summary>
        /// Converts Flood replication type to Dynamo lacing strategy
        /// </summary>
        /// <param name="type">Flood replication type</param>
        /// <returns>Dynamo lacing strategy</returns>
        public static string ToDynamoType(string type)
        {
            return replicationTypes.ContainsKey(type) ?
                replicationTypes[type] : 
                "Disabled";
        }

        /// <summary>
        /// Converts Dynamo lacing strategy to Flood replication type
        /// </summary>
        /// <param name="type">Dynamo lacing strategy</param>
        /// <returns>Flood replication type</returns>
        public static string ToFloodType(string type)
        {
            return replicationTypes.ContainsValue(type) ? 
                replicationTypes.First(x => x.Value == type).Key : 
                "applyDisabled";
        }
    }
}