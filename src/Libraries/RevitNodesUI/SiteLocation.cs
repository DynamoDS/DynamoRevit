﻿using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.Creation;

using Dynamo.Applications;
using Dynamo.Applications.Models;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.Nodes;

using Dynamo.UI;
using Dynamo.Wpf;

using ProtoCore.AST.AssociativeAST;
using Revit.GeometryConversion;

using Revit.Elements;
using RevitServices.Elements;
using RevitServices.Persistence;
using Autodesk.Revit.DB.Events;
using Dynamo.Applications;
using Dynamo.Graph.Nodes;
using BuiltinNodeCategories = Revit.Elements.BuiltinNodeCategories;

namespace DSRevitNodesUI
{
    public class SiteLocationNodeViewCustomization : INodeViewCustomization<SiteLocation>
    {
        public void CustomizeView(SiteLocation model, NodeView nodeView)
        {
            var locCtrl = new LocationControl { DataContext = this };
            nodeView.inputGrid.Children.Add(locCtrl);
        }

        public void Dispose()
        {

        }
    }

    [NodeName("SiteLocation"), NodeCategory(BuiltinNodeCategories.ANALYZE),
     NodeDescription("SiteLocationDescription", typeof(Properties.Resources)), IsDesignScriptCompatible]
    public class SiteLocation : RevitNodeModel
    {
        private readonly RevitDynamoModel model;

        public DynamoUnits.Location Location { get; set; }

        public SiteLocation()
        {
            OutPortData.Add(new PortData("Location", Properties.Resources.PortDataLocationToolTip));
            RegisterAllPorts();

            Location = DynamoUnits.Location.ByLatitudeAndLongitude(0.0, 0.0);
            Location.Name = string.Empty;
            
            ArgumentLacing = LacingStrategy.Disabled;

            DynamoRevitApp.EventHandlerProxy.DocumentOpened += model_RevitDocumentChanged;
            RevitServicesUpdater.Instance.ElementsUpdated += RevitServicesUpdater_ElementsUpdated;
            
            DynamoRevitApp.AddIdleAction(() => Update());
        }

        #region public methods

        public override void Dispose()
        {
            DynamoRevitApp.EventHandlerProxy.DocumentOpened -= model_RevitDocumentChanged;
            RevitServicesUpdater.Instance.ElementsUpdated -= RevitServicesUpdater_ElementsUpdated;
            base.Dispose();
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Location == null)
            {
                var nullNode = AstFactory.BuildNullNode();
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), nullNode) };
            }

            var latNode = AstFactory.BuildDoubleNode(Location.Latitude);
            var longNode = AstFactory.BuildDoubleNode(Location.Longitude);
            var nameNode = AstFactory.BuildStringNode(Location.Name);

            var node =
                AstFactory.BuildFunctionCall(
                    new Func<double, double,string, DynamoUnits.Location>(
                        DynamoUnits.Location.ByLatitudeAndLongitude), new List<AssociativeNode>() { latNode, longNode, nameNode });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }

        public override string ToString()
        {
            return string.Format(
                "Name: {0}, Lat: {1}, Lon: {2}",
                Location.Name,
                Location.Latitude,
                Location.Longitude);
        }
        
        #endregion

        #region private methods

        private void RevitServicesUpdater_ElementsUpdated(object sender, ElementUpdateEventArgs e)
        {
            if (e.Operation != ElementUpdateEventArgs.UpdateType.Modified)
                return;

            var locUuid = DocumentManager.Instance.CurrentDBDocument.SiteLocation.UniqueId;

            if (e.GetUniqueIds().Contains(locUuid))
            {
                Update();
            }
        }

        private void model_RevitDocumentChanged(object sender, System.EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            if (DocumentManager.Instance.CurrentDBDocument.IsFamilyDocument)
            {
                Location = null;
                Warning(Properties.Resources.SiteLocationFamilyDocumentWarning);
                return;
            }

            var location = DocumentManager.Instance.CurrentDBDocument.SiteLocation;
            Location.Name = location.PlaceName;
            Location.Latitude = location.Latitude.ToDegrees();
            Location.Longitude = location.Longitude.ToDegrees();

            OnNodeModified(true);

            RaisePropertyChanged("Location");
        }

        #endregion

    }
}
