using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class IntegerSliderExtra: BaseExtraInfo
    {
        public int Value { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }
        public int Step { get; private set; }
        public bool Lock { get; private set; }

        public IntegerSliderExtra(NodeModel nodeModel)
        {
            var intSlider = (CoreNodeModels.Input.IntegerSlider)nodeModel;
            Value = intSlider.Value;
            Min = intSlider.Min;
            Max = intSlider.Max;
            Step = intSlider.Step;
            Lock = !(nodeModel.IsInputNode && nodeModel.IsSetAsInput);
        }
    }
}
