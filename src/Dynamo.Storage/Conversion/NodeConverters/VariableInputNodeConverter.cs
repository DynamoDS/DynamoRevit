using Dynamo.Graph.Nodes;
using Dynamo.Storage.Conversion.ExtraInfo;
using PythonNodeModels;

namespace Dynamo.Storage.Conversion.NodeConverters
{
    class VariableInputNodeConverter: BaseConverter
    {
        public override bool ConvertsType(NodeModel node)
        {
            // A PythonNode is a VariableInputNode, but
            // we want to defer to the PythonNode's converter
            if (node is PythonNode)
            {
                return false;
            }

            return node is VariableInputNode;
        }

        internal override void SetExtra(NodeModel nodeModel, NodeToPublish node)
        {
            var extra = new VariableInputNodeExtra(nodeModel as VariableInputNode);
            node.Extra = extra;
        }
    }
}
