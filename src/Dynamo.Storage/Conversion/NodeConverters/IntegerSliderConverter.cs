using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class IntegerSliderConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is CoreNodeModels.Input.IntegerSlider;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new IntegerSliderExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
