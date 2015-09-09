using DSRevitNodesUI;
using Dynamo.Models;

namespace Dynamo.Wpf.Nodes.Revit
{
    public class FamilyInstanceParametersNodeViewCustomization : DropDownNodeViewCustomization, INodeViewCustomization<FamilyInstanceParameters>
    {
        public void CustomizeView(FamilyInstanceParameters model, Dynamo.Controls.NodeView nodeView)
        {
            base.CustomizeView(model, nodeView);

            // this is not a recommended workaround
            var ws = nodeView.ViewModel.DynamoViewModel.CurrentSpace as IHomeWorkspaceModel;
            if (ws == null) return;

            model.EngineController = ws.EngineController;
        }
    }
}
