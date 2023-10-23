using Autodesk.Revit.DB;
using CoreNodeModels;
using DSRevitNodesUI.Controls;
using Dynamo.Controls;
using Dynamo.Graph.Nodes;
using Dynamo.Nodes;
using Dynamo.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using System.Collections.ObjectModel;
using RevitServices.Persistence;
using Autodesk.Revit.UI.Selection;
using Dynamo.Logging;
using DSCore;
using Dynamo.UI.Commands;

namespace Dynamo.ComboNodes
{

    #region NodeModels

    [NodeName("Select Model Elements By Category"),
    NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
    NodeDescription("SelectModelElementsByCategoryDescription", typeof(DSRevitNodesUI.Properties.Resources)),
    IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.Element[]")]
    public class DSModelElementsByCategorySelection : ElementFilterSelection<Element>
    {
        private const string message = "Select Model Elements";
        private const string prefix = "Element";

        internal CategoryElementSelectionFilter<Element> SelectionFilter { get; set; }

        public DSRevitNodesUI.Categories DropDownNodeModel { get; set; }

        [JsonProperty(PropertyName = "SelectedIndex")]
        public int SelectedIndex
        {
            get { return DropDownNodeModel.SelectedIndex; }
            set
            {
                DropDownNodeModel.SelectedIndex = value;
                if (DropDownNodeModel.SelectedIndex >= 0)
                {
                    SelectionFilter.Category = (BuiltInCategory)DropDownNodeModel.Items[SelectedIndex].Item;
                }
            }
        }

        [JsonProperty(PropertyName = "SelectedString")]
        public string SelectedString
        {
            get { return DropDownNodeModel.SelectedString; }
            set
            {
                DropDownNodeModel.SelectedString = value;

                if (DropDownNodeModel.SelectedIndex >= 0)
                {
                    SelectionFilter.Category = (BuiltInCategory)DropDownNodeModel.Items[SelectedIndex].Item;
                }
            }
        }

        public DSModelElementsByCategorySelection()
            : base(
                SelectionType.Many,
                SelectionObjectType.None,
                message,
                prefix)
        {
            DropDownNodeModel = new DSRevitNodesUI.Categories();
            SelectedIndex = DropDownNodeModel.SelectedIndex;
            SelectionFilter = new CategoryElementSelectionFilter<Element>();
            base.Filter = SelectionFilter;
        }

        [JsonConstructor]
        public DSModelElementsByCategorySelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.Many,
                SelectionObjectType.None,
                message,
                prefix,
                selectionIdentifier,
                inPorts,
                outPorts)
        {
            DropDownNodeModel = new DSRevitNodesUI.Categories();
            SelectionFilter = new CategoryElementSelectionFilter<Element>();
            base.Filter = SelectionFilter;
        }
    }

    [NodeName("Select Model Element By Category"),
    NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
    NodeDescription("SelectModelElementByCategoryDescription", typeof(DSRevitNodesUI.Properties.Resources)),
    IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.Element")]
    public class DSModelElementByCategorySelection : ElementFilterSelection<Element>
    {
        private const string message = "Select Model Element";
        private const string prefix = "Element";

        internal CategoryElementSelectionFilter<Element> SelectionFilter { get; set; }

        public DSRevitNodesUI.Categories DropDownNodeModel { get; set; }

        [JsonProperty(PropertyName = "SelectedIndex")]
        public int SelectedIndex
        {
            get { return DropDownNodeModel.SelectedIndex; }
            set
            {
                DropDownNodeModel.SelectedIndex = value;
                if (DropDownNodeModel.SelectedIndex >= 0)
                {
                    SelectionFilter.Category = (BuiltInCategory)DropDownNodeModel.Items[SelectedIndex].Item;
                }
            }
        }

        [JsonProperty(PropertyName = "SelectedString")]
        public string SelectedString
        {
            get { return DropDownNodeModel.SelectedString; }
            set
            {
                DropDownNodeModel.SelectedString = value;

                if (DropDownNodeModel.SelectedIndex >= 0)
                {
                    SelectionFilter.Category = (BuiltInCategory)DropDownNodeModel.Items[SelectedIndex].Item;
                }
            }
        }

        public DSModelElementByCategorySelection()
            : base(
                SelectionType.One,
                SelectionObjectType.None,
                message,
                prefix)
        {
            DropDownNodeModel = new DSRevitNodesUI.Categories();
            SelectedIndex = DropDownNodeModel.SelectedIndex;
            SelectionFilter = new CategoryElementSelectionFilter<Element>();
            base.Filter = SelectionFilter;
        }

        [JsonConstructor]
        public DSModelElementByCategorySelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts,
            IEnumerable<PortModel> outPorts)
            : base(
                SelectionType.One,
                SelectionObjectType.None,
                message,
                prefix,
                selectionIdentifier,
                inPorts,
                outPorts)
        {
            DropDownNodeModel = new DSRevitNodesUI.Categories();
            SelectionFilter = new CategoryElementSelectionFilter<Element>();
            base.Filter = SelectionFilter;
        }
    }

    #endregion

    public class CategoryDropDown : DSRevitNodesUI.Categories
    {
        public CategoryDropDown() : base() { }
    }

    #region Node View Customization

    public class DSModelElementsByCategorySelectionNodeViewCustomization : INodeViewCustomization<DSModelElementsByCategorySelection>
    {
        public DSModelElementsByCategorySelection Model { get; set; }
        public DelegateCommand SelectCommand { get; set; }

