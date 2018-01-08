using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Nodes.ZeroTouch;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class DSVarArgFunctionExtra : BaseExtraInfo
    {
        public IEnumerable<VarInput> VarInputs { get; private set; }

        public int DefaultNumInputs { get; private set; }

        public DSVarArgFunctionExtra(DSVarArgFunction nodeModel)
        {
            DefaultNumInputs = nodeModel.DefaultNumInputs;

            VarInputs = nodeModel.InPorts.Select(portModel => new VarInput
            {
                Name = portModel.Name,
                Type = portModel.ToolTip
            }).ToArray();
        }
    }
}
