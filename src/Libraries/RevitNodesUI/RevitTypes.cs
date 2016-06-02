using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    [NodeName("Phase Selector")]
    [NodeCategory("Revit.Phase")]
    [NodeDescription("Phase Selector")]
    [IsDesignScriptCompatible]
    public class RevitPhases : CustomRevitElementDropDown
    {
        public RevitPhases() : base("Phase", typeof(Autodesk.Revit.DB.Phase)) { }
    }

    [NodeName("Revision Selector")]
    [NodeCategory("Revit.Revision")]
    [NodeDescription("Revision Selector")]
    [IsDesignScriptCompatible]
    public class RevitRevisions : CustomRevitElementDropDown
    {
        public RevitRevisions() : base("Revision", typeof(Autodesk.Revit.DB.Revision)) { }
    }

    [NodeName("Filled Region Type Selector")]
    [NodeCategory("Revit.FilledRegionType")]
    [NodeDescription("Filled Region Type Selector")]
    [IsDesignScriptCompatible]
    public class FilledRegionTypes : CustomRevitElementDropDown
    {
        public FilledRegionTypes() : base("FilledRegionType", typeof(Autodesk.Revit.DB.FilledRegionType)) { }
    }



    [NodeName("Revision Numbering")]
    [NodeCategory("Revit.Revision")]
    [NodeDescription("Revision Numbering")]
    [IsDesignScriptCompatible]
    public class RevisionNumbering : CustomGenericEnumerationDropDown
    {
        public RevisionNumbering() : base("Revision Numbering", typeof(Autodesk.Revit.DB.RevisionNumbering)) { }
    }

    [NodeName("Revision Number Type")]
    [NodeCategory("Revit.Revision")]
    [NodeDescription("Revision Number Type")]
    [IsDesignScriptCompatible]
    public class RevisionNumberType : CustomGenericEnumerationDropDown
    {
        public RevisionNumberType() : base("Revision Number Type", typeof(Autodesk.Revit.DB.RevisionNumberType)) { }
    }


    [NodeName("Parameter Type")]
    [NodeCategory("Revit.Parameter")]
    [NodeDescription("Parameter Type")]
    [IsDesignScriptCompatible]
    public class ParameterType : CustomGenericEnumerationDropDown
    {
        public ParameterType() : base("Parameter Type", typeof(Autodesk.Revit.DB.ParameterType)) { }
    }

    [NodeName("BuiltIn Parameter Group")]
    [NodeCategory("Revit.Parameter")]
    [NodeDescription("BuiltIn Parameter Group")]
    [IsDesignScriptCompatible]
    public class BuiltInParameterGroup : CustomGenericEnumerationDropDown
    {
        public BuiltInParameterGroup() : base("BuiltIn Parameter Group", typeof(Autodesk.Revit.DB.BuiltInParameterGroup)) { }
    }

    [NodeName("Revision Visibility")]
    [NodeCategory("Revit.Revision")]
    [NodeDescription("Revision Visibility")]
    [IsDesignScriptCompatible]
    public class RevisionVisibility : CustomGenericEnumerationDropDown
    {
        public RevisionVisibility() : base("Revision Visibility", typeof(Autodesk.Revit.DB.RevisionVisibility)) { }
    }

    [NodeName("Direct Shape Room Bounding Option")]
    [NodeCategory("Revit.DirectShape")]
    [NodeDescription("Direct Shape Room Bounding Option")]
    [IsDesignScriptCompatible]
    public class DirectShapeRoomBoundingOption : CustomGenericEnumerationDropDown
    {
        public DirectShapeRoomBoundingOption() : base("Direct Shape Room Bounding Option", typeof(Autodesk.Revit.DB.DirectShapeRoomBoundingOption)) { }
    }


    [NodeName("Horizontal Text Alignment Style")]
    [NodeCategory("Revit.Rebar")]
    [NodeDescription("Horizontal Text Alignment Style")]
    [IsDesignScriptCompatible]
    public class HorizontalAlignment : CustomGenericEnumerationDropDown
    {
        public HorizontalAlignment() : base("Horizontal Alignment", typeof(Autodesk.Revit.DB.HorizontalAlignmentStyle)) { }
    }

    [NodeName("Vertical Text Alignment Style")]
    [NodeCategory("Revit.Rebar")]
    [NodeDescription("Vertical Text Alignment Style")]
    [IsDesignScriptCompatible]
    public class VerticalAlignment : CustomGenericEnumerationDropDown
    {
        public VerticalAlignment() : base("Vertical Alignment", typeof(Autodesk.Revit.DB.VerticalAlignmentStyle)) { }
    }

}