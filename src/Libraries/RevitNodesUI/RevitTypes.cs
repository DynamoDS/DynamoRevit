using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using DSRevitNodesUI;
using RVT = Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;

using Dynamo.Utilities;
using Dynamo.Models;
using Dynamo.Nodes;
using ProtoCore.AST.AssociativeAST;
using CoreNodeModels.Properties;
using Dynamo.Graph.Nodes;

namespace DSRevitNodesUI
{

    [NodeName("Select Phase")]
    [NodeCategory("Revit.Elements.Phase")]
    [NodeDescription("PhaseSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.Phase")]
    public class RevitPhases : CustomRevitElementDropDown
    {
        private const string outputName = "Phase";

        public RevitPhases() : base(outputName, typeof(Autodesk.Revit.DB.Phase)) { }

        [JsonConstructor]
        public RevitPhases(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.Phase), inPorts, outPorts) { }
    }

    [NodeName("Select Revision")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionSelectorDescription",typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.Revision")]
    public class RevitRevisions : CustomRevitElementDropDown
    {
        private const string outputName = "Revision";

        public RevitRevisions() : base(outputName, typeof(Autodesk.Revit.DB.Revision)) { }

        [JsonConstructor]
        public RevitRevisions(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.Revision), inPorts, outPorts) { }
    }

    [NodeName("Select Filled Region Type")]
    [NodeCategory("Revit.Elements.FilledRegionType")]
    [NodeDescription("FilledRegionTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.FilledRegionType")]
    public class FilledRegionTypes : CustomRevitElementDropDown
    {
        private const string outputName = "FilledRegionType";

        public FilledRegionTypes() : base(outputName, typeof(Autodesk.Revit.DB.FilledRegionType)) { }

        [JsonConstructor]
        public FilledRegionTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.FilledRegionType), inPorts, outPorts) { }
    }

    [NodeName("Select Rule Type")]
    [NodeCategory("Revit.Filter.RuleType")]
    [NodeDescription("FilterTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class RuleTypes : CustomGenericEnumerationDropDown
    {
        private const string outputName = "RuleType";

        public RuleTypes() : base(outputName, typeof(Revit.Filter.FilterRule.RuleType)) { }

        [JsonConstructor]
        public RuleTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Revit.Filter.FilterRule.RuleType), inPorts, outPorts) { }
    }

    [NodeName("Select Revision Numbering")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionNumberingSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class RevisionNumbering : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Revision Numbering";

        public RevisionNumbering() : base(outputName, typeof(Autodesk.Revit.DB.RevisionNumbering)) { }

        [JsonConstructor]
        public RevisionNumbering(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.RevisionNumbering), inPorts, outPorts) { }
    }

    [NodeName("Select Revision Number Type")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionNumberTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class RevisionNumberType : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Revision Number Type";

        public RevisionNumberType() : base(outputName, typeof(Autodesk.Revit.DB.RevisionNumberType)) { }

        [JsonConstructor]
        public RevisionNumberType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.RevisionNumberType), inPorts, outPorts) { }
    }
    
    [NodeName("Spec Types")]
    [NodeCategory("Revit.Elements.Parameter")]
    [NodeDescription("SpecTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.SpecType")]
    public class SpecTypes : CustomForgeTypeIdDropDown
    {
        private const string outputName = "SpecType";

        public SpecTypes() : base(outputName, typeof(Autodesk.Revit.DB.SpecTypeId)) { }

        [JsonConstructor]
        public SpecTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(outputName, typeof(Autodesk.Revit.DB.SpecTypeId), inPorts, outPorts) { }

        public override AssociativeNode GetAstFunction(List<AssociativeNode> args)
        {
            return AstFactory.BuildFunctionCall<String, Revit.Elements.SpecType>(Revit.Elements.SpecType.ByTypeId, args);
        }
    }

    [NodeName("Group Types")]
    [NodeCategory("Revit.Elements.Parameter")]
    [NodeDescription("GroupTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.GroupType")]
    public class GroupTypes : CustomForgeTypeIdDropDown
    {
        private const string outputName = "GroupType";

        public GroupTypes() : base(outputName, typeof(Autodesk.Revit.DB.GroupTypeId)) { }

        [JsonConstructor]
        public GroupTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(outputName, typeof(Autodesk.Revit.DB.GroupTypeId), inPorts, outPorts) { }

        public override AssociativeNode GetAstFunction(List<AssociativeNode> args)
        {
            return AstFactory.BuildFunctionCall<String, Revit.Elements.GroupType>(Revit.Elements.GroupType.ByTypeId, args);
        }
    }

    [NodeName("Select Revision Visibility")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionVisibilitySelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class RevisionVisibility : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Revision Visibility";

        public RevisionVisibility() : base(outputName, typeof(Autodesk.Revit.DB.RevisionVisibility)) { }

        [JsonConstructor]
        public RevisionVisibility(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.RevisionVisibility), inPorts, outPorts) { }
    }

    [NodeName("Select Direct Shape Room Bounding Option")]
    [NodeCategory("Revit.Elements.DirectShape")]
    [NodeDescription("DirectShapeRoomBoundingOptionSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class DirectShapeRoomBoundingOption : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Direct Shape Room Bounding Option";

        public DirectShapeRoomBoundingOption() : base(outputName, typeof(Autodesk.Revit.DB.DirectShapeRoomBoundingOption)) { }

        [JsonConstructor]
        public DirectShapeRoomBoundingOption(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.DirectShapeRoomBoundingOption), inPorts, outPorts) { }
    }

    [NodeName("Detail Level")]
    [NodeCategory("Revit.Filter.OverrideGraphicSettings")]
    [NodeDescription("ViewDetailLevelDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class DetailLevel : CustomGenericEnumerationDropDown
    {
        private const string outputName = "detailLevel";

        public DetailLevel() : base(outputName, typeof(Autodesk.Revit.DB.ViewDetailLevel)) { }

        [JsonConstructor]
        public DetailLevel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.ViewDetailLevel), inPorts, outPorts) { }
    }

    [NodeName("Select Horizontal Text Alignment")]
    [NodeCategory("Revit.Elements.Text")]
    [NodeDescription("HorizontalTextAlignmentStyleSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class HorizontalAlignment : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Horizontal Alignment";

        public HorizontalAlignment() : base(outputName, typeof(Autodesk.Revit.DB.HorizontalAlignmentStyle)) { }

        [JsonConstructor]
        public HorizontalAlignment(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.HorizontalAlignmentStyle), inPorts, outPorts) { }
    }

    [NodeName("Select Vertical Text Alignment")]
    [NodeCategory("Revit.Elements.Text")]
    [NodeDescription("VerticalTextAlignmentStyleSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class VerticalAlignment : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Vertical Alignment";

        public VerticalAlignment() : base(outputName, typeof(Autodesk.Revit.DB.VerticalAlignmentStyle)) { }

        [JsonConstructor]
        public VerticalAlignment(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.VerticalAlignmentStyle), inPorts, outPorts) { }
    }
    
    [NodeName("Wall Location")]
    [NodeCategory("Revit.Elements.Wall")]
    [NodeDescription("WallLocationLineDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class WallLocation : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Wall Location";

        public WallLocation() : base(outputName, typeof(Autodesk.Revit.DB.WallLocationLine)) { }

        [JsonConstructor]
        public WallLocation(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.WallLocationLine), inPorts, outPorts) { }
    }


    [NodeName("Schedule Type")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ScheduleTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class ScheduleTypes : CustomGenericEnumerationDropDown
    {
        private const string outputName = "ScheduleType";

        public ScheduleTypes() : base(outputName, typeof(Revit.Elements.Views.ScheduleView.ScheduleType)) { }

        [JsonConstructor]
        public ScheduleTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Revit.Elements.Views.ScheduleView.ScheduleType), inPorts, outPorts) { }
    }

    [NodeName("Export Column Headers")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ExportColumnHeadersDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class ExportColumnHeaders : CustomGenericEnumerationDropDown
    {
        private const string outputName = "ColumnHeaders";

        public ExportColumnHeaders() : base(outputName, typeof(Revit.Schedules.ScheduleExportOptions.ExportColumnHeaders)) { }

        [JsonConstructor]
        public ExportColumnHeaders(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Revit.Schedules.ScheduleExportOptions.ExportColumnHeaders), inPorts, outPorts) { }
    }

    [NodeName("Export Text Qualifier")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ExportTextQualifierDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class ExportTextQualifier : CustomGenericEnumerationDropDown
    {
        private const string outputName = "TextQualifier";

        public ExportTextQualifier() : base(outputName, typeof(Revit.Schedules.ScheduleExportOptions.ExportTextQualifier)) { }

        [JsonConstructor]
        public ExportTextQualifier(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Revit.Schedules.ScheduleExportOptions.ExportTextQualifier), inPorts, outPorts) { }
    }

    [NodeName("Fill Patterns")]
    [NodeCategory("Revit.Elements.FillPatternElement")]
    [NodeDescription("FillPatternsDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.FillPatternElement")]
    public class FillPatterns : CustomRevitElementDropDown
    {
        private const string outputName = "FillPattern";

        public FillPatterns() : base(outputName, typeof(Autodesk.Revit.DB.FillPatternElement)) { }

        [JsonConstructor]
        public FillPatterns(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.FillPatternElement), inPorts, outPorts) { }
    }

    [NodeName("Fill Pattern Targets")]
    [NodeCategory("Revit.Elements.FillPatternElement")]
    [NodeDescription("FillPatternTargetDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class FillPatternTargets : CustomGenericEnumerationDropDown
    {
        private const string outputName = "FillPatternTarget";

        public FillPatternTargets() : base(outputName, typeof(Autodesk.Revit.DB.FillPatternTarget)) { }

        [JsonConstructor]
        public FillPatternTargets(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.FillPatternTarget), inPorts, outPorts) { }
    }

    [NodeName("Line Patterns")]
    [NodeCategory("Revit.Elements.LinePatternElement")]
    [NodeDescription("LinePatternsDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("Revit.Elements.LinePatternElement")]
    public class LinePatterns : CustomRevitElementDropDown
    {
        private const string outputName = "LinePattern";

        public LinePatterns() : base(outputName, typeof(Autodesk.Revit.DB.LinePatternElement)) { }

        [JsonConstructor]
        public LinePatterns(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.LinePatternElement), inPorts, outPorts) { }
    }

    [NodeName("Schedule Filter Type")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ScheduleFilterTypeDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class ScheduleFilterType : CustomGenericEnumerationDropDown
    {
        private const string outputName = "FilterType";

        public ScheduleFilterType() : base(outputName, typeof(Autodesk.Revit.DB.ScheduleFilterType)) { }

        [JsonConstructor]
        public ScheduleFilterType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.ScheduleFilterType), inPorts, outPorts) { }
    }

    [NodeName("View Duplicate Options")]
    [NodeCategory("Revit.View")]
    [NodeDescription("ViewDuplicateOptionsDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class ViewDuplicateOptions : CustomGenericEnumerationDropDown
    {
        private const string outputName = "DuplicateOption";

        public ViewDuplicateOptions() : base(outputName, typeof(Autodesk.Revit.DB.ViewDuplicateOption)) { }

        [JsonConstructor]
        public ViewDuplicateOptions(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(outputName, typeof(Autodesk.Revit.DB.ViewDuplicateOption), inPorts, outPorts) { }
    }

    [NodeName("View Disciplines")]
    [NodeCategory("Revit.View")]
    [NodeDescription("ViewDisciplinesDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class ViewDisciplines : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Discipline";

        public ViewDisciplines() : base(outputName, typeof(Autodesk.Revit.DB.ViewDiscipline)) { }

        [JsonConstructor]
        public ViewDisciplines(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(outputName, typeof(Autodesk.Revit.DB.ViewDiscipline), inPorts, outPorts) { }
    }

    [NodeName("View DisplayStyles")]
    [NodeCategory("Revit.View")]
    [NodeDescription("ViewDisplayStylesDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class ViewDisplayStyles : CustomGenericEnumerationDropDown
    {
        private const string outputName = "DisplayStyle";

        public ViewDisplayStyles() : base(outputName, typeof(Autodesk.Revit.DB.DisplayStyle)) { }

        [JsonConstructor]
        public ViewDisplayStyles(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(outputName, typeof(Autodesk.Revit.DB.DisplayStyle), inPorts, outPorts) { }
    }

    [NodeName("View Parts Visibilities")]
    [NodeCategory("Revit.View")]
    [NodeDescription("ViewPartsVisibilitysDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class ViewPartsVisibilitys : CustomGenericEnumerationDropDown
    {
        private const string outputName = "PartsVisibility";

        public ViewPartsVisibilitys() : base(outputName, typeof(Autodesk.Revit.DB.PartsVisibility)) { }

        [JsonConstructor]
        public ViewPartsVisibilitys(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(outputName, typeof(Autodesk.Revit.DB.PartsVisibility), inPorts, outPorts) { }
    }

    [NodeName("LeaderEnd Condition")]
    [NodeCategory("Revit.Elements.Tag")]
    [NodeDescription("LeaderEndConditionDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("string")]
    public class LeaderEndCondition : CustomGenericEnumerationDropDown
    {
        private const string outputName = "LeaderEndCondition";

        public LeaderEndCondition() : base(outputName, typeof(Autodesk.Revit.DB.LeaderEndCondition)) { }

        [JsonConstructor]
        public LeaderEndCondition(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(outputName, typeof(Autodesk.Revit.DB.LeaderEndCondition), inPorts, outPorts) { }
    }
}