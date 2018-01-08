using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class OutputConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is Output;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new OutputExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
