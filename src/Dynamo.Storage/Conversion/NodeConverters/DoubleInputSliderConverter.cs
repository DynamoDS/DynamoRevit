using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class DoubleInputSliderConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is CoreNodeModels.Input.DoubleSlider;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new DoubleInputSliderExtra(nodeModel);
            node.Extra = extra;
        }
    }
}
