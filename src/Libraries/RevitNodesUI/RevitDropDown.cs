using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Autodesk.Revit.DB;
using CoreNodeModels;
using DSCore;
using Dynamo.Applications;
using Dynamo.Engine;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Revit.Elements;
using RevitServices.Persistence;
using BuiltinNodeCategories = Revit.Elements.BuiltinNodeCategories;
using Category = Revit.Elements.Category;
using Element = Autodesk.Revit.DB.Element;
using Family = Autodesk.Revit.DB.Family;
using FamilySymbol = Autodesk.Revit.DB.FamilySymbol;
using Level = Autodesk.Revit.DB.Level;
using Parameter = Autodesk.Revit.DB.Parameter;

namespace DSRevitNodesUI
{
    public class DropDownItemEqualityComparer : IEqualityComparer<DynamoDropDownItem>
    {
        public bool Equals(DynamoDropDownItem x, DynamoDropDownItem y)
        {
            return string.Equals(x.Name, y.Name);
        }

        public int GetHashCode(DynamoDropDownItem obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public abstract class RevitDropDownBase : DSDropDownBase
    {

        protected RevitDropDownBase(string value) : base(value)
        {
            DynamoRevitApp.EventHandlerProxy.DocumentOpened += Controller_RevitDocumentChanged;
        }

        void Controller_RevitDocumentChanged(object sender, EventArgs e)
        {
            PopulateItems();
        }

        public override void Dispose()
        {
            DynamoRevitApp.EventHandlerProxy.DocumentOpened -= Controller_RevitDocumentChanged;
            base.Dispose();
        }
    }

    [NodeName("Family Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("FamilyTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class FamilyTypes : RevitDropDownBase
    {
        private const string NO_FAMILY_TYPES = "No family types available.";

        public FamilyTypes() : base("Family Type") { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            fec.OfClass(typeof(Family));
            var elements = fec.ToElements();
            if (!elements.Any())
            {
                Items.Add(new DynamoDropDownItem(NO_FAMILY_TYPES, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            foreach (Family family in elements)
            {
                foreach (var id in family.GetFamilySymbolIds())
                {
                    var fs = family.Document.GetElement(id);
                    Items.Add(new DynamoDropDownItem(string.Format("{0}:{1}", family.Name, fs.Name), fs));
                }
            }

            Items = Items.OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 ||
                Items[0].Name == NO_FAMILY_TYPES ||
                SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var args = new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(((FamilySymbol) Items[SelectedIndex].Item).Family.Name),
                AstFactory.BuildStringNode(((FamilySymbol) Items[SelectedIndex].Item).Name)
            };

            var functionCall = AstFactory.BuildFunctionCall
                <System.String, System.String, Revit.Elements.FamilyType>
                (Revit.Elements.FamilyType.ByFamilyNameAndTypeName, args);

            return new[] {AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }

    }

    [NodeName("Get Family Parameter")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("GetFamilyParameterDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class FamilyInstanceParameters : RevitDropDownBase 
    {
        private const string noFamilyParameters = "No family parameters available.";
        private Element element;
        private ElementId storedId = null;
        internal EngineController EngineController { get; set; }

        public FamilyInstanceParameters()
            : base("Parameter") 
        {
            this.AddPort(PortType.Input, new PortData("f", Properties.Resources.PortDataFamilySymbolToolTip), 0);
            this.PropertyChanged += OnPropertyChanged;
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CachedValue")
                return;

            if (InPorts.Any(x => x.Connectors.Count == 0))
            {
                Items.Clear(); //The input is not connected, so clear the list.
                return;
            }

            var oldElement = element;
            element = GetInputElement();
            if(element == null)
            {
                Items.Clear();
                return;
            }

            if (oldElement != null && oldElement.Id == element.Id)
                return;

            PopulateItems();
        }

        private static string getStorageTypeString(StorageType st)
        {
            switch (st)
            {
                case StorageType.Integer:
                    return "int";
                case StorageType.Double:
                    return "double";
                case StorageType.String:
                    return "string";
                case StorageType.ElementId:
                default:
                    return "id";
            }
        }

        /// <summary>
        /// Reads the internal element to get all the family 
        /// or instance parameters, and adds them to the Items collection.
        /// 
        /// Items are sorted alphabetically by name. 
        /// If the SelectedIndex is already set, it is set to zero.
        /// </summary>
        protected override SelectionState PopulateItemsCore(string currentSelection) //(IEnumerable set, bool readOnly)
        {
            //only update the collection on evaluate
            //if the item coming in is different
            if (element == null || element.Id.Equals(this.storedId))
                return SelectionState.Restore;

            storedId = element.Id;
            Items.Clear();
            
            AddElementParams(element);

            Items = Items.OrderBy(x => x.Name).Distinct(new DropDownItemEqualityComparer()).ToObservableCollection<DynamoDropDownItem>();

            return SelectionState.Restore;
        }

        private void AddElementParams(Element e)
        {
            foreach (Parameter p in e.Parameters)
            {
                if (!(p.StorageType == StorageType.None))
                {
                    AddDropDownItem(p);
                }
            }
            // if element can have type assigned it's safe to assume that it's an instance
            // and add type parameters to the list
            if (e.CanHaveTypeAssigned())
            {
                ElementType et = DocumentManager.Instance.CurrentDBDocument.GetElement(e.GetTypeId()) as ElementType;
                if (et != null)
                {
                    AddTypeParams(et);
                }
            }
        }

        private void AddTypeParams(ElementType et)
        {
            foreach (Parameter p in et.Parameters)
            {
                if (p.StorageType == StorageType.None)
                    continue;

                AddDropDownItem(p);
            }
        }

        private void AddDropDownItem(Parameter p)
        {
            Items.Add(
                new DynamoDropDownItem(
                    string.Format("{0}(Type)({1})", p.Definition.Name, getStorageTypeString(p.StorageType)), p.Definition.Name));
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 ||
                Items[0].Name == noFamilyParameters ||
                SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            return new[] {AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildStringNode((string)Items[SelectedIndex].Item)) };
        }

        private Element GetInputElement()
        {
            var inputNode = InPorts[0].Connectors[0].Start.Owner;
            var index = InPorts[0].Connectors[0].Start.Index;
            
            var identifier = inputNode.GetAstIdentifierForOutputIndex(index).Name;

            var data = inputNode.CachedValue; //This may not represent the correct value for multiple output ports.
            //If EngineController is set, find the real mirrordata. In headless 
            //mode it may not be set, because it is set from NodeView customization.
            if (EngineController != null) 
            {
                data = this.EngineController.GetMirror(identifier).GetData();
            }
            if (data == null) return null;

            object family = data.IsCollection ? 
                data.GetElements().FirstOrDefault() : 
                data.Data;

            var elem = family as Revit.Elements.Element;

            return null == elem ? null : elem.InternalElement;
        }

        protected override void SerializeCore(XmlElement nodeElement, SaveContext context)
        {
            base.SerializeCore(nodeElement, context);
            if (this.storedId != null)
            {
                XmlElement outEl = nodeElement.OwnerDocument.CreateElement("familyid");
                outEl.SetAttribute("value", this.storedId.IntegerValue.ToString(CultureInfo.InvariantCulture));
                nodeElement.AppendChild(outEl);

                XmlElement param = nodeElement.OwnerDocument.CreateElement("index");
                param.SetAttribute("value", SelectedIndex.ToString(CultureInfo.InvariantCulture));
                nodeElement.AppendChild(param);
            }

        }

        protected override void DeserializeCore(XmlElement nodeElement, SaveContext context)
        {
            base.DeserializeCore(nodeElement, context);
            var doc = DocumentManager.Instance.CurrentDBDocument;

            int index = -1;

            foreach (XmlNode subNode in nodeElement.ChildNodes)
            {
                if (subNode.Name.Equals("familyid"))
                {
                    int id;
                    try
                    {
                        id = Convert.ToInt32(subNode.Attributes[0].Value);
                    }
                    catch
                    {
                        continue;
                    }
                    element = doc.GetElement(new ElementId(id));

                }
                else if (subNode.Name.Equals("index"))
                {
                    try
                    {
                        index = Convert.ToInt32(subNode.Attributes[0].Value);
                    }
                    catch
                    {
                    }
                }
            }

            if (element != null)
            {
                PopulateItems();
                SelectedIndex = index;
            }
        }
    }

    [NodeName("Floor Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("FloorTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class FloorTypes : RevitDropDownBase
    {
        

        public FloorTypes() : base("Floor Type") { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(Autodesk.Revit.DB.FloorType));
            var elements = fec.ToElements();
            if (!elements.Any())
            {
                Items.Add(new DynamoDropDownItem(Properties.Resources.NoFloorTypesAvailable, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            Items = elements.Select(x => new DynamoDropDownItem(x.Name, x)).OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 || 
                Items[0].Name == Properties.Resources.NoFloorTypesAvailable ||
                SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var args = new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(((Autodesk.Revit.DB.FloorType) Items[SelectedIndex].Item).Name)
            };

            var functionCall = AstFactory.BuildFunctionCall
                <System.String, Revit.Elements.FloorType>
                (Revit.Elements.FloorType.ByName, args);

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }

    [NodeName("Wall Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("WallTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class WallTypes : RevitDropDownBase
    {
 

        public WallTypes() : base("Wall Type") { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            fec.OfClass(typeof(Autodesk.Revit.DB.WallType));
            var elements = fec.ToElements();
            if (!elements.Any())
            {
                Items.Add(new DynamoDropDownItem(Properties.Resources.NoWallTypesAvailable, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            Items = elements.Select(x => new DynamoDropDownItem(x.Name, x)).OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 ||
                Items[0].Name == Properties.Resources.NoWallTypesAvailable ||
                SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var args = new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(((Autodesk.Revit.DB.WallType) Items[SelectedIndex].Item).Name)
            };
            var functionCall = AstFactory.BuildFunctionCall("Revit.Elements.WallType",
                                                            "ByName",
                                                            args);

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }

    [NodeName("Performance Adviser Rules")]
    [NodeCategory(BuiltinNodeCategories.REVIT_ELEMENTS_PERFORMANCEADVISER)]
    [NodeDescription("PerformanceAdviserDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class PerformanceAdviserRules : RevitDropDownBase
    {
        public PerformanceAdviserRules() : base("Performance Adviser Rules") { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            PerformanceAdviser adviser = PerformanceAdviser.GetPerformanceAdviser();
            IList<PerformanceAdviserRuleId> ruleIds = adviser.GetAllRuleIds();
            string ruleInfo = string.Empty;

            List<Revit.Elements.PerformanceAdviserRule> elements = new List<Revit.Elements.PerformanceAdviserRule>();
            foreach (PerformanceAdviserRuleId ruleId in ruleIds)
            {
                elements.Add(new Revit.Elements.PerformanceAdviserRule(ruleId));
            }

            if (!elements.Any())
            {
                Items.Add(new DynamoDropDownItem(Properties.Resources.NoWallTypesAvailable, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            Items = elements.Select(x => new DynamoDropDownItem(x.Name, x)).OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 ||
                Items[0].Name == Properties.Resources.NoWallTypesAvailable ||
                SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var args = new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(((Revit.Elements.PerformanceAdviserRule) Items[SelectedIndex].Item).RuleId.ToString())
            };
            var functionCall = AstFactory.BuildFunctionCall("Revit.Elements.PerformanceAdviserRule",
                                                            "ById",
                                                            args);

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }


    [NodeName("Categories")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("CategoriesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class Categories : EnumBase<BuiltInCategory>
    {
        public Categories()
        {
            OutPortData[0].NickName = "Category";
            OutPortData[0].ToolTipString = Properties.Resources.PortDataCategoriesToolTip;
            OutPorts[0].SetPortData(OutPortData[0]);
        }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();
            var document = DocumentManager.Instance.CurrentDBDocument;

            foreach (BuiltInCategory categoryId in Enum.GetValues(typeof(BuiltInCategory)))
            {
                Autodesk.Revit.DB.Category category;
                
                try 
                {
                    category = Autodesk.Revit.DB.Category.GetCategory(document, categoryId);
                }
                catch
                {
                    // We get here for internal/deprecated categories
                    continue;
                }

                if (category != null)
                {
                    string name = getFullName(category);
                    Items.Add(new DynamoDropDownItem(name, categoryId));
                }
            }

            Items = Items.OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            //Some of the legacy categories which were not working before will now be out of index.
            if (SelectedIndex < 0 || SelectedIndex >= Items.Count)
                return new[] { AstFactory.BuildNullNode() };

            BuiltInCategory categoryId = (BuiltInCategory)Items[SelectedIndex].Item;

            var args = new List<AssociativeNode>
            {
                AstFactory.BuildIntNode((int)categoryId)
            };

            var func = new Func<int, Category>(Revit.Elements.Category.ById);
            var functionCall = AstFactory.BuildFunctionCall(func, args);

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }

        protected override int ParseSelectedIndex(string index, IList<DynamoDropDownItem> items)
        {
            int selectedIndex = -1;

            var splits = index.Split(':');
            if (splits.Count() > 1)
            {
                var name = XmlUnescape(index.Substring(index.IndexOf(':') + 1));

                // Lookup by built in category enum name (without the OST_ prefix)
                var item = items.FirstOrDefault(i => ((BuiltInCategory)i.Item).ToString().Substring(4) == name);
                selectedIndex = item != null ?
                    items.IndexOf(item) :
                    -1;
            }


            return selectedIndex;
        }

        protected override string SaveSelectedIndex(int index, IList<DynamoDropDownItem> items)
        {
            // If nothing is selected or there are no
            // items in the collection, than return -1
            if (index == -1 || items.Count == 0)
            {
                return "-1";
            }

            // Save the enum name for the category (without the OST_ prefix)
            var item = items[index];
            BuiltInCategory categoryId = (BuiltInCategory)item.Item;
            return string.Format("{0}:{1}", index, XmlEscape(categoryId.ToString().Substring(4)));
        }

        private static string getFullName(Autodesk.Revit.DB.Category category)
        {
            string name = string.Empty;

            if(category != null)
            {
                var parent = category.Parent;
                if (parent == null)
                {
                    // Top level category
                    // For example "Cable Trays"
                    name = category.Name.ToString();
                }
                else
                {
                    // Sub-category
                    // For example "Cable Tray - Center Lines"
                    name = parent.Name.ToString() + " - " + category.Name.ToString();
                }
            }

            return name;
        }
    }

    [NodeName("Levels")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("LevelsDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class Levels : RevitDropDownBase
    {
        private const string noLevels = "No levels available.";

        public Levels() : base("Levels") { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            //find all levels in the project
            var levelColl = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            levelColl.OfClass(typeof(Level));
            var elements = levelColl.ToElements();
            if (!elements.Any())
            {
                Items.Add(new DynamoDropDownItem(noLevels, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            Items = elements.Select(x => new DynamoDropDownItem(x.Name, x)).OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 ||
                Items[0].Name == noLevels ||
                SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var node = AstFactory.BuildFunctionCall(
                "Revit.Elements.ElementSelector",
                "ByElementId",
                new List<AssociativeNode>
                {
                    AstFactory.BuildIntNode(((Level)Items[SelectedIndex].Item).Id.IntegerValue)
                });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }
    }

    public abstract class AllElementsInBuiltInCategory : RevitDropDownBase 
    {
        private string noTypesMessage;
        private BuiltInCategory category;

        internal AllElementsInBuiltInCategory(
            BuiltInCategory category, string outputMessage, string noTypesMessage) : base(outputMessage)
        {
            this.category = category;
            this.noTypesMessage = noTypesMessage;
            PopulateItems();
        }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            //find all the structural framing family types in the project
            var collector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            var catFilter = new ElementCategoryFilter(category);
            collector.OfClass(typeof(FamilySymbol)).WherePasses(catFilter);
            var elements = collector.ToElements();
            if (!elements.Any())
            {
                Items.Add(new DynamoDropDownItem(noTypesMessage, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            Items = elements.Select(x => new DynamoDropDownItem(x.Name, x)).OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 ||
                Items[0].Name == noTypesMessage ||
                SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var node = AstFactory.BuildFunctionCall(
                "Revit.Elements.ElementSelector",
                "ByElementId",
                new List<AssociativeNode>
                {
                    AstFactory.BuildIntNode(((Element)Items[SelectedIndex].Item).Id.IntegerValue)
                });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }
    }

    [NodeName("Structural Framing Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("StructuralFramingTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class StructuralFramingTypes : AllElementsInBuiltInCategory
    {
        public StructuralFramingTypes()
            : base(BuiltInCategory.OST_StructuralFraming, "Framing Types", Properties.Resources.DropDownNoFramingType){}
    }

    [NodeName("Structural Column Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("StructuralColumnTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class StructuralColumnTypes : AllElementsInBuiltInCategory
    {
        public StructuralColumnTypes()
            : base(BuiltInCategory.OST_StructuralColumns, "Column Types", Properties.Resources.DropDownNoColumnType){}
    }

    [NodeName("Spacing Rule Layout")]
    [NodeCategory(BuiltinNodeCategories.REVIT_ELEMENTS_DIVIDEDPATH_ACTION)]
    [NodeDescription("SpacingRuleLayoutDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class SpacingRuleLayouts : EnumAsInt<SpacingRuleLayout> {
    }

    [NodeName("Element Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("ElementTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class ElementTypes : AllChildrenOfType<Element>
    {
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;
            if(SelectedIndex < 0 || SelectedIndex >= Items.Count)
            {
                node = AstFactory.BuildNullNode();
            }
            else
            {
               var typeName = AstFactory.BuildStringNode(Items[SelectedIndex].Name);
               var assemblyName = AstFactory.BuildStringNode("RevitAPI");

               node =
                     AstFactory.BuildFunctionCall(
                        new Func<string, string, object>(Types.FindTypeByNameInAssembly),
                        new List<AssociativeNode>() { typeName, assemblyName });
            }
            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }
    }
    
    [NodeName("Views")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("ViewsDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class Views : RevitDropDownBase
    {
        public Views() : base("Views") { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();
            //find all views in the project
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            var views = fec.OfClass(typeof(View)).ToElements();
            
            //there must always be at least 1 view in a Revit document, so we can exclude the empty list check
            foreach (var v in views)
            {
                Items.Add(new DynamoDropDownItem(v.Name, v));
            }
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;

            if (SelectedIndex == -1)
            {
                node = AstFactory.BuildNullNode();
            }
            else
            {
                var view = Items[SelectedIndex].Item as View;
                if (view == null)
                {
                    node = AstFactory.BuildNullNode();
                }
                else
                {
                    var idNode = AstFactory.BuildStringNode(view.UniqueId);
                    var falseNode = AstFactory.BuildBooleanNode(true);
                    
                    node =
                        AstFactory.BuildFunctionCall(
                            new Func<string, bool, object>(ElementSelector.ByUniqueId),
                            new List<AssociativeNode>() { idNode, falseNode });
                }
            }

            return new []{AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node)};
        }
    }

        
}
