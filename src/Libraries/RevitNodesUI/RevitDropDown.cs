using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
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

        [JsonConstructor]
        public RevitDropDownBase(string value, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(value, inPorts, outPorts)
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

        /// <summary>
        /// whether it have valid Enumeration values to the output
        /// </summary>
        /// <param name="itemValueToIgnore"></param>
        /// <param name="selectedValueToIgnore"></param>
        /// <returns>true is that there are valid values to output,false is that only a null value to output</returns>
        public Boolean CanBuildOutputAst(string itemValueToIgnore = null, string selectedValueToIgnore = null)
        {
            if(Items.Count == 0 || SelectedIndex < 0)
                return false;
            if (!string.IsNullOrEmpty(itemValueToIgnore) && Items[0].Name == itemValueToIgnore) 
                return false;
            if (!string.IsNullOrEmpty(selectedValueToIgnore) && Items[SelectedIndex].Name == selectedValueToIgnore)
                return false;
            return true;
        }
    }

    [NodeName("Family Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("FamilyTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class FamilyTypes : RevitDropDownBase
    {
        private const string NO_FAMILY_TYPES = "No family types available.";
        private const string outputName = "Family Type";

        public FamilyTypes() : base(outputName) { }

        [JsonConstructor]
        public FamilyTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
        {
        }

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
            if(!CanBuildOutputAst(NO_FAMILY_TYPES))
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            
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
        private const string outputName = "Parameter";
        private Element element;
        private ElementId storedId = null;
        internal EngineController EngineController { get; set; }

        public FamilyInstanceParameters()
            : base(outputName) 
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("f", Properties.Resources.PortDataFamilySymbolToolTip)));
            PropertyChanged += OnPropertyChanged;
        }

        [JsonConstructor]
        public FamilyInstanceParameters(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
        {
            PropertyChanged += OnPropertyChanged;
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
                Autodesk.Revit.DB.ElementType et = DocumentManager.Instance.CurrentDBDocument.GetElement(e.GetTypeId()) as Autodesk.Revit.DB.ElementType;
                if (et != null)
                {
                    AddTypeParams(et);
                }
            }
        }

        private void AddTypeParams(Autodesk.Revit.DB.ElementType et)
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
            if(!CanBuildOutputAst(noFamilyParameters))
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            
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
        private const string outputName = "Floor Type";

        public FloorTypes() : base(outputName) { }

        [JsonConstructor]
        public FloorTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts) { }

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
            if(!CanBuildOutputAst(Properties.Resources.NoFloorTypesAvailable))
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            
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
        private const string outputName = "Wall Type";

        public WallTypes() : base(outputName) { }

        [JsonConstructor]
        public WallTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts) { }

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
            if(!CanBuildOutputAst(Properties.Resources.NoWallTypesAvailable))
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            
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
        private const string outputName = "Performance Adviser Rules";

        public PerformanceAdviserRules() : base(outputName) { }

        [JsonConstructor]
        public PerformanceAdviserRules(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts) { }

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
            if(!CanBuildOutputAst(Properties.Resources.NoWallTypesAvailable))
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            
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
            var existing = OutPorts[0];
            OutPorts[0] = new PortModel(PortType.Output, this, 
                new PortData("Category", Properties.Resources.PortDataCategoriesToolTip, existing.DefaultValue));
            OutPorts[0].GUID = existing.GUID;
        }

        [JsonConstructor]
        public Categories(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            // TODO verify additional information for output ports is not required here!
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

        /// <summary>
        /// Override the default behavior to serialize internal category id 
        /// instead of category name.
        /// </summary>
        /// <param name="item">Selected DynamoDropDownItem</param>
        /// <returns></returns>
        protected override string GetSelectedStringFromItem(DynamoDropDownItem item)
        {
            return item == null ? string.Empty : item.Item.ToString();
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

        protected override bool UpdateValueCore(UpdateValueParams updateValueParams)
        {
            string name = updateValueParams.PropertyName;
            string value = updateValueParams.PropertyValue;

            if (name == "Value" && value != null)
            {
                // Un-exception: Find selection by display name, just like the base class does!
                SelectedIndex = ParseSelectedIndexImpl(value, Items);
                if (SelectedIndex < 0)
                    Warning(Dynamo.Properties.Resources.NothingIsSelectedWarning);
                return true; // UpdateValueCore handled.
            }

            return base.UpdateValueCore(updateValueParams);
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
        private const string outputName = "Levels";

        public Levels() : base(outputName) { }

        [JsonConstructor]
        public Levels(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts) { }

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
            if(!CanBuildOutputAst(noLevels))
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            
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

        [JsonConstructor]
        internal AllElementsInBuiltInCategory(
            BuiltInCategory category, string outputMessage, string noTypesMessage,
            IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputMessage, inPorts, outPorts)
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
            if(!CanBuildOutputAst(noTypesMessage))
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            
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
        private const string outputName = "Framing Types";

        public StructuralFramingTypes()
            : base(BuiltInCategory.OST_StructuralFraming, outputName, Properties.Resources.DropDownNoFramingType){}

        [JsonConstructor]
        public StructuralFramingTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(BuiltInCategory.OST_StructuralFraming, outputName, Properties.Resources.DropDownNoFramingType, inPorts, outPorts) { }
    }

    [NodeName("Structural Column Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("StructuralColumnTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class StructuralColumnTypes : AllElementsInBuiltInCategory
    {
        private const string outputName = "Column Types";

        public StructuralColumnTypes()
            : base(BuiltInCategory.OST_StructuralColumns, outputName, Properties.Resources.DropDownNoColumnType){}

        [JsonConstructor]
        public StructuralColumnTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(BuiltInCategory.OST_StructuralColumns, outputName, Properties.Resources.DropDownNoColumnType, inPorts, outPorts) { }
    }

    [NodeName("Spacing Rule Layout")]
    [NodeCategory(BuiltinNodeCategories.REVIT_ELEMENTS_DIVIDEDPATH_ACTION)]
    [NodeDescription("SpacingRuleLayoutDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class SpacingRuleLayouts : EnumAsInt<SpacingRuleLayout>
    {
        public SpacingRuleLayouts() { }

        [JsonConstructor]
        public SpacingRuleLayouts(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }
    }

    [NodeName("Element Types")]
    [NodeCategory(BuiltinNodeCategories.REVIT_SELECTION)]
    [NodeDescription("ElementTypesDescription", typeof(Properties.Resources))]
    [IsDesignScriptCompatible]
    public class ElementTypes : AllChildrenOfType<Element>
    {
        public ElementTypes() { }

        [JsonConstructor]
        public ElementTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

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
        private const string outputName = "Views";

        public Views() : base(outputName) { }

        [JsonConstructor]
        public Views(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            //find all views in the project
            //exclude <RevisionSchedule> (revision tables on sheets) from list
            var views = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(View))
                .Where(x => !x.Name.Contains('<'))
                .ToList();
            
            //there must always be at least 1 view in a Revit document, so we can exclude the empty list check
            foreach (var v in views)
            {
                Items.Add(new DynamoDropDownItem(v.Name, v));
            }
            Items = Items.OrderBy(x => x.Name).ToObservableCollection();

            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            AssociativeNode node;
            if (!CanBuildOutputAst())
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
