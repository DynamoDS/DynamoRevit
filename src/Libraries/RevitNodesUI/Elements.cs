﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Structure;

using Dynamo.Applications;
using Dynamo.Applications.Models;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;
using Revit.Elements;
using Revit.Elements.InternalUtilities;
using RevitServices.Elements;
using RevitServices.Persistence;
using Category = Revit.Elements.Category;
using CurveElement = Autodesk.Revit.DB.CurveElement;
using DividedSurface = Autodesk.Revit.DB.DividedSurface;
using Element = Autodesk.Revit.DB.Element;
using FamilyType = Revit.Elements.FamilyType;
using Level = Revit.Elements.Level;
using ModelText = Autodesk.Revit.DB.ModelText;
using ReferencePlane = Autodesk.Revit.DB.ReferencePlane;
using ReferencePoint = Autodesk.Revit.DB.ReferencePoint;
using BuiltinNodeCategories = Revit.Elements.BuiltinNodeCategories;
using View = Revit.Elements.Views.View;
using RevitServices.Transactions;

namespace DSRevitNodesUI
{
    public abstract class ElementsQueryBase : RevitNodeModel
    {
        protected ElementsQueryBase()
        {
            var u = RevitServicesUpdater.Instance;
            u.ElementsUpdated += OnElementsUpdated;

            ShouldDisplayPreviewCore = true;
        }

        [JsonConstructor]
        public ElementsQueryBase(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            var u = RevitServicesUpdater.Instance;
            u.ElementsUpdated += OnElementsUpdated;

            ShouldDisplayPreviewCore = true;
        }

        void OnElementsUpdated(object sender, ElementUpdateEventArgs e)
        {
            if (!e.Elements.Any()) return;

#if DEBUG
            Debug.WriteLine("There are {0} elements {1}", e.Elements.Count(), e.Operation.ToString());
            DebugElements(e.Elements);
#endif
            OnNodeModified(forceExecute: true);
        }

        public override void Dispose()
        {
            base.Dispose();

            var u = RevitServicesUpdater.Instance;
            u.ElementsUpdated -= OnElementsUpdated;
        }

        private static void DebugElements(IEnumerable<ElementId> updated)
        {
            var els = updated.Select(
                id => DocumentManager.Instance.CurrentDBDocument.GetElement(id));
            foreach (var el in els.Where(el => el != null)) {
                Debug.WriteLine(string.Format("\t{0}", el.Name));
            }
        }

    }

