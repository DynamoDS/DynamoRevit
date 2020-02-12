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
using System.Text;
using System.Threading.Tasks;
using CoreNodeModelsWpf.Nodes;
using Autodesk.DesignScript.Runtime;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Wpf.Nodes.Revit;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RevitServices.Persistence;
using Autodesk.Revit.UI.Selection;
using Dynamo.Logging;

namespace DSRevitNodesUI
{
    public class SelectionCombo
    {
        [NodeName("Select Model Elements By Category"),
            NodeCategory(Revit.Elements.BuiltinNodeCategories.REVIT_SELECTION),
            NodeDescription("SelectModelElementsByCategoryDescription", typeof(DSRevitNodesUI.Properties.Resources)),
            IsDesignScriptCompatible]
        public class DSModelElementByCategorySelection : ElementCategorySelection<Element>
        {
            private const string message = "Select Model Elements";
            private const string prefix = "Element";

            public Categories CategoriesNodeModel { get; set; }

            public int SelectedIndex 
            {
                get { return CategoriesNodeModel.SelectedIndex; }
                set 
                { 
                    CategoriesNodeModel.SelectedIndex = value; 
                    FilterCategory = (BuiltInCategory)CategoriesNodeModel.Items[value].Item; 
                }
            }
            
            public DSModelElementByCategorySelection()
                : base(
                    SelectionType.Many,
                    SelectionObjectType.None,
                    message,
                    prefix)
            {
                CategoriesNodeModel = new Categories();
            }

            [JsonConstructor]
            public DSModelElementByCategorySelection(IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts,
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
            }
        }

        public class DSModelElementByCategorySelectionNodeViewCustomization : INodeViewCustomization<DSModelElementByCategorySelection>
        {
            public DSModelElementByCategorySelection Model { get; set; }
            public DelegateCommand SelectCommand { get; set; }

            public void CustomizeView(DSModelElementByCategorySelection model, NodeView nodeView)
            {
                Model = model;
                SelectCommand = new DelegateCommand(() => Model.Select(null), Model.CanBeginSelect);
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

        public abstract class ElementCategorySelection<TSelection> : ElementSelection<TSelection>
        where TSelection : Element
        {
            public BuiltInCategory FilterCategory { get; set; }
            protected ElementCategorySelection(SelectionType selectionType,
                SelectionObjectType selectionObjectType, string message, string prefix)
                : base(selectionType, selectionObjectType, message, prefix) { }

            [JsonConstructor]
            protected ElementCategorySelection(SelectionType selectionType,
                SelectionObjectType selectionObjectType, string message, string prefix,
                IEnumerable<string> selectionIdentifier, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
                : base(selectionType, selectionObjectType, message, prefix, selectionIdentifier, inPorts, outPorts) { }

            public override IModelSelectionHelper<TSelection> SelectionHelper
            {
                get 
                { 
                    var selectionHelper = RevitElementSelectionHelper<TSelection>.Instance;
                    selectionHelper.FilterCategory = FilterCategory;
                    return selectionHelper;
                }
            }
        }


        [IsVisibleInDynamoLibrary(false)]
        internal class RevitElementSelectionHelper<T> : LogSourceBase, IModelSelectionHelper<T> where T : Autodesk.Revit.DB.Element
        {
            private CategoryElementSelectionFilter<T> selectionFilter;

            public static RevitElementSelectionHelper<T> Instance { get; } = new RevitElementSelectionHelper<T>();

            public BuiltInCategory FilterCategory { get; set; }

            public CategoryElementSelectionFilter<T> SelectionFilter
            {
                get { return selectionFilter; }
                set
                {
                    selectionFilter = value;
                    selectionFilter.Category = FilterCategory;
                }
            }

            /// <summary>
            /// Request an element in a selection.
            /// </summary>
            /// <typeparam name="T">The type of the Element.</typeparam>
            /// <param name="selectionMessage">The message to display.</param>
            /// <param name="selectionType">The selection type.</param>
            /// <param name="objectType">The selection object type.</param>
            /// <returns></returns>
            public IEnumerable<T> RequestSelectionOfType(
                string selectionMessage, SelectionType selectionType, SelectionObjectType objectType)
            {
                SelectionFilter = new CategoryElementSelectionFilter<T>();
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
                    SelectionFilter,
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
                    SelectionFilter,
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

    }
}
