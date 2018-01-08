using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class ConvertNodeConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is DynamoConvert;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            node.Extra = new ConvertNodeExtra(nodeModel);
        }
    }
}
