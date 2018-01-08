using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class WatchConverter : BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is Watch;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new WatchExtra(nodeModel as Watch);
            node.Extra = extra;
        }
    }
}
