using Dynamo.Graph.Nodes;
using PythonNodeModels;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class PythonNodeExtra: VariableInputNodeExtra
    {
        public string Script { get; private set; }

        public PythonNodeExtra(VariableInputNode nodeModel ) : base(nodeModel)
        {
            var pyt = (PythonNode) nodeModel;
            Script = pyt.Script;
        }
    }
}
