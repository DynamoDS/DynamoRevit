using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class CodeBlockConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is CodeBlockNodeModel;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new CodeBlockExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
