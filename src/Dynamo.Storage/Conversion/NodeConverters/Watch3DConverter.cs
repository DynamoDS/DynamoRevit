using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;
using Watch3DNodeModels;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class Watch3DConverter : BaseConverter
    {
        public override bool ConvertsType(NodeModel type)
        {
            return type is Watch3D;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new Watch3DExtra(nodeModel as Watch3D);
            node.Extra = extra;
        }
    }
}
