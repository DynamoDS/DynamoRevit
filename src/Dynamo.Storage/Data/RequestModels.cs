using System.Collections.Generic;

namespace Dynamo.Storage.Data
{
    public class RequestBody
    {
        public Workspace Workspace { get; set; }
        public IEnumerable<Workspace> CustomNodes { get; set; }
        public Properties Properties { get; set; }
    }

    public class Workspace
    {
        public string Name { get; set; }
        public string Guid { get; set; }

        public IEnumerable<Node> Nodes { get; set; }
        public IEnumerable<Connection> Connections { get; set; }
    }

    public class Node
    {
        public string _id { get; set; }

        public string CreationName { get; set; }
        public string Replication { get; set; }
        public List<double> Position { get; set; }
        public List<object> IgnoreDefaults { get; set; }

        public dynamic Extra { get; set; }
    }

    public class Connection
    {
        public int StartPortIndex { get; set; }
        public int EndPortIndex { get; set; }

        public string StartNodeId { get; set; }
        public string EndNodeId { get; set; }
    }

    public class Properties
    {
        /// <summary>
        /// SendGeometry property
        /// </summary>
        /// <value>True if geometry data should be sent back in the response.</value>
        public bool SendGeometry { get; set; }
        /// <summary>
        /// SendSerializedNodes property
        /// </summary>
        /// <value>True if fully serialized node data should be sent back in the response.</value>
        public bool SendSerializedNodes { get; set; }
        /// <summary>
        /// NodesToSerialize property
        /// </summary>
        /// <value>List of GUIDs of nodes that should be fully serialized in the response. If this list 
        /// is not provided, and SendSerializedNodes is true, then all nodes will be serialized.</value>
        public IEnumerable<System.Guid> NodesToSerialize { get; set; }
    }
}
