using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using Dynamo.Applications;
using Dynamo.Controls;
using Dynamo.Interfaces;

using Dynamo.Models;
using Dynamo.Wpf;
using ProtoCore.AST.AssociativeAST;

using Revit.Elements;
using Revit.GeometryConversion;
using Revit.GeometryObjects;
using Revit.Interactivity;

using RevitServices.Elements;
using RevitServices.Persistence;

using DividedSurface = Autodesk.Revit.DB.DividedSurface;
using Element = Autodesk.Revit.DB.Element;
using RevitDynamoModel = Dynamo.Applications.Models.RevitDynamoModel;
using Point = Autodesk.DesignScript.Geometry.Point;
using String = System.String;
using UV = Autodesk.DesignScript.Geometry.UV;
using RevitServices.EventHandler;
using Autodesk.Revit.DB.Events;
using CoreNodeModels;
using Dynamo.Applications;
using DSRevitNodesUI.Properties;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using RevitServices.Transactions;

using Newtonsoft.Json;

namespace Dynamo.Nodes
{

    #region RevitSelection

    public abstract class RevitSelection<TSelection, TResult> : SelectionBase<TSelection, TResult>
    {
        protected Document SelectionOwner { get; private set; }
        private RevitDynamoModel revitDynamoModel;

        #region public properties

        public RevitDynamoModel RevitDynamoModel
        {
            get
            {
               return revitDynamoModel;
            }
            set
            {
                if (revitDynamoModel != null)
                {
                    var hwm = revitDynamoModel.CurrentWorkspace as HomeWorkspaceModel;
                    if (hwm != null)
                    {
                        hwm.RunSettings.PropertyChanged -= revMod_PropertyChanged;
                    }
                }

                revitDynamoModel = value;

                if (revitDynamoModel != null)
                {
                    var hwm = revitDynamoModel.CurrentWorkspace as HomeWorkspaceModel;
                    if (hwm != null)
                    {
                        hwm.RunSettings.PropertyChanged += revMod_PropertyChanged;
                    }
                }
            }
       }

        public override bool CanSelect
        {
            get
            {
                if (revitDynamoModel != null)
                {
                    // Different document, disable selection button.
                    if (!revitDynamoModel.IsInMatchingDocumentContext)
                        return false;
                    
                    var hwm = revitDynamoModel.CurrentWorkspace as HomeWorkspaceModel;
                    if (hwm != null)
                    {
                        return base.CanSelect && hwm.RunSettings.RunEnabled;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                   return base.CanSelect;
                }
            }
            set { base.CanSelect = value; }
        }

        public override string SelectionSuggestion
        {
            get
            {
               if (revitDynamoModel != null)
                {
                    var hwm = revitDynamoModel.CurrentWorkspace as HomeWorkspaceModel;
                    if (hwm != null)
                    {
                        return hwm.RunSettings.RunEnabled
                            ? base.SelectionSuggestion
                            : DSRevitNodesUI.Properties.Resources.SelectionIsDisabledDescription;
                    }
                    else
                    {
                        return DSRevitNodesUI.Properties.Resources.SelectionIsDisabledDescription;
                    }
                }
                else
                {
                    return base.SelectionSuggestion;
                }   
            }
        }

        /// <summary>
        /// Handler for the RevitDynamoModel's PropertyChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void revMod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Use the RunEnabled flag on the dynamo model
            // to set the CanSelect flag, enabling or disabling
            // any bound UI when the dynamo model is not
            // in a runnable state.
            if (e.PropertyName == "RunEnabled")
            {
                RaisePropertyChanged("CanSelect");
                RaisePropertyChanged("SelectionSuggestion");
            }
        }
     

        #endregion

        #region protected constructors

        protected RevitSelection(SelectionType selectionType,
            SelectionObjectType selectionObjectType, string message, string prefix)
            : base(selectionType, selectionObjectType, message, prefix)
        {
            RevitServicesUpdater.Instance.ElementsUpdated += Updater_ElementsUpdated;
            DynamoRevitApp.EventHandlerProxy.DocumentOpened += Controller_RevitDocumentChanged;
        }

