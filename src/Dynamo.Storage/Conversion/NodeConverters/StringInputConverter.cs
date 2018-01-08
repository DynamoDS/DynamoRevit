using CoreNodeModels.Input;
using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class StringInputConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is StringInput;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new StringInputExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
