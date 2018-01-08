using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class VariableInputNodeExtra: BaseExtraInfo
    {
        public IEnumerable<VarInput> VarInputs { get; protected set; }

        public VariableInputNodeExtra(VariableInputNode nodeModel)
        {
            VarInputs = nodeModel.InPorts.Select(portModel => new VarInput
            {
                Name = portModel.Name,
                Type = portModel.ToolTip
            }).ToArray();
        }
    }

    public class VarInput
    {
        public string Name;
        public string Type;
    }
}
