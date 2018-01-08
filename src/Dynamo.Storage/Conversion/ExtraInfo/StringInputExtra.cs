using CoreNodeModels.Input;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class StringInputExtra: BaseExtraInfo
    {
        public bool Lock { get; private set; }
        public string Value { get; private set; }

        public StringInputExtra(NodeModel nodeModel)
        {
            Lock = !(nodeModel.IsInputNode && nodeModel.IsSetAsInput);
            Value = ((StringInput)nodeModel).Value;
        }
    }
}
