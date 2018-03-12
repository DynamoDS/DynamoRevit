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
    public class RevisionNumberType : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Revision Number Type";

        public RevisionNumberType() : base(outputName, typeof(Autodesk.Revit.DB.RevisionNumberType)) { }

        [JsonConstructor]
        public RevisionNumberType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.RevisionNumberType), inPorts, outPorts) { }
    }


    [NodeName("Select Parameter Type")]
    [NodeCategory("Revit.Elements.Parameter")]
    [NodeDescription("ParameterTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class ParameterType : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Parameter Type";

        public ParameterType() : base(outputName, typeof(Autodesk.Revit.DB.ParameterType)) { }

        [JsonConstructor]
        public ParameterType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.ParameterType), inPorts, outPorts) { }
    }

    [NodeName("Select BuiltIn Parameter Group")]
    [NodeCategory("Revit.Elements.Parameter")]
    [NodeDescription("BuiltInParameterGroupSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class BuiltInParameterGroup : CustomGenericEnumerationDropDown
    {
        private const string outputName = "BuiltIn Parameter Group";

        public BuiltInParameterGroup() : base(outputName, typeof(Autodesk.Revit.DB.BuiltInParameterGroup)) { }

        [JsonConstructor]
        public BuiltInParameterGroup(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.BuiltInParameterGroup), inPorts, outPorts) { }
    }

    [NodeName("Select Revision Visibility")]
    [NodeCategory("Revit.Elements.Revision")]
    [NodeDescription("RevisionVisibilitySelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
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
    public class DetailLevel : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Detail Level";

        public DetailLevel() : base(outputName, typeof(Autodesk.Revit.DB.ViewDetailLevel)) { }

        [JsonConstructor]
        public DetailLevel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.ViewDetailLevel), inPorts, outPorts) { }
    }

    [NodeName("Select Horizontal Text Alignment")]
    [NodeCategory("Revit.Elements.Text")]
    [NodeDescription("HorizontalTextAlignmentStyleSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
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
    public class VerticalAlignment : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Vertical Alignment";

        public VerticalAlignment() : base(outputName, typeof(Autodesk.Revit.DB.VerticalAlignmentStyle)) { }

        [JsonConstructor]
        public VerticalAlignment(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.VerticalAlignmentStyle), inPorts, outPorts) { }
    }
    
    [NodeName("Schedule Type")]
    [NodeCategory("Revit.Views.ScheduleView")]
    [NodeDescription("ScheduleTypeSelectorDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
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
    public class ScheduleFilterType : CustomGenericEnumerationDropDown
    {
        private const string outputName = "FilterType";

        public ScheduleFilterType() : base(outputName, typeof(Autodesk.Revit.DB.ScheduleFilterType)) { }

        [JsonConstructor]
        public ScheduleFilterType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(outputName, typeof(Autodesk.Revit.DB.ScheduleFilterType), inPorts, outPorts) { }
    }

    [NodeName("Wall Location")]
    [NodeCategory("Revit.Elements.Wall")]
    [NodeDescription("WallLocationLineDescription", typeof(DSRevitNodesUI.Properties.Resources))]
    [IsDesignScriptCompatible]
    public class WallLocation : CustomGenericEnumerationDropDown
    {
        private const string outputName = "Wall Location";

        public WallLocation() : base(outputName, typeof(Autodesk.Revit.DB.WallLocationLine)) { }

        [JsonConstructor]
        public WallLocation(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(outputName, typeof(Autodesk.Revit.DB.WallLocationLine), inPorts, outPorts) { }
    }
}