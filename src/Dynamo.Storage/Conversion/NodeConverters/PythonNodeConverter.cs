using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;
using PythonNodeModels;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class PythonNodeConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel node)
        {
            var converts = node is PythonNode;
            return converts;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new PythonNodeExtra(nodeModel as VariableInputNode);
            node.Extra = extra;
        }
    }
}
