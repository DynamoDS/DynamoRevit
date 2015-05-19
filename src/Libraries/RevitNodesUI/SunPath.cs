﻿using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.UI.Events;

using Dynamo.Applications;
using Dynamo.Applications.Models;
using Dynamo.Models;

using ProtoCore.AST.AssociativeAST;

using Revit.Elements;
using RevitServices.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace DSRevitNodesUI
{
    [NodeName("SunSettings.Current"), NodeCategory(BuiltinNodeCategories.REVIT_VIEW),
     NodeDescription("SunSettingsCurrentDescription", typeof(Properties.Resources)), IsDesignScriptCompatible]
    public class SunSettings : RevitNodeModel
    {
        private string settingsID;

        public SunSettings()
        {
            OutPortData.Add(
                new PortData("SunSettings", Properties.Resources.PortDataSunSettingToolTip));

            RegisterAllPorts();
            DynamoRevit.AddIdleAction(
                () =>
                {
                    RevitServicesUpdater.Instance.ElementsModified += Updater_ElementsModified;
                    DocumentManager.Instance.CurrentUIApplication.ViewActivated +=
                        CurrentUIApplication_ViewActivated;

                    CurrentUIApplicationOnViewActivated();
                }
        );
        }

        public override void Dispose()
        {
            DynamoRevit.AddIdleAction(
                () =>
                {
                    RevitServicesUpdater.Instance.ElementsModified -=
                        Updater_ElementsModified;
                    DocumentManager.Instance.CurrentUIApplication.ViewActivated -=
                        CurrentUIApplication_ViewActivated;
                });
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

        private void Updater_ElementsModified(IEnumerable<string> updated)
        {
            if (updated.Contains(settingsID))
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
