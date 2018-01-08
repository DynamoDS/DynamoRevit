using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class FunctionExtra: BaseExtraInfo
    {
        public string FunctionName { get; private set; }
        public string CreationName { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsProxy { get; private set; }
        public int NumInputs { get; private set; }
        public int NumOutputs { get; private set; }
        public IEnumerable<string> InputNames { get; private set; }
        public IEnumerable<string> OutputNames { get; private set; }

        public FunctionExtra(NodeModel nodeModel)
        {
            FunctionName = nodeModel.Name;
            CreationName = nodeModel.CreationName;
            DisplayName = nodeModel.Name;
            IsProxy = false;
            NumInputs = nodeModel.InPorts.Count;
            NumOutputs = nodeModel.OutPorts.Count;
            InputNames = nodeModel.InPorts.Select(portModel => portModel.Name).ToArray();
            OutputNames = nodeModel.OutPorts.Select(portModel => portModel.Name).ToArray();
        }
    }
}
