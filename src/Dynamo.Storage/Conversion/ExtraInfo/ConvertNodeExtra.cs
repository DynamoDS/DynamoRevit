using CoreNodeModels;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class ConvertNodeExtra : BaseExtraInfo
    {
        public string SelectedMetric { get; private set; }

        public string SelectedConvertFrom { get; private set; }

        public string SelectedConvertTo { get; private set; }

        public ConvertNodeExtra(NodeModel n)
        {
            var cNode = (DynamoConvert)n;
            SelectedMetric = cNode.SelectedMetricConversion.ToString();
            SelectedConvertFrom = cNode.SelectedFromConversion.ToString();
            SelectedConvertTo = cNode.SelectedToConversion.ToString();
        }
    }
}