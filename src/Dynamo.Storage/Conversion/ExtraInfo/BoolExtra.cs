using CoreNodeModels.Input;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class BoolExtra: BaseExtraInfo
    {
        public bool Lock { get; private set; }
        public bool Value { get; private set; }

        public BoolExtra(NodeModel nodeModel)
        {
            Lock = !(nodeModel.IsInputNode && nodeModel.IsSetAsInput);
            Value = ((Bool)nodeModel).Value;
        }
    }
}
