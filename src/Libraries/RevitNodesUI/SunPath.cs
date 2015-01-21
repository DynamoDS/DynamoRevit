using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI.Events;

using Dynamo.Applications.Models;
using Dynamo.Models;
using Dynamo.Nodes;

using ProtoCore.AST.AssociativeAST;

using RevitServices.Elements;
using RevitServices.Persistence;

namespace DSRevitNodesUI
{
    [NodeName("SunSettings.Current"), NodeCategory(BuiltinNodeCategories.REVIT_VIEW),
     NodeDescription("SunSettingsCurrentDescription", typeof(Properties.Resources)), IsDesignScriptCompatible]
    public class SunSettings : RevitNodeModel
    {
        private string settingsID;

        public SunSettings()
        {
            OutPortData.Add(new PortData("SunSettings", Properties.Resources.PortDataSunSettingToolTip));
            
            RegisterAllPorts();
            
            RevitServicesUpdater.Instance.ElementsModified += Updater_ElementsModified;
            DocumentManager.Instance.CurrentUIApplication.ViewActivated += CurrentUIApplication_ViewActivated;

            CurrentUIApplicationOnViewActivated();
        }

        public override void Dispose()
        {
            base.Dispose();

            RevitServicesUpdater.Instance.ElementsModified -= Updater_ElementsModified;
            DocumentManager.Instance.CurrentUIApplication.ViewActivated -=
                CurrentUIApplication_ViewActivated;
        }

        private void CurrentUIApplication_ViewActivated(object sender, ViewActivatedEventArgs e)
        {
            CurrentUIApplicationOnViewActivated();
        }

        private void CurrentUIApplicationOnViewActivated()
        {
            settingsID =
                DocumentManager.Instance.CurrentDBDocument.ActiveView.SunAndShadowSettings.UniqueId;
            ForceReExecuteOfNode = true;
            OnAstUpdated();
        }

        private void Updater_ElementsModified(IEnumerable<string> updated)
        {
            if (updated.Contains(settingsID))
            {
                ForceReExecuteOfNode = true;
                OnAstUpdated();
            }
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            Func<Revit.Elements.SunSettings> func = Revit.Elements.SunSettings.Current;

            return new[]
            {
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildFunctionCall(func, new List<AssociativeNode>()))
            };
        }
    

    }
}
