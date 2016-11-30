using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI.Events;

using Dynamo.Applications;
using Dynamo.Applications.Models;
using Dynamo.Graph.Nodes;

using ProtoCore.AST.AssociativeAST;
using RevitServices.Elements;
using RevitServices.Persistence;
using BuiltinNodeCategories = Revit.Elements.BuiltinNodeCategories;

namespace DSRevitNodesUI
{
    [NodeName("SunSettings.Current"), NodeCategory(BuiltinNodeCategories.REVIT_VIEW),
     NodeDescription("SunSettingsCurrentDescription", typeof(Properties.Resources)), IsDesignScriptCompatible]
    public class SunSettings : RevitNodeModel
    {
        private string settingsID;

        public SunSettings()
        {
            OutPorts.Add(new PortModel(PortType.Output, this,
                new PortData("SunSettings", Properties.Resources.PortDataSunSettingToolTip)));

            RegisterAllPorts();

            RevitServicesUpdater.Instance.ElementsUpdated += Updater_ElementsUpdated;
            DynamoRevitApp.EventHandlerProxy.ViewActivated += CurrentUIApplication_ViewActivated;

            DynamoRevitApp.AddIdleAction(() => CurrentUIApplicationOnViewActivated());
        }

        public override void Dispose()
        {
            RevitServicesUpdater.Instance.ElementsUpdated -= Updater_ElementsUpdated;
            DynamoRevitApp.EventHandlerProxy.ViewActivated -= CurrentUIApplication_ViewActivated;

            base.Dispose();
        }

        private void CurrentUIApplication_ViewActivated(object sender, ViewActivatedEventArgs e)
        {
            CurrentUIApplicationOnViewActivated();
        }

        private void CurrentUIApplicationOnViewActivated()
        {
            settingsID =
                DocumentManager.Instance.CurrentDBDocument.ActiveView.SunAndShadowSettings.UniqueId;
            OnNodeModified(forceExecute:true);
        }

        private void Updater_ElementsUpdated(object sender, ElementUpdateEventArgs e)
        {
            if (e.Operation != ElementUpdateEventArgs.UpdateType.Modified) return;

            if (e.GetUniqueIds().Contains(settingsID))
            {
                OnNodeModified(forceExecute:true);
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
