using System.Runtime.Serialization;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Storage.Conversion;

namespace Reach.Messages.Data
{
    /// <summary>
    /// This class represents the data that is required to recreate nodes on the 
    /// client. When a file is uploaded and opened on Dynamo Server, this 
    /// information is delivered to the client to generate nodes found in the file.
    /// </summary>
    public class NodeToCreate: ReachNode
    {
        /// <summary>
        /// Value of the specified node if it's some input node, code block or custom node
        /// </summary>
        [DataMember]
        public object Value { get; private set; }

        /// <summary>
        /// Call this method when this node should be created on a client.
        /// </summary>
        /// <param name="node">The specified node</param>
        /// <param name="data">Represents a value of the node. 
        /// Also can contain node's ports information for code block or custom node</param>
        public NodeToCreate(NodeModel node, string data): base(node)
        {
            switch (CreationName)
            {
                case "Number":
                    double number;
                    Value = double.TryParse(data, out number) ? number : 0;
                    break;
                case "Boolean":
                    bool boolValue;
                    Value = bool.TryParse(data, out boolValue) ? boolValue : false;
                    break;
                case "String":
                case "Code Block":
                case "Python Script":
                case "List.Create":
                case "Input":
                case "Output":
                case "Number Slider":
                case "Integer Slider":
                case "Watch 3D":
                case "Convert Between Units":
                    Value = data;
                    break;
                default:
                    if (node is Function)
                    {
                        Value = data;
                    }
                    break;
            }
        }
    }
}
