using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class OutputExtra: BaseExtraInfo
    {
        public string Name { get; private set; }

        public OutputExtra(NodeModel nodeModel)
        {
            Name = ((Output)nodeModel).Symbol;
        }
    }
}
