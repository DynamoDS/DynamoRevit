using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using DSCoreNodesUI;
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
using Dynamo.Applications.Models;
using Dynamo.Nodes;

namespace DSRevitNodesUI
{
    public enum ConversionUnit { Feet, Inches, Millimeters, Centimeters, Meters, Degrees, Radians, Kilograms, Pounds }
   
    [NodeName("Translate Units"), NodeCategory(BuiltinNodeCategories.ANALYZE),
     NodeDescription("RevitConversionNodeDescription", typeof (Properties.Resources)), IsDesignScriptCompatible]
    public class RevitConvert : DynamoConvert
    {
        private readonly ConversionUnit fromConversion;
        private readonly ConversionUnit toConversion;

        public RevitConvert() : base(0)
        {            
            SelectedFromConversion = fromConversion;
            SelectedToConversion = toConversion;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(
           List<AssociativeNode> inputAstNodes)
        {
            var conversionToNode =
                AstFactory.BuildDoubleNode(1);

            var conversionFromNode =
                AstFactory.BuildDoubleNode(1);
            AssociativeNode node = null;

            node = AstFactory.BuildFunctionCall(
                        new Func<double, double, double, double>(ConvertUnits),
                        new List<AssociativeNode> { inputAstNodes[0], conversionFromNode, conversionToNode });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
        }

    }
}
