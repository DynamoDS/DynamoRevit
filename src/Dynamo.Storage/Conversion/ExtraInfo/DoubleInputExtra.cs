using CoreNodeModels.Input;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class DoubleInputExtra
    {
        public bool Lock { get; private set; }
        public string Value { get; private set; }

        public DoubleInputExtra(NodeModel nodeModel)
        {
            Lock = !(nodeModel.IsInputNode && nodeModel.IsSetAsInput);
            Value = ((DoubleInput)nodeModel).Value;
        }
    }
}