        [JsonConstructor]
        public RevitSelection(SelectionType selectionType,
            SelectionObjectType selectionObjectType, string message, string prefix,
            IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(selectionType, selectionObjectType, message, prefix, selectionIdentifier, inPorts, outPorts)
        {
            RevitServicesUpdater.Instance.ElementsUpdated += Updater_ElementsUpdated;
            DynamoRevitApp.EventHandlerProxy.DocumentOpened += Controller_RevitDocumentChanged;
        }

        #endregion

        #region ElementSync

        private void Controller_RevitDocumentChanged(object sender, EventArgs e)
        {
            ClearSelections();
        }

        #endregion

        #region public methods

        public override void Dispose()
        {
            base.Dispose();

            RevitServicesUpdater.Instance.ElementsUpdated -= Updater_ElementsUpdated;
            DynamoRevitApp.EventHandlerProxy.DocumentOpened -= Controller_RevitDocumentChanged;

            if (revitDynamoModel != null)
            {
                var hwm = revitDynamoModel.CurrentWorkspace as HomeWorkspaceModel;
                if (hwm != null)
                {
                    hwm.RunSettings.PropertyChanged -= revMod_PropertyChanged;
                }
            }
         }

        public override void UpdateSelection(IEnumerable<TSelection> rawSelection)
        {
            base.UpdateSelection(rawSelection);
            SelectionOwner = DocumentManager.Instance.CurrentDBDocument;
        }

        #endregion

        #region protected methods
        private void Updater_ElementsUpdated(object sender, ElementUpdateEventArgs e)
        {
            switch (e.Operation)
            {
                case ElementUpdateEventArgs.UpdateType.Added:
                    break;
                case ElementUpdateEventArgs.UpdateType.Modified:
                    bool dynamoTransaction = e.Transactions.Contains(TransactionWrapper.TransactionName);
                    HomeWorkspaceModel hwm = null;
                    if (revitDynamoModel != null)
                    {
                        hwm = revitDynamoModel.CurrentWorkspace as HomeWorkspaceModel;
                    }
                    if (!dynamoTransaction || (hwm != null && hwm.RunSettings.RunType == RunType.Manual))
                    {
                        Updater_ElementsModified(e.GetUniqueIds());
                    }
                    break;
                case ElementUpdateEventArgs.UpdateType.Deleted:
                    Updater_ElementsDeleted(e.RevitDocument, e.Elements);
                    break;
                default:
                    break;
            }
        }

        protected virtual void Updater_ElementsDeleted(
            Document document, IEnumerable<ElementId> deleted) { }

        protected virtual void Updater_ElementsModified(IEnumerable<string> updated) { }

        #endregion
    }

    #endregion

    #region ElementSelection

