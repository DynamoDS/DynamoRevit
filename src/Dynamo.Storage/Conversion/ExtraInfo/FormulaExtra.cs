using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class FormulaExtra: BaseExtraInfo
    {
        public string Script { get; private set; }
        public IEnumerable<string> Inputs { get; private set; }
        public IEnumerable<string> Outputs { get; private set; }

        public FormulaExtra(NodeModel nodeModel)
        {
            var fn = (CoreNodeModels.Formula)nodeModel;

            Script = fn.FormulaString;
            Inputs = fn.InPorts.Select(portModel => portModel.Name).ToArray();
            Outputs = fn.OutPorts.Select(portModel => portModel.ToolTip).ToArray();
        }
    }
}