        public void CustomizeView(DSModelElementsByCategorySelection model, NodeView nodeView)
        {
            Model = model;
            SelectCommand = new DelegateCommand((_) => Model.Select(null),(_)=> Model.CanBeginSelect());
            Model.PropertyChanged += (s, e) => {
                nodeView.Dispatcher.Invoke(new Action(() =>
                {
                    if (e.PropertyName == "CanSelect")
                    {
                        SelectCommand.RaiseCanExecuteChanged();
                    }
                }));
            };
            var comboControl = new ComboControl { DataContext = this };
            nodeView.inputGrid.Children.Add(comboControl);
        }

        public void Dispose()
        {
        }
    }

    public class DSModelElementByCategorySelectionNodeViewCustomization : INodeViewCustomization<DSModelElementByCategorySelection>
    {
        public DSModelElementByCategorySelection Model { get; set; }
        public DelegateCommand SelectCommand { get; set; }

        public void CustomizeView(DSModelElementByCategorySelection model, NodeView nodeView)
        {
            Model = model;
            SelectCommand = new DelegateCommand((_) => Model.Select(null), (_) => Model.CanBeginSelect());
            Model.PropertyChanged += (s, e) => {
                nodeView.Dispatcher.Invoke(new Action(() =>
                {
                    if (e.PropertyName == "CanSelect")
                    {
                        SelectCommand.RaiseCanExecuteChanged();
                    }
                }));
            };
            var comboControl = new ComboControl { DataContext = this };
            nodeView.inputGrid.Children.Add(comboControl);
        }

        public void Dispose()
        {
        }
    }
    #endregion

    #region Selection Helpers
    public abstract class ElementFilterSelection<TSelection> : ElementSelection<TSelection>
    where TSelection : Element
    {
        public ISelectionFilter Filter { get; set; }
        protected ElementFilterSelection(SelectionType selectionType,
            SelectionObjectType selectionObjectType, string message, string prefix)
            : base(selectionType, selectionObjectType, message, prefix) { }

        [JsonConstructor]
        protected ElementFilterSelection(SelectionType selectionType,
            SelectionObjectType selectionObjectType, string message, string prefix,
            IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(selectionType, selectionObjectType, message, prefix, selectionIdentifier, inPorts, outPorts) { }

        public override IModelSelectionHelper<TSelection> SelectionHelper
        {
            get 
            { 
                var selectionHelper = RevitElementSelectionHelper<TSelection>.Instance;
                selectionHelper.ElementFilter = Filter;
                return selectionHelper;
            }
        }
    }

    [IsVisibleInDynamoLibrary(false)]
    internal class RevitElementSelectionHelper<T> : LogSourceBase, IModelSelectionHelper<T> where T : Autodesk.Revit.DB.Element
    {

        public static RevitElementSelectionHelper<T> Instance { get; } = new RevitElementSelectionHelper<T>();

        public ISelectionFilter ElementFilter { get; set; }

        /// <summary>
        /// Request an element in a selection.
        /// </summary>
        /// <typeparam name="T">The type of the Element.</typeparam>
        /// <param name="selectionMessage">The message to display.</param>
        /// <param name="selectionType">The selection type.</param>
        /// <param name="objectType">The selection object type.</param>
        /// <returns></returns>
        public IEnumerable<T> RequestSelectionOfType(
            string selectionMessage, 
            SelectionType selectionType, 
            SelectionObjectType objectType)
        {
            switch (selectionType)
            {
                case SelectionType.One:
                    return RequestElementSelection(selectionMessage, AsLogger());

                case SelectionType.Many:
                    return RequestMultipleElementsSelection(selectionMessage, AsLogger());
            }

            return null;
        }

        #region private static methods

        private IEnumerable<T> RequestElementSelection(string selectionMessage, ILogger logger)
        {
            var doc = DocumentManager.Instance.CurrentUIDocument;

            Element e = null;

            var choices = doc.Selection;
            choices.SetElementIds(new Collection<ElementId>());

            logger.Log(selectionMessage);

            var elementRef = doc.Selection.PickObject(
                ObjectType.Element,
                ElementFilter,
                selectionMessage);

            if (elementRef != null)
            {
                e = DocumentManager.Instance.CurrentDBDocument.GetElement(elementRef);
            }

            return new[] { e }.Cast<T>();
        }

        private IEnumerable<T> RequestMultipleElementsSelection(
            string selectionMessage, ILogger logger)
        {
            var doc = DocumentManager.Instance.CurrentUIDocument;

            var choices = doc.Selection;
            choices.SetElementIds(new Collection<ElementId>());

            logger.Log(selectionMessage);

            var elementRefs = doc.Selection.PickObjects(ObjectType.Element,
                ElementFilter,
                selectionMessage);

            if (elementRefs == null || !elementRefs.Any())
                return null;

            var elements = new List<T>();
            Element e = null;

            foreach (var elementRef in elementRefs)
            {
                e = DocumentManager.Instance.CurrentDBDocument.GetElement(elementRef);
                elements.Add((T)e);
            }

            return elements;
        }

        #endregion
    }

    #endregion

    #region Filters

    [IsVisibleInDynamoLibrary(false)]
    internal class CategoryElementSelectionFilter<T> : ISelectionFilter
    {
        public BuiltInCategory Category { get; set; }
        public bool AllowElement(Element elem)
        {
            return (BuiltInCategory)System.Enum.Parse(typeof(BuiltInCategory), elem.Category.Id.ToString()) == Category;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    [IsVisibleInDynamoLibrary(false)]
    internal class TypeElementSelectionFilter<T> : ISelectionFilter
    {
        public string ElementTypeName { get; set; }

        internal Type GetElementType()
        {
            return Types.FindTypeByNameInAssembly(ElementTypeName, "RevitAPI");
        }

        public bool AllowElement(Element elem)
        {
            return elem.GetType() == GetElementType();
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    #endregion

}
