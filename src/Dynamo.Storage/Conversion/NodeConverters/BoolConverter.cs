using CoreNodeModels.Input;
using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class BoolConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is Bool;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new BoolExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
