using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class SymbolConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is Symbol;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new SymbolExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
