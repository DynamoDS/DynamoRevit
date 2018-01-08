using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class DoubleInputSliderExtra: BaseExtraInfo
    {
        public double Value { get; private set; }
        public double Min { get; private set; }
        public double Max { get; private set; }
        public double Step { get; private set; }
        public bool Lock { get; private set; }

        public DoubleInputSliderExtra(NodeModel nodeModel)
        {
            var doubleSlider = (CoreNodeModels.Input.DoubleSlider)nodeModel;
            Value = doubleSlider.Value;
            Min = doubleSlider.Min;
            Max = doubleSlider.Max;
            Step = doubleSlider.Step;
            Lock = !(nodeModel.IsInputNode && nodeModel.IsSetAsInput);
        }
    }
}
