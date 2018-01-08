using System.Runtime.Serialization;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Storage.Conversion;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage
{
    /// <summary>
    /// This class represents the data that is required to create nodes by publishing the workspace
    /// </summary>
    public class NodeToPublish : ReachNode
    {
        public NodeToPublish(NodeModel nodeModel) 
            : base(nodeModel)
        {
            var modelName = GetCreationName(nodeModel);
            
            this.Name = nodeModel.Name;
            this.TypeName = nodeModel is Function ? "CustomNode" : modelName;
            this.Selected = false;
            this.Visible = nodeModel.IsVisible;
            this.Extra = new BaseExtraInfo();
        }

        /// <summary>
        /// Represents name of published node
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Represents type name of published node
        /// </summary>
        [DataMember]
        public string TypeName { get; private set; }

        /// <summary>
        /// Indicates whether node is selected or not
        /// </summary>
        [DataMember]
        public bool Selected { get; private set; }

        /// <summary>
        /// Indicates whether node is visible or not
        /// </summary>
        [DataMember]
        public bool Visible { get; private set; }

        /// <summary>
        /// Contains extra object for Flood nodes
        /// </summary>
        [DataMember]
        public object Extra { get; set; }     
    }
}