    /// <summary>
    /// A selection type where the derived class specifies the type to select,
    /// and returns an element.
    /// </summary>
    /// <typeparam name="TSelection"></typeparam>
    public abstract class ElementSelection<TSelection> : RevitSelection<TSelection, Element>
        where TSelection : Element
    {
        protected ElementSelection(SelectionType selectionType,
            SelectionObjectType selectionObjectType, string message, string prefix)
            : base(selectionType, selectionObjectType, message, prefix) { }

        [JsonConstructor]
        protected ElementSelection(SelectionType selectionType,
            SelectionObjectType selectionObjectType, string message, string prefix,
            IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(selectionType, selectionObjectType, message, prefix, selectionIdentifier, inPorts, outPorts) { }

        

        public override IModelSelectionHelper<TSelection> SelectionHelper
        {
            get { return RevitElementSelectionHelper<TSelection>.Instance; }
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;
            Func<string, bool, Revit.Elements.Element> func = ElementSelector.ByUniqueId;

            var results = SelectionResults.ToList();

            if (SelectionResults == null || !results.Any())
            {
                node = AstFactory.BuildNullNode();
            }
            else if (results.Count == 1)
            {
                var el = results.First();

                // If there is only one object in the list,
                // return a single item.
                node = AstFactory.BuildFunctionCall(
                    func,
                    new List<AssociativeNode>
                    {
                        AstFactory.BuildStringNode(el.UniqueId),
                        AstFactory.BuildBooleanNode(true)
                    });
            }
            else
            {
                var newInputs =
                    results.Select(
                        el =>
                            AstFactory.BuildFunctionCall(
                                func,
                                new List<AssociativeNode>
                                {
                                    AstFactory.BuildStringNode(el.UniqueId),
                                    AstFactory.BuildBooleanNode(true)
                                })).ToList();

                node = AstFactory.BuildExprList(newInputs);
            }

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }

        protected override string FormatSelectionText<T>(IEnumerable<T> elements)
        {
            if (typeof(T) != typeof(Element)) return "";

            var ids =
                elements.Cast<Element>().Where(el => el.IsValidObject).Select(el => el.Id).ToArray();
            return ids.Any() ? String.Join(" ", ids.Take(20)) : "";
        }

        protected override TSelection GetModelObjectFromIdentifer(string id)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var e = doc.GetElement(id);
            return e as TSelection;
        }

        protected override string GetIdentifierFromModelObject(TSelection modelObject)
        {
            try
            {
                return modelObject == null ? null : modelObject.UniqueId;
            }
            catch (Autodesk.Revit.Exceptions.InvalidObjectException)
            {
                // This call is being made from SelectionBase.SerializeCore in 
                // scenarios like dragging of a node (undo recorder needs the 
                // node to be serialized prior to dragging). If the current 
                // document is not the same as that of the selected modelObject,
                // an exception will be thrown. Dynamo shouldn't be crashed in 
                // cases like this, so handle this exception gracefully.
                // 
                return null;
            }
        }

        protected override void Updater_ElementsDeleted(
            Document document, IEnumerable<ElementId> deleted)
        {
            if (!SelectionResults.Any() || 
                !document.Equals(SelectionOwner) ||
                !deleted.Any())
            {
                return;
            }

            // If the deleting operations does not make any elements in SelectionResults
            // invalid, then there is no need to update.
            if (SelectionResults.Where(el => !el.IsValidObject).Count() == 0)
            {
                return;
            }

            // We are given a set of ElementIds, but because the elements
            // have already been deleted from Revit, we can't get the 
            // corresponding GUID. Instead, we just go through the collection of
            // elements and get the ones that are still valid.

            var validEls = Selection.Where(el => el.IsValidObject).ToList();

            UpdateSelection(validEls);
        }

        protected override void Updater_ElementsModified(IEnumerable<string> updated)
        {
            // If nothing has been updated, then return

            if (!updated.Any())
                return;

            // If the updated list doesn't include any objects in the current selection
            // then return;
            if (!SelectionResults.Where(x => x.IsValidObject).Select(x => x.UniqueId).Any(updated.Contains))
            {
                return;
            }

            UpdateSelection(Selection);
        }

        protected override IEnumerable<Element> ExtractSelectionResults(TSelection selection)
        {
            yield return selection;
        }
    }

    #endregion

    #region ReferenceSelection

    /// <summary>
    /// A selection type for selecting reference objects in Revit (Faces, Edges, etc.)
    /// </summary>
    public abstract class ReferenceSelection : RevitSelection<Reference, Reference>
    {
        protected ReferenceSelection(SelectionType selectionType,
            SelectionObjectType selectionObjectType, string message, string prefix)
            : base(selectionType, selectionObjectType, message, prefix) { }

