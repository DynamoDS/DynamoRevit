using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using DSCoreNodesUI;
using Dynamo.Models;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Nodes;
using Revit.Elements;
using RevitServices.Persistence;

namespace DSRevitNodesUI
{
    public static class RevitConversion
    {
        public enum RevitUnits {UT_Length,UT_Mass,UT_Area,UT_Volume,UT_Angle}
        public enum ConversionMetricUnit { Length, Area, Volume,}
        public enum ConversionUnit
        {
            Feet, Inches, Millimeters, Centimeters, Meters, Degrees, Radians, Kilograms, Pounds,
            CubicMeters, CubicFoot, SquareMeter, SquareFoot
        }
        public enum RevitDisplayUnit
        {
            DUT_FEET_FRACTIONAL_INCHES,
            DUT_CUBIC_FEET,
            DUT_SQUARE_FEET
        };

        public static readonly Dictionary<ConversionMetricUnit, RevitUnits> RevitConversionDefaults =
          new Dictionary<ConversionMetricUnit, RevitUnits>()
            {
                {ConversionMetricUnit.Length, RevitUnits.UT_Length},              
                {ConversionMetricUnit.Area, RevitUnits.UT_Area},
                {ConversionMetricUnit.Volume, RevitUnits.UT_Volume},                               
            };

        public static readonly Dictionary<RevitDisplayUnit, List<ConversionUnit>> RevitUnitConversionLookup =
         new Dictionary<RevitDisplayUnit, List<ConversionUnit>>()
            {
                {RevitDisplayUnit.DUT_FEET_FRACTIONAL_INCHES, new List<ConversionUnit>() {ConversionUnit.Feet}},              
                {RevitDisplayUnit.DUT_CUBIC_FEET, new List<ConversionUnit>() {ConversionUnit.CubicFoot}},
                {RevitDisplayUnit.DUT_SQUARE_FEET, new List<ConversionUnit>() {ConversionUnit.SquareMeter}}                               
            };

        public static readonly Dictionary<ConversionMetricUnit, List<ConversionUnit>> ConversionMetricLookup =
           new Dictionary<ConversionMetricUnit, List<ConversionUnit>>()
            {               
                {ConversionMetricUnit.Length, new List<ConversionUnit>()
                                    {ConversionUnit.Feet,ConversionUnit.Inches,ConversionUnit.Millimeters,ConversionUnit.Centimeters, 
                                        ConversionUnit.Meters}},
                {ConversionMetricUnit.Area, new List<ConversionUnit>()
                                    {ConversionUnit.SquareMeter,ConversionUnit.SquareFoot}},
                   {ConversionMetricUnit.Volume, new List<ConversionUnit>()
                                    {ConversionUnit.CubicMeters,ConversionUnit.CubicFoot}}
            };

        public static readonly Dictionary<ConversionMetricUnit, ConversionUnit> ConversionDefaults =
            new Dictionary<ConversionMetricUnit, ConversionUnit>()
            {
                {ConversionMetricUnit.Length, ConversionUnit.Meters},
                {ConversionMetricUnit.Area, ConversionUnit.SquareMeter},
                {ConversionMetricUnit.Volume, ConversionUnit.CubicMeters},                    
            };

        public static readonly Dictionary<RevitDisplayUnit, ConversionUnit> RevitUnitConversionDefault =
         new Dictionary<RevitDisplayUnit, ConversionUnit>()
            {
                {RevitDisplayUnit.DUT_FEET_FRACTIONAL_INCHES, ConversionUnit.Feet},              
                {RevitDisplayUnit.DUT_CUBIC_FEET, ConversionUnit.CubicFoot},
                {RevitDisplayUnit.DUT_SQUARE_FEET,ConversionUnit.SquareMeter}                               
            };
       
    }
   
    [NodeName("Translate Units"), NodeCategory(BuiltinNodeCategories.ANALYZE),
     NodeDescription("RevitConversionNodeDescription", typeof (Properties.Resources)), IsDesignScriptCompatible]
    public class RevitConvert : DynamoConvert
    {       
       private object selectedMetricConversion;
       private object selectedFromConversion;
       private object selectedToConversion;

       public RevitConvert() : base()
       {                    
       }

        public override object SelectedMetricConversion
        {
            get { return selectedMetricConversion; }
            set
            {
                selectedMetricConversion = value;
                var revitUnit = RevitConversion.RevitConversionDefaults[(RevitConversion.ConversionMetricUnit)value];
                var revitDisplayUnits = DocumentManager.Instance.CurrentDBDocument.GetUnits().
                                           GetFormatOptions((UnitType) Enum.Parse(typeof(UnitType), revitUnit.ToString())).DisplayUnits;
                SelectedFromConversionSource = RevitConversion.RevitUnitConversionLookup[(RevitConversion.RevitDisplayUnit) 
                    Enum.Parse(typeof(RevitConversion.RevitDisplayUnit),revitDisplayUnits.ToString())];
                SelectedToConversionSource =
                        RevitConversion.ConversionMetricLookup[(RevitConversion.ConversionMetricUnit)Enum.Parse(typeof(RevitConversion.ConversionMetricUnit), value.ToString())];

                SelectedFromConversion =
                    RevitConversion.RevitUnitConversionDefault[(RevitConversion.RevitDisplayUnit)
                    Enum.Parse(typeof(RevitConversion.RevitDisplayUnit), revitDisplayUnits.ToString())];
                SelectedToConversion = RevitConversion.ConversionDefaults[(RevitConversion.ConversionMetricUnit)Enum.Parse(typeof(RevitConversion.ConversionMetricUnit), value.ToString())];

                RaisePropertyChanged("SelectedMetricConversion");
            }
        }

        public override object SelectedFromConversion
        {
            get { return selectedFromConversion; }
            set
            {
                if (value != null)
                    selectedFromConversion = (RevitConversion.ConversionUnit)Enum.Parse(typeof(RevitConversion.ConversionUnit), value.ToString());
                else
                    selectedFromConversion = null;
                RaisePropertyChanged("SelectedFromConversion");
            }
        }

        public override object SelectedToConversion
        {
            get { return selectedToConversion; }
            set
            {
                if (value != null)
                    selectedToConversion = (RevitConversion.ConversionUnit)Enum.Parse(typeof(RevitConversion.ConversionUnit), value.ToString());
                else
                    selectedToConversion = null;
                RaisePropertyChanged("SelectedToConversion");
            }
        }
    }
}
