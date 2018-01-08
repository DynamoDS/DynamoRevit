using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    /// <summary>
    /// This is a base class for all nodes converter. 
    /// </summary>
    abstract class BaseConverter : INodeModelConverter<NodeModel>
    {
        /// <summary>
        /// For each descendant indicates whether converter could perform specific NodeModel or not
        /// </summary>
        public abstract bool ConvertsType(NodeModel type);

        /// <summary>
        /// Sets a specific extra field for each descendant
        /// <param name="nodeModel">NodeModel from Dynamo</param>
        /// <param name="nodeToPublish">Object that will contain all necessary fields for Flood</param>
        /// </summary>
        internal abstract void SetExtra(NodeModel nodeModel, NodeToPublish nodeToPublish);

        public NodeToPublish Convert(NodeModel nodeModel)
        {
            var nodeToPublish = new NodeToPublish(nodeModel);
            SetExtra(nodeModel, nodeToPublish);

            return nodeToPublish;
        }
    }
}
