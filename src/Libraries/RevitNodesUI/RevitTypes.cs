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
    public class RevitPhases : CustomRevitElementDropDown
    {
        public RevitPhases() : base("Phase", typeof(Autodesk.Revit.DB.Phase)) { }

        [JsonConstructor]
        public RevitPhases(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Phase", typeof(Autodesk.Revit.DB.Phase), inPorts, outPorts) { }
    }

    [NodeName("Select Revision")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionSelectorDescription",typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class RevitRevisions : CustomRevitElementDropDown
    {
        public RevitRevisions() : base("Revision", typeof(Autodesk.Revit.DB.Revision)) { }

        [JsonConstructor]
        public RevitRevisions(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Revision", typeof(Autodesk.Revit.DB.Revision), inPorts, outPorts) { }
    }

    [NodeName("Select Filled Region Type")]
    [NodeCategory("Revit.Elements.FilledRegionType")]
    [NodeDescription("FilledRegionTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class FilledRegionTypes : CustomRevitElementDropDown
    {
        public FilledRegionTypes() : base("FilledRegionType", typeof(Autodesk.Revit.DB.FilledRegionType)) { }

        [JsonConstructor]
        public FilledRegionTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("FilledRegionType", typeof(Autodesk.Revit.DB.FilledRegionType), inPorts, outPorts) { }
    }

    [NodeName("Select Rule Type")]
    [NodeCategory("Revit.Filter.RuleType")]
    [NodeDescription("FilterTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class RuleTypes : CustomGenericEnumerationDropDown
    {
        public RuleTypes() : base("RuleType", typeof(Revit.Filter.FilterRule.RuleType)) { }

        [JsonConstructor]
        public RuleTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("RuleType", typeof(Revit.Filter.FilterRule.RuleType), inPorts, outPorts) { }
    }

    [NodeName("Select Revision Numbering")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionNumberingSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class RevisionNumbering : CustomGenericEnumerationDropDown
    {
        public RevisionNumbering() : base("Revision Numbering", typeof(Autodesk.Revit.DB.RevisionNumbering)) { }

        [JsonConstructor]
        public RevisionNumbering(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Revision Numbering", typeof(Autodesk.Revit.DB.RevisionNumbering), inPorts, outPorts) { }
    }

    [NodeName("Select Revision Number Type")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionNumberTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class RevisionNumberType : CustomGenericEnumerationDropDown
    {
        public RevisionNumberType() : base("Revision Number Type", typeof(Autodesk.Revit.DB.RevisionNumberType)) { }

        [JsonConstructor]
        public RevisionNumberType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Revision Number Type", typeof(Autodesk.Revit.DB.RevisionNumberType), inPorts, outPorts) { }
    }


    [NodeName("Select Parameter Type")]
    [NodeCategory("Revit.Elements.Parameter")]
    [NodeDescription("ParameterTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class ParameterType : CustomGenericEnumerationDropDown
    {
        public ParameterType() : base("Parameter Type", typeof(Autodesk.Revit.DB.ParameterType)) { }

        [JsonConstructor]
        public ParameterType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Parameter Type", typeof(Autodesk.Revit.DB.ParameterType), inPorts, outPorts) { }
    }

    [NodeName("Select BuiltIn Parameter Group")]
    [NodeCategory("Revit.Elements.Parameter")]
    [NodeDescription("BuiltInParameterGroupSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class BuiltInParameterGroup : CustomGenericEnumerationDropDown
    {
        public BuiltInParameterGroup() : base("BuiltIn Parameter Group", typeof(Autodesk.Revit.DB.BuiltInParameterGroup)) { }

        [JsonConstructor]
        public BuiltInParameterGroup(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("BuiltIn Parameter Group", typeof(Autodesk.Revit.DB.BuiltInParameterGroup), inPorts, outPorts) { }
    }

    [NodeName("Select Revision Visibility")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionVisibilitySelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class RevisionVisibility : CustomGenericEnumerationDropDown
    {
        public RevisionVisibility() : base("Revision Visibility", typeof(Autodesk.Revit.DB.RevisionVisibility)) { }

        [JsonConstructor]
        public RevisionVisibility(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Revision Visibility", typeof(Autodesk.Revit.DB.RevisionVisibility), inPorts, outPorts) { }
    }

    [NodeName("Select Direct Shape Room Bounding Option")]
    [NodeCategory("Revit.Elements.DirectShape")]
    [NodeDescription("DirectShapeRoomBoundingOptionSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class DirectShapeRoomBoundingOption : CustomGenericEnumerationDropDown
    {
        public DirectShapeRoomBoundingOption() : base("Direct Shape Room Bounding Option", typeof(Autodesk.Revit.DB.DirectShapeRoomBoundingOption)) { }

        [JsonConstructor]
        public DirectShapeRoomBoundingOption(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Direct Shape Room Bounding Option", typeof(Autodesk.Revit.DB.DirectShapeRoomBoundingOption), inPorts, outPorts) { }
    }

    [NodeName("Detail Level")]
    [NodeCategory("Revit.Filter.OverrideGraphicSettings")]
    [NodeDescription("ViewDetailLevelDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class DetailLevel : CustomGenericEnumerationDropDown
    {
        public DetailLevel() : base("Detail Level", typeof(Autodesk.Revit.DB.ViewDetailLevel)) { }

        [JsonConstructor]
        public DetailLevel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Detail Level", typeof(Autodesk.Revit.DB.ViewDetailLevel), inPorts, outPorts) { }
    }

    [NodeName("Select Horizontal Text Alignment")]
    [NodeCategory("Revit.Elements.Text")]
    [NodeDescription("HorizontalTextAlignmentStyleSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class HorizontalAlignment : CustomGenericEnumerationDropDown
    {
        public HorizontalAlignment() : base("Horizontal Alignment", typeof(Autodesk.Revit.DB.HorizontalAlignmentStyle)) { }

        [JsonConstructor]
        public HorizontalAlignment(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Horizontal Alignment", typeof(Autodesk.Revit.DB.HorizontalAlignmentStyle), inPorts, outPorts) { }
    }

    [NodeName("Select Vertical Text Alignment")]
    [NodeCategory("Revit.Elements.Text")]
    [NodeDescription("VerticalTextAlignmentStyleSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class VerticalAlignment : CustomGenericEnumerationDropDown
    {
        public VerticalAlignment() : base("Vertical Alignment", typeof(Autodesk.Revit.DB.VerticalAlignmentStyle)) { }

        [JsonConstructor]
        public VerticalAlignment(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Vertical Alignment", typeof(Autodesk.Revit.DB.VerticalAlignmentStyle), inPorts, outPorts) { }
    }
    
    [NodeName("Wall Location")]
    [NodeCategory("Revit.Elements.Wall")]
    [NodeDescription("WallLocationLineDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class WallLocation : CustomGenericEnumerationDropDown
    {
        public WallLocation() : base("Wall Location", typeof(Autodesk.Revit.DB.WallLocationLine)) { }

        [JsonConstructor]
        public WallLocation(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("Wall Location", typeof(Autodesk.Revit.DB.WallLocationLine), inPorts, outPorts) { }
    }


    [NodeName("Schedule Type")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ScheduleTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class ScheduleTypes : CustomGenericEnumerationDropDown
    {
        public ScheduleTypes() : base("ScheduleType", typeof(Revit.Elements.Views.ScheduleView.ScheduleType)) { }

        [JsonConstructor]
        public ScheduleTypes(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("ScheduleType", typeof(Revit.Elements.Views.ScheduleView.ScheduleType), inPorts, outPorts) { }
    }

    [NodeName("Export Column Headers")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ExportColumnHeadersDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class ExportColumnHeaders : CustomGenericEnumerationDropDown
    {
        public ExportColumnHeaders() : base("ColumnHeaders", typeof(Revit.Schedules.ScheduleExportOptions.ExportColumnHeaders)) { }

        [JsonConstructor]
        public ExportColumnHeaders(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("ColumnHeaders", typeof(Revit.Schedules.ScheduleExportOptions.ExportColumnHeaders), inPorts, outPorts) { }
    }

    [NodeName("Export Text Qualifier")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ExportTextQualifierDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class ExportTextQualifier : CustomGenericEnumerationDropDown
    {
        public ExportTextQualifier() : base("TextQualifier", typeof(Revit.Schedules.ScheduleExportOptions.ExportTextQualifier)) { }

        [JsonConstructor]
        public ExportTextQualifier(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("TextQualifier", typeof(Revit.Schedules.ScheduleExportOptions.ExportTextQualifier), inPorts, outPorts) { }
    }

    [NodeName("Fill Patterns")]
    [NodeCategory("Revit.Elements.FillPatternElement")]
    [NodeDescription("FillPatternsDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class FillPatterns : CustomRevitElementDropDown
    {
        public FillPatterns() : base("FillPattern", typeof(Autodesk.Revit.DB.FillPatternElement)) { }

        [JsonConstructor]
        public FillPatterns(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("FillPattern", typeof(Autodesk.Revit.DB.FillPatternElement), inPorts, outPorts) { }
    }

    [NodeName("Fill Pattern Targets")]
    [NodeCategory("Revit.Elements.FillPatternElement")]
    [NodeDescription("FillPatternTargetDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class FillPatternTargets : CustomGenericEnumerationDropDown
    {
        public FillPatternTargets() : base("FillPatternTarget", typeof(Autodesk.Revit.DB.FillPatternTarget)) { }

        [JsonConstructor]
        public FillPatternTargets(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("FillPatternTarget", typeof(Autodesk.Revit.DB.FillPatternTarget), inPorts, outPorts) { }
    }

    [NodeName("Line Patterns")]
    [NodeCategory("Revit.Elements.LinePatternElement")]
    [NodeDescription("LinePatternsDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class LinePatterns : CustomRevitElementDropDown
    {
        public LinePatterns() : base("LinePattern", typeof(Autodesk.Revit.DB.LinePatternElement)) { }

        [JsonConstructor]
        public LinePatterns(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("LinePattern", typeof(Autodesk.Revit.DB.LinePatternElement), inPorts, outPorts) { }
    }

    [NodeName("Schedule Filter Type")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ScheduleFilterTypeDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class ScheduleFilterType : CustomGenericEnumerationDropDown
    {
        public ScheduleFilterType() : base("FilterType", typeof(Autodesk.Revit.DB.ScheduleFilterType)) { }

        [JsonConstructor]
        public ScheduleFilterType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base("FilterType", typeof(Autodesk.Revit.DB.ScheduleFilterType), inPorts, outPorts) { }
    }
}