        [JsonConstructor]
        public ReferenceSelection(SelectionType selectionType,
            SelectionObjectType selectionObjectType, string message, string prefix,
            IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(selectionType, selectionObjectType, message, prefix, selectionIdentifier, inPorts, outPorts) { }

        public override IModelSelectionHelper<Reference> SelectionHelper
        {
            get { return RevitReferenceSelectionHelper.Instance; }
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;
            Func<string, object> func = GeometryObjectSelector.ByReferenceStableRepresentation;

            var results = SelectionResults.ToList();

            if (SelectionResults == null || !results.Any())
            {
                node = AstFactory.BuildNullNode();
            }
            else if (results.Count == 1)
            {
                var stableRef = GetIdentifierFromModelObject(results.First());

                node = AstFactory.BuildFunctionCall(
                    func,
                    new List<AssociativeNode> { AstFactory.BuildStringNode(stableRef), });
            }
            else
            {
                var stableRefs = results.Select(GetIdentifierFromModelObject);

                var newInputs =
                    stableRefs.Select(
                        stableRef =>
                            AstFactory.BuildFunctionCall(
                                func,
                                new List<AssociativeNode> { AstFactory.BuildStringNode(stableRef), }))
                        .ToList();

                node = AstFactory.BuildExprList(newInputs);
            }

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }

        protected override string FormatSelectionText<T>(IEnumerable<T> elements)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;

            var ids =
                elements.Cast<Reference>()
                    .Select(doc.GetElement)
                    .Where(el => el != null)
                    .Select(el => el.Id);

            return ids.Any() ? String.Join(" ", ids.Take(20)) : "";
        }

