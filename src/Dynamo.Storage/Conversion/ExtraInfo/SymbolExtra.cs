using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class SymbolExtra: BaseExtraInfo
    {
        public string Name { get; private set; }

        public SymbolExtra(NodeModel nodeModel)
        {
            Name = ((Symbol)nodeModel).InputSymbol;
        }
    }
}