    [NodeName("All Elements of Family Type"), 
     NodeCategory(BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("ElementsofFamilyTypeDescription",typeof(Properties.Resources)),
     IsDesignScriptCompatible]
    public class ElementsOfFamilyType : ElementsQueryBase
    {
        public ElementsOfFamilyType()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Family Type", Properties.Resources.PortDataFamilTypeToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("Elements", Properties.Resources.PortDataElementsToolTip)));

            RegisterAllPorts();
        }

        [JsonConstructor]
        public ElementsOfFamilyType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            var func =
                new Func<FamilyType, IList<Revit.Elements.Element>>(ElementQueries.OfFamilyType);

            var functionCall = AstFactory.BuildFunctionCall(func, inputAstNodes);
            return new[]
            { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }

    [NodeName("All Elements of Type"), NodeCategory(BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("ElementsofTypeDescription", typeof(Properties.Resources)),
     IsDesignScriptCompatible]
    public class ElementsOfType : ElementsQueryBase
    {
        public ElementsOfType()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("element type", Properties.Resources.PortDataElementTypeToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("elements", Properties.Resources.PortDataAllElementsInDocumentToolTip)));
            RegisterAllPorts();
        }

        [JsonConstructor]
        public ElementsOfType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            var func = new Func<Type, IList<Revit.Elements.Element>>(ElementQueries.OfElementType);

            var functionCall = AstFactory.BuildFunctionCall(func, inputAstNodes);
            return new[]
            { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }

    [NodeName("All Elements of Category"), NodeCategory(BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("ElementsofCategoryDescription", typeof(Properties.Resources)),
     IsDesignScriptCompatible]
    public class ElementsOfCategory : ElementsQueryBase
    {
        public ElementsOfCategory()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Category", Properties.Resources.PortDataCategoryToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("Elements", Properties.Resources.PortDataElementTypeToolTip)));

            RegisterAllPorts();
        }

        [JsonConstructor]
        public ElementsOfCategory(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            var func = new Func<Category, View, IList<Revit.Elements.Element>>(ElementQueries.OfCategory);

            var functionCall = AstFactory.BuildFunctionCall(func, inputAstNodes);
            return new[]
            { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }

    [NodeName("All Elements of Category in View"), NodeCategory(BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("ElementsofCategoryInViewDescription", typeof(Properties.Resources)),
     IsDesignScriptCompatible]
    public class ElementsOfCategoryInView : ElementsQueryBase
    {
        public ElementsOfCategoryInView()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Category", Properties.Resources.PortDataCategoryToolTip)));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("View", Properties.Resources.PortDataViewToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("Elements", Properties.Resources.PortDataElementTypeToolTip)));

            RegisterAllPorts();
        }

        [JsonConstructor]
        public ElementsOfCategoryInView(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            var func = new Func<Category, View, IList<Revit.Elements.Element>>(ElementQueries.OfCategory);

            var functionCall = AstFactory.BuildFunctionCall(func, inputAstNodes);
            return new[]
            { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }

    [NodeName("All Elements at Level"), NodeCategory(BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("ElementsatLevelDescription", typeof(Properties.Resources)),
     IsDesignScriptCompatible]
    public class ElementsAtLevel : ElementsQueryBase
    {
        public ElementsAtLevel()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Level", Properties.Resources.PortDataALevelToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("Elements", Properties.Resources.PortDataElementAtLevelToolTip)));

            RegisterAllPorts();
        }

        [JsonConstructor]
        public ElementsAtLevel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            var func = new Func<Level, IList<Revit.Elements.Element>>(ElementQueries.AtLevel);
            var functionCall = AstFactory.BuildFunctionCall(func, inputAstNodes);
            return new[]
            { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }

    [NodeName("Element By Id"), NodeCategory(BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("ElementById", typeof(Properties.Resources)),
     IsDesignScriptCompatible]
    public class ElementById : ElementsQueryBase
    {
        public ElementById()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Id", Properties.Resources.PortDataByElementId)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("Element", Properties.Resources.PortDataElementsToolTip)));

            RegisterAllPorts();
        }

        [JsonConstructor]
        public ElementById(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            var func = new Func<object, Revit.Elements.Element>(ElementQueries.ById);
            var functionCall = AstFactory.BuildFunctionCall(func, inputAstNodes);
            return new[]
            { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }

    [NodeName("All Elements In Active View"), NodeCategory(BuiltinNodeCategories.REVIT_VIEW),
     NodeDescription("ElementsInActiveViewDescription", typeof(Properties.Resources)),
     IsDesignScriptCompatible]
    public class ElementsInView : RevitNodeModel
    {
        private Document doc;
        private HashSet<ElementId> elementIds = new HashSet<ElementId>();
        private HashSet<string> uniqueIds = new HashSet<string>();

        public ElementsInView()
        {
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("elements", Properties.Resources.PortDataAllVisibleElementsToolTip)));
            RegisterAllPorts();

            DynamoRevitApp.EventHandlerProxy.ViewActivated += RevitDynamoModel_RevitDocumentChanged;
            DynamoRevitApp.EventHandlerProxy.DocumentOpened += RevitDynamoModel_RevitDocumentChanged;

            RevitServicesUpdater.Instance.ElementsUpdated += RevitServicesUpdaterOnElementsUpdated;
            RevitDynamoModel_RevitDocumentChanged(null, null);
        }

        [JsonConstructor]
        public ElementsInView(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            DynamoRevitApp.EventHandlerProxy.ViewActivated += RevitDynamoModel_RevitDocumentChanged;
            DynamoRevitApp.EventHandlerProxy.DocumentOpened += RevitDynamoModel_RevitDocumentChanged;

            RevitServicesUpdater.Instance.ElementsUpdated += RevitServicesUpdaterOnElementsUpdated;
            RevitDynamoModel_RevitDocumentChanged(null, null);
        }

        public override void Dispose()
        {
            DynamoRevitApp.EventHandlerProxy.ViewActivated -= RevitDynamoModel_RevitDocumentChanged;
            DynamoRevitApp.EventHandlerProxy.DocumentOpened -= RevitDynamoModel_RevitDocumentChanged;

            RevitServicesUpdater.Instance.ElementsUpdated -= RevitServicesUpdaterOnElementsUpdated;

            base.Dispose();
        }

        void RevitServicesUpdaterOnElementsUpdated(object sender, ElementUpdateEventArgs e)
        {
            switch (e.Operation)
            {
                case ElementUpdateEventArgs.UpdateType.Added:
                    RevitServicesUpdaterOnElementsAdded(e.GetUniqueIds());
                    break;
                case ElementUpdateEventArgs.UpdateType.Modified:
                    RevitServicesUpdaterOnElementsModified(e.GetUniqueIds());
                    break;
                case ElementUpdateEventArgs.UpdateType.Deleted:
                    RevitServicesUpdaterOnElementsDeleted(e.RevitDocument, e.Elements);
                    break;
                default:
                    break;
            }
        }

        private void RevitServicesUpdaterOnElementsAdded(IEnumerable<string> updated)
        {
            var filter = GetVisibleElementFilter();

            bool recalc = false;
            foreach (var id in updated)
            {
                Element e;
                if (doc.TryGetElement(id, out e))
                {
                    if (filter.PassesFilter(e))
                    {
                        uniqueIds.Add(id);
                        elementIds.Add(e.Id);
                        recalc = true;
                    }
                }
            }
            if (recalc)
            {
                OnNodeModified(forceExecute: true);
            }
        }

        #region Get Visible Elements

        public static IList<Element> GetElementsVisibleInActiveView()
        {
            var fec = new FilteredElementCollector(
                DocumentManager.Instance.CurrentDBDocument,
                DocumentManager.Instance.CurrentDBDocument.ActiveView.Id);

            return
                fec.WherePasses(GetVisibleElementFilter())
                    .WhereElementIsNotElementType()
                    .ToElements();
        }

        private static ElementFilter GetVisibleElementFilter()
        {
            var filterList = new List<ElementFilter>();

            var fContinuousRail = new ElementClassFilter(typeof(ContinuousRail));
            var fRailing = new ElementClassFilter(typeof(Railing));
            var fStairs = new ElementClassFilter(typeof(Stairs));
            var fStairsLanding = new ElementClassFilter(typeof(StairsLanding));
            var fTopographySurface = new ElementClassFilter(typeof(TopographySurface));
            var fAssemblyInstance = new ElementClassFilter(typeof(AssemblyInstance));
            var fBaseArray = new ElementClassFilter(typeof(BaseArray));
            var fBeamSystem = new ElementClassFilter(typeof(BeamSystem));
            var fBoundaryConditions = new ElementClassFilter(typeof(BoundaryConditions));
            var fConnectorElement = new ElementClassFilter(typeof(ConnectorElement));
            var fControl = new ElementClassFilter(typeof(Control));
            var fCurveElement = new ElementClassFilter(typeof(CurveElement));
            var fDividedSurface = new ElementClassFilter(typeof(DividedSurface));
            var fCableTrayConduitRunBase = new ElementClassFilter(typeof(CableTrayConduitRunBase));
            var fHostObject = new ElementClassFilter(typeof(HostObject));
            var fInstance = new ElementClassFilter(typeof(Instance));
            var fmepSystem = new ElementClassFilter(typeof(MEPSystem));
            var fModelText = new ElementClassFilter(typeof(ModelText));
            var fOpening = new ElementClassFilter(typeof(Opening));
            var fPart = new ElementClassFilter(typeof(Part));
            var fPartMaker = new ElementClassFilter(typeof(PartMaker));
            var fReferencePlane = new ElementClassFilter(typeof(ReferencePlane));
            var fReferencePoint = new ElementClassFilter(typeof(ReferencePoint));
            var fSpatialElement = new ElementClassFilter(typeof(SpatialElement));
            var fAreaReinforcement = new ElementClassFilter(typeof(AreaReinforcement));
            var fHub = new ElementClassFilter(typeof(Hub));
            var fPathReinforcement = new ElementClassFilter(typeof(PathReinforcement));
            var fRebar = new ElementClassFilter(typeof(Rebar));
            var fTruss = new ElementClassFilter(typeof(Truss));
            var fViewport = new ElementClassFilter(typeof(Autodesk.Revit.DB.Viewport));
            var fScheduleSheetInstance = new ElementClassFilter(typeof(ScheduleSheetInstance));

            filterList.Add(fContinuousRail);
            filterList.Add(fRailing);
            filterList.Add(fStairs);
            filterList.Add(fStairsLanding);
            filterList.Add(fTopographySurface);
            filterList.Add(fAssemblyInstance);
            filterList.Add(fBaseArray);
            filterList.Add(fBeamSystem);
            filterList.Add(fBoundaryConditions);
            filterList.Add(fConnectorElement);
            filterList.Add(fControl);
            filterList.Add(fCurveElement);
            filterList.Add(fDividedSurface);
            filterList.Add(fCableTrayConduitRunBase);
            filterList.Add(fHostObject);
            filterList.Add(fInstance);
            filterList.Add(fmepSystem);
            filterList.Add(fModelText);
            filterList.Add(fOpening);
            filterList.Add(fPart);
            filterList.Add(fPartMaker);
            filterList.Add(fReferencePlane);
            filterList.Add(fReferencePoint);
            filterList.Add(fAreaReinforcement);
            filterList.Add(fHub);
            filterList.Add(fPathReinforcement);
            filterList.Add(fRebar);
            filterList.Add(fTruss);
            filterList.Add(fSpatialElement);
            filterList.Add(fViewport);
            filterList.Add(fScheduleSheetInstance);

            var cRvtLinks = new ElementCategoryFilter(BuiltInCategory.OST_RvtLinks);
            filterList.Add(cRvtLinks);

            var filters = new LogicalOrFilter(filterList);
            return filters;
        }

        #endregion

        private void RevitServicesUpdaterOnElementsModified(IEnumerable<string> updated)
        {
            if (updated.Any(uniqueIds.Contains))
            {
                OnNodeModified(forceExecute:true);
            }
        }

        private void RevitDynamoModel_RevitDocumentChanged(object sender, EventArgs e)
        {
            doc = DocumentManager.Instance.CurrentDBDocument;
            var elements = GetElementsVisibleInActiveView();
            elementIds = new HashSet<ElementId>(elements.Select(x => x.Id));
            uniqueIds = new HashSet<string>(elements.Select(x => x.UniqueId));
            OnNodeModified(forceExecute:true);
        }

        private void RevitServicesUpdaterOnElementsDeleted(
            Document document, IEnumerable<ElementId> deleted)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (doc == document || doc.Equals(document))
            {
                elementIds.RemoveWhere(deleted.Contains);
                uniqueIds =
                    new HashSet<string>(elementIds.Select(id => document.GetElement(id).UniqueId));

                OnNodeModified(forceExecute: true);
            }
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            if (uniqueIds == null)
                return new[] { AstFactory.BuildNullNode() };

            Func<string, bool, Revit.Elements.Element> func = ElementSelector.ByUniqueId;

            var elementList =
                AstFactory.BuildExprList(
                    uniqueIds.Select(
                        id =>
                            AstFactory.BuildFunctionCall(
                                func,
                                new List<AssociativeNode>
                                {
                                    AstFactory.BuildStringNode(id),
                                    AstFactory.BuildBooleanNode(true)
                                })).ToList());

            return new[]
            { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), elementList) };
        }
    }

    [NodeName("Rooms By Status"), NodeCategory(BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("RoomsByStatusDescription", typeof(Properties.Resources)),
     IsDesignScriptCompatible]
    public class RoomsByStatus : ElementsQueryBase
    {
        public RoomsByStatus()
        {
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("PlacedRooms", Properties.Resources.PortDataPlacedRoomsToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("UnplacedRooms", Properties.Resources.PortDataUnplacedRoomsToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("NotEnclosedRooms", Properties.Resources.PortDataNotEnclosedRoomsToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("RedundantRooms", Properties.Resources.PortDataElementAtLevelToolTip)));

            RegisterAllPorts();
        }

        [JsonConstructor]
        public RoomsByStatus(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
        List<AssociativeNode> inputAstNodes)
        {
            var packedId = "__temp" + AstIdentifierGuid;
            var func = new Func<List<List<Revit.Elements.Element>>>(ElementQueries.RoomsByStatus);
            return new[]
            {
                AstFactory.BuildAssignment(
                    AstFactory.BuildIdentifier(packedId),
                    AstFactory.BuildFunctionCall(func, inputAstNodes)),
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(0),
                    new IdentifierNode(packedId)
                    {
                        ArrayDimensions = new ArrayNode{Expr = AstFactory.BuildIntNode(0)}
                    }),
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(1),
                    new IdentifierNode(packedId)
                    {
                        ArrayDimensions = new ArrayNode{Expr = AstFactory.BuildIntNode(1)}
                    }),
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(2),
                    new IdentifierNode(packedId)
                    {
                        ArrayDimensions = new ArrayNode{Expr = AstFactory.BuildIntNode(2)}
                    }),
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(3),
                    new IdentifierNode(packedId)
                    {
                        ArrayDimensions = new ArrayNode{Expr = AstFactory.BuildIntNode(3)}
                    })
            };
        }
    }
}