        protected override Reference GetModelObjectFromIdentifer(string id)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            try
            {
                var reference = Reference.ParseFromStableRepresentation(doc, id);
                return reference;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override string GetIdentifierFromModelObject(Reference modelObject)
        {
            if (modelObject == null)
            {
                return null;
            }

            try
            {
                var doc = DocumentManager.Instance.CurrentDBDocument;
                return modelObject.ConvertToStableRepresentation(doc);
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override void Updater_ElementsDeleted(
            Document document, IEnumerable<ElementId> deleted)
        {
            // If an element is deleted, ensure all references which refer
            // to that element are removed from the selection

            // If there is no selection, or the doc of the deleted
            // elements is not this doc, or if there is nothing
            // in the deleted set, then return

            if (!SelectionResults.Any() ||
                !document.Equals(SelectionOwner) ||
                !deleted.Any() ||
                !SelectionResults.Any(x => deleted.Contains(x.ElementId))) return;

            // The new selections is everything in the current selection
            // that is not in the deleted collection as well
            var newSelection = SelectionResults.Where(x => !deleted.Contains(x.ElementId));

            UpdateSelection(newSelection);
        }

        protected override void Updater_ElementsModified(IEnumerable<string> updated)
        {
            // If there is nothing modified or the SelectionResults
            // collection is null, then return
            if (!updated.Any() || SelectionResults == null)
            {
                return;
            }

            var doc = DocumentManager.Instance.CurrentDBDocument;

            // If this modification is being parsed as part of a document
            // update that also contains a deletion, then we need to try to 
            // get the elements first to see if they are valid. 
            var validIds = SelectionResults.Select(doc.GetElement).Where(x => x != null).Select(x => x.UniqueId);

            // If none of the updated elements are included in the 
            // list of valid ids in the selection, then return.
            if (!validIds.Any(updated.Contains))
            {
                return;
            }

            // We want this modification to trigger a graph reevaluation
            // and we want the AST for this node to be regenerated.
            OnNodeModified(forceExecute: true);
        }

        protected override IEnumerable<Reference> ExtractSelectionResults(Reference selection)
        {
            yield return selection;
        }
    }

    #endregion

    [NodeName("Select Analysis Results"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectAnalysisResultsDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible,
     IsVisibleInDynamoLibrary(false)]
    public class DSAnalysisResultSelection : ElementSelection<Element>
    {
        public DSAnalysisResultSelection()
            : base(
                SelectionType.One,
                SelectionObjectType.None,
                "Select an analysis result.",
                "Analysis Results") { }

        [JsonConstructor]
        public DSAnalysisResultSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                  SelectionType.One, 
                  SelectionObjectType.None, 
                  "Select an analysis result.", 
                  "Analysis Results", 
                  selectionIdentifier, 
                  inPorts, 
                  outPorts) { }
    }

    [NodeName("Select Model Element"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectModelElementDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible]
    public class DSModelElementSelection : ElementSelection<Element>
    {
        public DSModelElementSelection()
            : base(
                SelectionType.One,
                SelectionObjectType.None,
                "Select Model Element",
                "Element") { }

        [JsonConstructor]
        public DSModelElementSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.One,
                SelectionObjectType.None,
                "Select Model Element",
                "Element",
                selectionIdentifier,
                inPorts,
                outPorts) { }
    }

    [NodeName("Select Face"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectFaceDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible]
    public class DSFaceSelection : ReferenceSelection
    {
        public DSFaceSelection()
            : base(
                SelectionType.One,
                SelectionObjectType.Face,
                "Select a face.",
                "Face of Element Id") { }
        
        [JsonConstructor]
        public DSFaceSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                  SelectionType.One, 
                  SelectionObjectType.Face, 
                  "Select a face.", 
                  "Face of Element Id", 
                  selectionIdentifier, 
                  inPorts, 
                  outPorts) {}
    }

    [NodeName("Select Edge"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectEdgeDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible]
    public class DSEdgeSelection : ReferenceSelection
    {
        public DSEdgeSelection()
            : base(
                SelectionType.One,
                SelectionObjectType.Edge,
                "Select an edge.",
                "Edge of Element Id") { }

        [JsonConstructor]
        public DSEdgeSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.One,
                SelectionObjectType.Edge,
                "Select an edge.",
                "Edge of Element Id",
                selectionIdentifier,
                inPorts,
                outPorts) { }
    }

    [NodeName("Select Point on Face"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectPointonFaceDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible]
    public class DSPointOnElementSelection : ReferenceSelection
    {
        public DSPointOnElementSelection()
            : base(
                SelectionType.One,
                SelectionObjectType.PointOnFace,
                "Select a point on a face.",
                "Point on Element") { }

        [JsonConstructor]
        public DSPointOnElementSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.One,
                SelectionObjectType.PointOnFace,
                "Select a point on a face.",
                "Point on Element",
                selectionIdentifier,
                inPorts,
                outPorts) { }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;

            if (SelectionResults == null || !SelectionResults.Any())
            {
                node = AstFactory.BuildNullNode();
            }
            else
            {
                var newInputs = new List<AssociativeNode>();

                foreach (var reference in SelectionResults)
                {
                    if (reference.GlobalPoint == null)
                    {
                        return new[]
                        {
                            AstFactory.BuildAssignment(
                                GetAstIdentifierForOutputIndex(0),
                                AstFactory.BuildNullNode())
                        };
                    }

                    var pt = reference.GlobalPoint.ToPoint();

                    //this is a selected point on a face
                    var ptArgs = new List<AssociativeNode>
                    {
                        AstFactory.BuildDoubleNode(pt.X),
                        AstFactory.BuildDoubleNode(pt.Y),
                        AstFactory.BuildDoubleNode(pt.Z)
                    };

                    var functionCallNode =
                        AstFactory.BuildFunctionCall(
                            new Func<double, double, double, Point>(Point.ByCoordinates),
                            ptArgs);

                    newInputs.Add(functionCallNode);
                }

                node = AstFactory.BuildExprList(newInputs);
            }

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }
    }

    [NodeName("Select UV on Face"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectUVonFaceDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible]
    public class DSUvOnElementSelection : ReferenceSelection
    {
        public DSUvOnElementSelection()
            : base(
                SelectionType.One,
                SelectionObjectType.PointOnFace,
                "Select a point on a face.",
                "UV on Element") { }

        [JsonConstructor]
        public DSUvOnElementSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.One,
                SelectionObjectType.PointOnFace,
                "Select a point on a face.",
                "UV on Element",
                selectionIdentifier,
                inPorts,
                outPorts) { }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;

            if (SelectionResults == null || !SelectionResults.Any())
            {
                node = AstFactory.BuildNullNode();
            }
            else
            {
                var newInputs = new List<AssociativeNode>();

                foreach (var reference in SelectionResults)
                {
                    if (reference.UVPoint == null)
                    {
                        return new[]
                        {
                            AstFactory.BuildAssignment(
                                GetAstIdentifierForOutputIndex(0),
                                AstFactory.BuildNullNode())
                        };
                    }

                    var pt = reference.UVPoint;

                    //this is a selected point on a face
                    var ptArgs = new List<AssociativeNode>
                    {
                        AstFactory.BuildDoubleNode(pt.U),
                        AstFactory.BuildDoubleNode(pt.V),
                    };

                    var functionCallNode =
                        AstFactory.BuildFunctionCall(
                            new Func<double, double, UV>(UV.ByCoordinates),
                            ptArgs);

                    newInputs.Add(functionCallNode);
                }

                node = AstFactory.BuildExprList(newInputs);
            }

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }
    }

