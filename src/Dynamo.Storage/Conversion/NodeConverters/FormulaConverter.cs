using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class FormulaConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is CoreNodeModels.Formula;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new FormulaExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
