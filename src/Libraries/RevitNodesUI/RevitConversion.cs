using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using DSCoreNodesUI;
using Dynamo.Models;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Nodes;
using Revit.Elements;
using RevitServices.Persistence;

namespace DSRevitNodesUI
{
    public static class RevitUnitTypes
    {
        public enum RevitUnits {UT_Length,UT_Mass,UT_Area,UT_Volume,UT_Angle}
        public enum ConversionMetricUnit { Length, Area, Volume,}
        public enum ConversionUnit
        {
            Feet, Inches, Millimeters, Centimeters, Decimeters, Meters, 
            SquareMeter, SquareFoot,SquareInch,SquareCentimeter,SquareMillimeter,
            Acres, Hectares, CubicMeters, CubicFoot, CubicYards, CubicInches,CubicCentimeter,
            CubicMillimeter,Litres,USGallons
        }
        public enum RevitDisplayUnit
        {
            DUT_DECIMAL_FEET,DUT_FEET_FRACTIONAL_INCHES,DUT_DECIMAL_INCHES,DUT_FRACTIONAL_INCHES,
            DUT_METERS,DUT_CENTIMETERS,DUT_MILLIMETERS,DUT_DECIMETERS,DUT_METERS_CENTIMETERS,
            DUT_SQUARE_FEET, DUT_SQUARE_INCHES, DUT_SQUARE_METERS, DUT_SQUARE_CENTIMETERS,
            DUT_SQUARE_MILLIMETERS, DUT_SQUARE_ACRES, DUT_SQUARE_HECTARES,
            DUT_CUBIC_YARDS, DUT_CUBIC_FEET, DUT_CUBIC_INCHES, DUT_CUBIC_METERS, DUT_CUBIC_CENTIMETERS,
            DUT_CUBIC_MILLIMETERS, DUT_CUBIC_LITRES, DUT_CUBIC_GALLONS_US
        };

        public static readonly Dictionary<ConversionMetricUnit, RevitUnits> RevitConversionDefaults =
          new Dictionary<ConversionMetricUnit, RevitUnits>()
            {
                {ConversionMetricUnit.Length, RevitUnits.UT_Length},              
                {ConversionMetricUnit.Area, RevitUnits.UT_Area},
                {ConversionMetricUnit.Volume, RevitUnits.UT_Volume},                               
            };

        public static readonly Dictionary<RevitDisplayUnit, ConversionUnit> RevitUnitConversionLookup =
         new Dictionary<RevitDisplayUnit, ConversionUnit>()
            {
                /* LENGTH - DEFAULT UNITS */
                {RevitDisplayUnit.DUT_FEET_FRACTIONAL_INCHES, ConversionUnit.Feet},    
                {RevitDisplayUnit.DUT_DECIMAL_FEET,ConversionUnit.Feet},   
                {RevitDisplayUnit.DUT_DECIMAL_INCHES, ConversionUnit.Inches}, 
                {RevitDisplayUnit.DUT_FRACTIONAL_INCHES, ConversionUnit.Inches},   
                {RevitDisplayUnit.DUT_METERS, ConversionUnit.Meters},   
                {RevitDisplayUnit.DUT_CENTIMETERS,ConversionUnit.Centimeters},   
                {RevitDisplayUnit.DUT_MILLIMETERS, ConversionUnit.Millimeters},   
                {RevitDisplayUnit.DUT_DECIMETERS, ConversionUnit.Decimeters},   

                /*AREA - DEFAULT UNITS*/
                {RevitDisplayUnit.DUT_SQUARE_FEET, ConversionUnit.SquareFoot},
                {RevitDisplayUnit.DUT_SQUARE_INCHES,  ConversionUnit.SquareInch},
                {RevitDisplayUnit.DUT_SQUARE_METERS, ConversionUnit.SquareMeter},
                {RevitDisplayUnit.DUT_SQUARE_CENTIMETERS, ConversionUnit.SquareCentimeter},
                {RevitDisplayUnit.DUT_SQUARE_MILLIMETERS, ConversionUnit.SquareMillimeter},               
                {RevitDisplayUnit.DUT_SQUARE_ACRES, ConversionUnit.Acres},
                {RevitDisplayUnit.DUT_SQUARE_HECTARES, ConversionUnit.Hectares},

                /*VOLUME - DEFAULT UNITS */
                {RevitDisplayUnit.DUT_CUBIC_YARDS, ConversionUnit.CubicYards},
                {RevitDisplayUnit.DUT_CUBIC_FEET,  ConversionUnit.CubicFoot},
                {RevitDisplayUnit.DUT_CUBIC_INCHES,  ConversionUnit.CubicInches},
                {RevitDisplayUnit.DUT_CUBIC_METERS,  ConversionUnit.CubicMeters},
                {RevitDisplayUnit.DUT_CUBIC_CENTIMETERS,  ConversionUnit.CubicCentimeter},
                {RevitDisplayUnit.DUT_CUBIC_MILLIMETERS,  ConversionUnit.CubicMillimeter},
                {RevitDisplayUnit.DUT_CUBIC_LITRES,  ConversionUnit.Litres},
                {RevitDisplayUnit.DUT_CUBIC_GALLONS_US,  ConversionUnit.USGallons}
                                       
            };