    [NodeName("Select Divided Surface Families"),
     NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectDividedSurfaceFamiliesDescription", typeof(DSRevitNodesUI.Properties.Resources)),
     IsDesignScriptCompatible]
    public class DSDividedSurfaceFamiliesSelection : ElementSelection<DividedSurface>
    {
        public DSDividedSurfaceFamiliesSelection()
            : base(
                SelectionType.One,
                SelectionObjectType.None,
                "Select a divided surface.",
                "Elements") { }

        [JsonConstructor]
        public DSDividedSurfaceFamiliesSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.One,
                SelectionObjectType.None,
                "Select a divided surface.",
                "Elements",
                selectionIdentifier,
                inPorts,
                outPorts) { }

        // Set an update method. When the target object is modified in
        // Revit, this will cause the sub-elements to be modified.
        protected override IEnumerable<Element> ExtractSelectionResults(DividedSurface selection)
        {
            IEnumerable<Element> result;
            try
            {
                result = RevitElementSelectionHelper<DividedSurface>.GetFamilyInstancesFromDividedSurface(
                            selection).ToList();
            }
            catch
            {
                result = new List<Element>();
            }

            return result;
        }

        public override string ToString()
        {
            if (Selection.Any() && !SelectionResults.Any())
            {
                return Resources.NoFamilyInstancesInDividedSurfaceWarning;
            }

            return base.ToString();
        }
    }

    [NodeName("Select Model Elements"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectModelElementsDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible]
    public class DSModelElementsSelection : ElementSelection<Element>
    {
        public DSModelElementsSelection()
            : base(
                SelectionType.Many,
                SelectionObjectType.None,
                "Select elements.",
                "Elements") { }

        [JsonConstructor]
        public DSModelElementsSelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.Many,
                SelectionObjectType.None,
                "Select elements.",
                "Elements",
                selectionIdentifier,
                inPorts,
                outPorts)
        { }
    }

    [NodeName("Select Faces"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectFacesDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible]
    public class SelectFaces : ReferenceSelection
    {
        public SelectFaces()
            : base(
                SelectionType.Many,
                SelectionObjectType.Face,
                "Select faces.",
                "Faces") { }

        [JsonConstructor]
        public SelectFaces(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.Many,
                SelectionObjectType.Face,
                "Select faces.",
                "Faces",
                selectionIdentifier,
                inPorts,
                outPorts)
        { }
    }

    [NodeName("Select Edges"), NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
     NodeDescription("SelectEdgesDescription", typeof(DSRevitNodesUI.Properties.Resources)), IsDesignScriptCompatible]
    public class SelectEdges : ReferenceSelection
    {
        public SelectEdges()
            : base(
                SelectionType.Many,
                SelectionObjectType.Edge,
                DSRevitNodesUI.Properties.Resources.SelectEdgesDescription,
                DSRevitNodesUI.Properties.Resources.SelectEdgesPrefix) { }

        [JsonConstructor]
        public SelectEdges(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.Many,
                SelectionObjectType.Edge,
                DSRevitNodesUI.Properties.Resources.SelectEdgesDescription,
                DSRevitNodesUI.Properties.Resources.SelectEdgesPrefix,
                selectionIdentifier,
                inPorts,
                outPorts) { }
    }

}
