using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Nodes;
using Dynamo.Storage.Utilities;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class CodeBlockExtra: BaseExtraInfo
    {
        public string Code { get; private set; }
        public bool Lock { get; private set; }
        public IEnumerable<string> Inputs { get; private set; }
        public IEnumerable<string> Outputs { get; private set; }
        public IEnumerable<int> LineIndices { get; private set; }

        public CodeBlockExtra(NodeModel node)
        {
            var cbn = (CodeBlockNodeModel)node;
            Code = cbn.Code;
            Lock = !(node.IsInputNode && node.IsSetAsInput);
            Inputs = cbn.InPorts.Select(portModel => portModel.Name).ToArray();
            Outputs = cbn.OutPorts.Select(portModel =>
            {
                // there's hardcoded prefix tooltip for all non unassigned variables on CBN.
                // but also there's a delta between 0.9 and 1.0 versions prefixes.
                // to overcome it we cast dynamo prefix to universal pre-defined one.
                var toolTipContent = portModel.ToolTip;
                var toolTip_guid = ProtoCore.DSASM.Constants.kTempVarForNonAssignment;

                if (toolTipContent.IndexOf(toolTip_guid) == -1) return toolTipContent;
                var tempArray = toolTipContent.Split(new string[] { toolTip_guid }, System.StringSplitOptions.None);
                return (tempArray.Length > 1)
                    ? Constants.ReachTempVarForNonAssignment + tempArray[1]
                    : toolTipContent;
            }).ToArray();

            var allDefs = CodeBlockUtils.GetDefinitionLineIndexMap(cbn.CodeStatements);
            LineIndices = allDefs.Select(def => def.Value - 1).ToArray();
        }
    }
}
