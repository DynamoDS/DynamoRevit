using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Storage.Data;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;

namespace Dynamo.Storage.Conversion
{
    /// <summary>
    /// This class represents the data that is required to create nodes on Flood using interaction with Reach
    /// </summary>
    public class ReachNode
    {
        /// <summary>
        /// Guid of the specified node
        /// </summary>
        [DataMember]
        public string _id { get; private set; }

        /// <summary>
        /// CreationName of the specified node
        /// </summary>
        [DataMember]
        public string CreationName { get; private set; }

        /// <summary>
        /// DisplayName of the specified node
        /// </summary>
        [DataMember]
        public string DisplayName { get; private set; }

        /// <summary>
        /// X and Y coordinates of the specified node
        /// </summary>
        [DataMember]
        public IEnumerable<double> Position { get; private set; }

        /// <summary>
        /// Replication of the specified node
        /// </summary>
        [DataMember]
        public string Replication { get; private set; }

        /// <summary>
        /// IgnoreDefautlts of the specified node
        /// </summary>
        [DataMember]
        public IEnumerable<bool> IgnoreDefaults { get; private set; }

        /// <summary>
        /// Indicates whether the node geometry is shown
        /// </summary>
        [DataMember]
        public bool IsGeometryVisible { get; private set; }

        /// <summary>
        /// Indicates whether the node is frozen
        /// </summary>
        [DataMember]
        public bool IsFrozen { get; private set; }

        /// <summary>
        /// Indicates whether the node preview is pinned
        /// </summary>
        [DataMember]
        public bool IsPinned { get; private set; }

        /// <summary>
        /// Indicates whether the node is an Custom Node instance 
        /// </summary>
        [DataMember]
        public bool IsCustomNode { get; private set; }

        /// <summary>
        /// Represents the port data of a node. This is very specific to list@level. 
        /// </summary>
        [DataMember]
        public List<NodePortData> NodePortData { get; private set; }

        public ReachNode(NodeModel node)
        {
            this._id = node.GUID.ToString();
            this.CreationName = GetCreationName(node);
            this.Replication = GetReplication(node.ArgumentLacing);
            this.IgnoreDefaults = node.InPorts.Select(p => !p.UsingDefaultValue).ToList();
            this.DisplayName = node.Name;
            this.Position = new List<double> { node.X, node.Y };
            this.IsGeometryVisible = node.IsVisible;
            this.IsFrozen = node.isFrozenExplicitly;
            this.IsPinned = node.PreviewPinned;
            if (node is Function)
            {
                IsCustomNode = true;
            }

            //This update is only for 1.2 release. ideally, you should send the inports to Flood.
            //Today port model cannot be serialized. So, for 1.2 release this is the easiest solution
            //note : flood does not have list@level UI (until 1.2 release).          
            this.NodePortData = new List<NodePortData>();
            var portIndexTuples = node.InPorts.Select((port, index) => new { port, index });
            foreach (var t in portIndexTuples)
            {               
                this.NodePortData.Add(new NodePortData
                {
                    index = t.index,
                    level = t.port.Level,
                    useLevels = t.port.UseLevels,
                    shouldKeepListStructure = t.port.KeepListStructure
                });
            }
            
        }

        public static string GetCreationName(NodeModel node)
        {
            if (!string.IsNullOrEmpty(node.CreationName))
                return node.CreationName;

            Type type = node.GetType();
            object[] attribs = type.GetCustomAttributes(typeof(NodeNameAttribute), false);
            if (attribs.Length > 0)
            {
                var elCatAttrib = attribs[0] as NodeNameAttribute;
                return elCatAttrib.Name;
            }

            return type.Name;
        }

        private string GetReplication(LacingStrategy strategy)
        {
            switch (strategy)
            {
                case LacingStrategy.CrossProduct:
                    return "applyCartesian";
                case LacingStrategy.Disabled:
                    return "applyDisabled";
                case LacingStrategy.First:
                    return "applyFirst";
                case LacingStrategy.Longest:
                    return "applyLongest";
                case LacingStrategy.Shortest:
                    return "applyShortest";
                case LacingStrategy.Auto:
                    return "applyAuto";
                default:
                    return null;
            }
        }
    }
}