        public static readonly Dictionary<ConversionMetricUnit, List<ConversionUnit>> ConversionMetricLookup =
           new Dictionary<ConversionMetricUnit, List<ConversionUnit>>()
            {               
                {ConversionMetricUnit.Length, new List<ConversionUnit>()
                                    {ConversionUnit.Feet,ConversionUnit.Inches,ConversionUnit.Millimeters,ConversionUnit.Centimeters, 
                                        ConversionUnit.Meters,ConversionUnit.Decimeters}},
                {ConversionMetricUnit.Area, new List<ConversionUnit>()
                                    {ConversionUnit.SquareMeter,ConversionUnit.SquareFoot,ConversionUnit.SquareInch,
                                        ConversionUnit.SquareCentimeter,ConversionUnit.SquareMillimeter,
                                        ConversionUnit.Acres,ConversionUnit.Hectares}},
                   {ConversionMetricUnit.Volume, new List<ConversionUnit>()
                   {
                       ConversionUnit.CubicMeters,ConversionUnit.CubicFoot,ConversionUnit.CubicInches,
                       ConversionUnit.CubicCentimeter,ConversionUnit.CubicMillimeter,ConversionUnit.CubicYards,
                       ConversionUnit.Litres,ConversionUnit.USGallons
                   }}
            };

        public static readonly Dictionary<ConversionMetricUnit, ConversionUnit> ConversionDefaults =
            new Dictionary<ConversionMetricUnit, ConversionUnit>()
            {
                {ConversionMetricUnit.Length, ConversionUnit.Meters},
                {ConversionMetricUnit.Area, ConversionUnit.SquareMeter},
                {ConversionMetricUnit.Volume, ConversionUnit.CubicMeters},                    
            };
    }
   
    [NodeName("Convert Units"), NodeCategory(BuiltinNodeCategories.ANALYZE),
     NodeDescription("RevitConversionNodeDescription", typeof (Properties.Resources)), IsDesignScriptCompatible]
    public class RevitConvert : DynamoConvert
    {       
       private object selectedMetricConversion;
       private object selectedFromConversion;
       private object selectedToConversion;     

       public RevitConvert() : base("From Revit")
       {                        
       }

        public override object SelectedMetricConversion
        {
            get { return selectedMetricConversion; }
            set
            {             
                selectedMetricConversion = value;
                var revitUnit = RevitUnitTypes.RevitConversionDefaults[(RevitUnitTypes.ConversionMetricUnit)value];
                var revitDisplayUnits = DocumentManager.Instance.CurrentDBDocument.GetUnits().
                                           GetFormatOptions((UnitType) Enum.Parse(typeof(UnitType), revitUnit.ToString())).DisplayUnits;
                var convertUnit = RevitUnitTypes.RevitUnitConversionLookup[(RevitUnitTypes.RevitDisplayUnit)
                    Enum.Parse(typeof(RevitUnitTypes.RevitDisplayUnit), revitDisplayUnits.ToString())];

                SelectedFromConversionSource = Enum.GetValues(typeof (RevitUnitTypes.ConversionUnit))
                    .Cast<RevitUnitTypes.ConversionUnit>().ToList();

                SelectedToConversionSource =
                        RevitUnitTypes.ConversionMetricLookup[(RevitUnitTypes.ConversionMetricUnit)Enum.Parse(typeof(RevitUnitTypes.ConversionMetricUnit), value.ToString())];

                SelectedFromConversion = convertUnit;
                SelectedToConversion = RevitUnitTypes.ConversionDefaults[(RevitUnitTypes.ConversionMetricUnit)Enum.Parse(typeof(RevitUnitTypes.ConversionMetricUnit), value.ToString())];

                RaisePropertyChanged("SelectedMetricConversion");
            }
        }

        public override object SelectedFromConversion
        {
            get { return selectedFromConversion; }
            set
            {
                if (value != null)
                    selectedFromConversion = (RevitUnitTypes.ConversionUnit)Enum.Parse(typeof(RevitUnitTypes.ConversionUnit), value.ToString());
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
                    selectedToConversion = (RevitUnitTypes.ConversionUnit)Enum.Parse(typeof(RevitUnitTypes.ConversionUnit), value.ToString());
                else
                    selectedToConversion = null;
                RaisePropertyChanged("SelectedToConversion");
            }
        }
    }
}
