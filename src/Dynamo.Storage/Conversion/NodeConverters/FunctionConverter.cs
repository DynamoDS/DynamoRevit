using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class FunctionConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is Function;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new FunctionExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
