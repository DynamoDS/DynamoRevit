using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.ZeroTouch;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class DSVarArgFunctionConverter : BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is DSVarArgFunction;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new DSVarArgFunctionExtra(nodeModel as DSVarArgFunction);
            node.Extra = extra;
        }
    }
}
