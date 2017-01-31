using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using Revit.GeometryConversion;
using RevitServices.Transactions;

namespace Revit.Elements.InternalUtilities
{
    [SupressImportIntoVM]
    public class ElementUtils
    {
        /// <summary>
        /// This function checks if the name ends with "(num)". Here num is a integer.
        /// If yes, it will replace "(num)" with "(num+1)". Here num+1 is the form of the
        /// evaluated integer. Otherwise, it will append "(1)" at the end of the name.
        /// For example:
        /// This function will change the name from "abc(2)" to "abc(3)",
        /// from "abc" to "abc(1)".
        /// </summary>
        /// <param name="name"></param>
        [SupressImportIntoVM]
        public static void UpdateLevelName(ref string name)
        {
            if (name.EndsWith(")"))
            {
                int index = name.LastIndexOf("(");
                if (index < 0)
                {
                    name += "(1)";
                }
                else
                {
                    int length = name.Length - index - 2;
                    string strNumber = name.Substring(index + 1, length);
                    int number = 0;
                    if (int.TryParse(strNumber, out number))
                    {
                        number = number + 1;
                        name = name.Substring(0, index) + "(" + number.ToString() + ")";
                    }
                    else
                    {
                        name += "(1)";
                    }
                }
            }
            else
            {
                name += "(1)";
            }
        }


        /// <summary>
        /// Get a revit parameters value
        /// </summary>
        /// <param name="param">Revit parameter</param>
        /// <returns></returns>
        [SupressImportIntoVM]
        public static object GetParameterValue(Autodesk.Revit.DB.Parameter param)
        {
            object result;

            switch (param.StorageType)
            {
                case StorageType.ElementId:
                    int valueId = param.AsElementId().IntegerValue;
                    if (valueId > 0)
                    {
                        // When the element is obtained here, to convert it to our element wrapper, it
                        // need to be figured out whether this element is created by us. Here the existing
                        // element wrappers will be checked. If there is one, its property to specify
                        // whether it is created by us will be followed. If there is none, it means the
                        // element is not created by us.
                        var elem = ElementIDLifecycleManager<int>.GetInstance().GetFirstWrapper(valueId) as Element;
                        result = ElementSelector.ByElementId(valueId, elem == null ? true : elem.IsRevitOwned);
                    }
                    else
                    {
                        int paramId = param.Id.IntegerValue;
                        if (paramId == (int)BuiltInParameter.ELEM_CATEGORY_PARAM || paramId == (int)BuiltInParameter.ELEM_CATEGORY_PARAM_MT)
                        {
                            var categories = DocumentManager.Instance.CurrentDBDocument.Settings.Categories;
                            result = new Category(categories.get_Item((BuiltInCategory)valueId));
                        }
                        else
                        {
                            // For other cases, return a localized string
                            result = param.AsValueString();
                        }
                    }
                    break;
                case StorageType.String:
                    result = param.AsString();
                    break;
                case StorageType.Integer:
                    result = param.AsInteger();
                    break;
                case StorageType.Double:
                    var paramType = param.Definition.ParameterType;
                    UnitType unitType;
                    if (ParamTypeMap.TryGetValue(paramType, out unitType))
                    {
                        result = param.AsDouble() * Revit.GeometryConversion.UnitConverter.HostToDynamoFactor(unitType);
                    }
                    else
                    {
                        result = param.AsDouble();
                    }
                    break;
                default:
                    throw new Exception(string.Format(Properties.Resources.ParameterWithoutStorageType, param));
            }

            return result;
        }

        /// <summary>
        /// Convert Parameter value if necessary
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [SupressImportIntoVM]
        private static double ConvertValue(ParameterType type, double value)
        {
            UnitType unitType;
            if (ParamTypeMap.TryGetValue(type, out unitType))
            {
                return value * UnitConverter.DynamoToHostFactor(unitType);
            }
            else
            {
                return value;
            }
        }

        #region dynamic parameter setting methods

        [SupressImportIntoVM]
        public static void SetParameterValue(Autodesk.Revit.DB.Parameter param, double value)
        {
            if (param.StorageType != StorageType.Integer && param.StorageType != StorageType.Double)
                throw new Exception(Properties.Resources.ParameterStorageNotNumber);

            var valueToSet = GetConvertedParameterValue(param, value);

            param.Set(valueToSet);
        }

        [SupressImportIntoVM]
        public static void SetParameterValue(Autodesk.Revit.DB.Parameter param, Element value)
        {
            if (param.StorageType != StorageType.ElementId)
                throw new Exception(Properties.Resources.ParameterStorageNotElement);

            param.Set(value.InternalElement.Id);
        }

        [SupressImportIntoVM]
        public static void SetParameterValue(Autodesk.Revit.DB.Parameter param, int value)
        {
            if (param.StorageType != StorageType.Integer && param.StorageType != StorageType.Double)
                throw new Exception(Properties.Resources.ParameterStorageNotNumber);

            var valueToSet = GetConvertedParameterValue(param, value);

            param.Set(valueToSet);
        }

        [SupressImportIntoVM]
        public static void SetParameterValue(Autodesk.Revit.DB.Parameter param, string value)
        {
            if (param.StorageType != StorageType.String)
                throw new Exception(Properties.Resources.ParameterStorageNotString);

            param.Set(value);
        }

        [SupressImportIntoVM]
        public static void SetParameterValue(Autodesk.Revit.DB.Parameter param, bool value)
        {
            if (param.StorageType != StorageType.Integer)
                throw new Exception(Properties.Resources.ParameterStorageNotInteger);

            param.Set(value == false ? 0 : 1);
        }

        [SupressImportIntoVM]
        public static double GetConvertedParameterValue(Autodesk.Revit.DB.Parameter param, double value)
        {
            var paramType = param.Definition.ParameterType;
            UnitType unitType;
            if (ParamTypeMap.TryGetValue(paramType, out unitType))
            {
                return value * UnitConverter.DynamoToHostFactor(unitType);
            }
            else
            {
                return value;
            }
        }

        private static Dictionary<ParameterType, UnitType> ParamTypeMap = new Dictionary<ParameterType, UnitType>()
        {
            { ParameterType.Number, UnitType.UT_Number},
            { ParameterType.Length, UnitType.UT_Length},
            { ParameterType.Area, UnitType.UT_Area},
            { ParameterType.Volume, UnitType.UT_Volume},
            { ParameterType.Angle, UnitType.UT_Angle},
            { ParameterType.Force, UnitType.UT_Force},
            { ParameterType.LinearForce, UnitType.UT_LinearForce},
            { ParameterType.AreaForce, UnitType.UT_AreaForce},
            { ParameterType.Moment, UnitType.UT_Moment},
            { ParameterType.HVACDensity, UnitType.UT_HVAC_Density},
            { ParameterType.HVACEnergy, UnitType.UT_HVAC_Energy},
            { ParameterType.HVACFriction, UnitType.UT_HVAC_Friction},
            { ParameterType.HVACPower, UnitType.UT_HVAC_Power},
            { ParameterType.HVACPowerDensity, UnitType.UT_HVAC_Power_Density},
            { ParameterType.HVACPressure, UnitType.UT_HVAC_Pressure},
            { ParameterType.HVACTemperature, UnitType.UT_HVAC_Temperature},
            { ParameterType.HVACVelocity, UnitType.UT_HVAC_Velocity},
            { ParameterType.HVACAirflow, UnitType.UT_HVAC_Airflow},
            { ParameterType.HVACDuctSize, UnitType.UT_HVAC_DuctSize},
            { ParameterType.HVACCrossSection, UnitType.UT_HVAC_CrossSection},
            { ParameterType.HVACHeatGain, UnitType.UT_HVAC_HeatGain},
            { ParameterType.ElectricalCurrent, UnitType.UT_Electrical_Current},
            { ParameterType.ElectricalPotential, UnitType.UT_Electrical_Potential},
            { ParameterType.ElectricalFrequency, UnitType.UT_Electrical_Frequency},
            { ParameterType.ElectricalIlluminance, UnitType.UT_Electrical_Illuminance},
            { ParameterType.ElectricalLuminousFlux, UnitType.UT_Electrical_Luminous_Flux},
            { ParameterType.ElectricalPower, UnitType.UT_Electrical_Power},
            { ParameterType.HVACRoughness, UnitType.UT_HVAC_Roughness},
            { ParameterType.ElectricalApparentPower, UnitType.UT_Electrical_Apparent_Power},
            { ParameterType.ElectricalPowerDensity, UnitType.UT_Electrical_Power_Density},
            { ParameterType.PipingDensity, UnitType.UT_Piping_Density},
            { ParameterType.PipingFlow, UnitType.UT_Piping_Flow},
            { ParameterType.PipingFriction, UnitType.UT_Piping_Friction},
            { ParameterType.PipingPressure, UnitType.UT_Piping_Pressure},
            { ParameterType.PipingTemperature, UnitType.UT_Piping_Temperature},
            { ParameterType.PipingVelocity, UnitType.UT_Piping_Velocity},
            { ParameterType.PipingViscosity, UnitType.UT_Piping_Viscosity},
            { ParameterType.PipeSize, UnitType.UT_PipeSize},
            { ParameterType.PipingRoughness, UnitType.UT_Piping_Roughness},
            { ParameterType.Stress, UnitType.UT_Stress},
            { ParameterType.UnitWeight, UnitType.UT_UnitWeight},
            { ParameterType.ThermalExpansion, UnitType.UT_ThermalExpansion},
            { ParameterType.LinearMoment, UnitType.UT_LinearMoment},
            { ParameterType.ForcePerLength, UnitType.UT_ForcePerLength},
            { ParameterType.ForceLengthPerAngle, UnitType.UT_ForceLengthPerAngle},
            { ParameterType.LinearForcePerLength, UnitType.UT_LinearForcePerLength},
            { ParameterType.LinearForceLengthPerAngle, UnitType.UT_LinearForceLengthPerAngle},
            { ParameterType.AreaForcePerLength, UnitType.UT_AreaForcePerLength},
            { ParameterType.PipingVolume, UnitType.UT_Piping_Volume},
            { ParameterType.HVACViscosity, UnitType.UT_HVAC_Viscosity},
            { ParameterType.HVACCoefficientOfHeatTransfer, UnitType.UT_HVAC_CoefficientOfHeatTransfer},
            { ParameterType.HVACAirflowDensity, UnitType.UT_HVAC_Airflow_Density},
            { ParameterType.HVACCoolingLoad, UnitType.UT_HVAC_Cooling_Load},
            { ParameterType.HVACCoolingLoadDividedByArea, UnitType.UT_HVAC_Cooling_Load_Divided_By_Area},
            { ParameterType.HVACCoolingLoadDividedByVolume, UnitType.UT_HVAC_Cooling_Load_Divided_By_Volume},
            { ParameterType.HVACHeatingLoad, UnitType.UT_HVAC_Heating_Load},
            { ParameterType.HVACHeatingLoadDividedByArea, UnitType.UT_HVAC_Heating_Load_Divided_By_Area},
            { ParameterType.HVACHeatingLoadDividedByVolume, UnitType.UT_HVAC_Heating_Load_Divided_By_Volume},
            { ParameterType.HVACAirflowDividedByVolume, UnitType.UT_HVAC_Airflow_Divided_By_Volume},
            { ParameterType.HVACAirflowDividedByCoolingLoad, UnitType.UT_HVAC_Airflow_Divided_By_Cooling_Load},
            { ParameterType.HVACAreaDividedByCoolingLoad, UnitType.UT_HVAC_Area_Divided_By_Cooling_Load},
            { ParameterType.WireSize, UnitType.UT_WireSize},
            { ParameterType.HVACSlope, UnitType.UT_HVAC_Slope},
            { ParameterType.PipingSlope, UnitType.UT_Piping_Slope},
            { ParameterType.ElectricalEfficacy, UnitType.UT_Electrical_Efficacy},
            { ParameterType.ElectricalWattage, UnitType.UT_Electrical_Wattage},
            { ParameterType.ColorTemperature, UnitType.UT_Color_Temperature},
            { ParameterType.ElectricalLuminousIntensity, UnitType.UT_Electrical_Luminous_Intensity},
            { ParameterType.ElectricalLuminance, UnitType.UT_Electrical_Luminance},
            { ParameterType.HVACAreaDividedByHeatingLoad, UnitType.UT_HVAC_Area_Divided_By_Heating_Load},
            { ParameterType.HVACFactor, UnitType.UT_HVAC_Factor},
            { ParameterType.ElectricalTemperature, UnitType.UT_Electrical_Temperature},
            { ParameterType.ElectricalCableTraySize, UnitType.UT_Electrical_CableTraySize},
            { ParameterType.ElectricalConduitSize, UnitType.UT_Electrical_ConduitSize},
            { ParameterType.ElectricalDemandFactor, UnitType.UT_Electrical_Demand_Factor},
            { ParameterType.HVACDuctInsulationThickness, UnitType.UT_HVAC_DuctInsulationThickness},
            { ParameterType.HVACDuctLiningThickness, UnitType.UT_HVAC_DuctLiningThickness},
            { ParameterType.PipeInsulationThickness, UnitType.UT_PipeInsulationThickness},
            { ParameterType.HVACThermalResistance, UnitType.UT_HVAC_ThermalResistance},
            { ParameterType.HVACThermalMass, UnitType.UT_HVAC_ThermalMass},
            { ParameterType.Acceleration, UnitType.UT_Acceleration},
            { ParameterType.BarDiameter, UnitType.UT_Bar_Diameter},
            { ParameterType.CrackWidth, UnitType.UT_Crack_Width},
            { ParameterType.DisplacementDeflection, UnitType.UT_Displacement_Deflection},
            { ParameterType.Energy, UnitType.UT_Energy},
            { ParameterType.StructuralFrequency, UnitType.UT_Structural_Frequency},
            { ParameterType.Mass, UnitType.UT_Mass},
            { ParameterType.MassPerUnitLength, UnitType.UT_Mass_per_Unit_Length},
            { ParameterType.MomentOfInertia, UnitType.UT_Moment_of_Inertia},
            { ParameterType.SurfaceArea, UnitType.UT_Surface_Area},
            { ParameterType.Period, UnitType.UT_Period},
            { ParameterType.Pulsation, UnitType.UT_Pulsation},
            { ParameterType.ReinforcementAreaPerUnitLength, UnitType.UT_Reinforcement_Area_per_Unit_Length},
            { ParameterType.ReinforcementCover, UnitType.UT_Reinforcement_Cover},
            { ParameterType.ReinforcementSpacing, UnitType.UT_Reinforcement_Spacing},
            { ParameterType.Rotation, UnitType.UT_Rotation},
            { ParameterType.SectionArea, UnitType.UT_Section_Area},
            { ParameterType.SectionDimension, UnitType.UT_Section_Dimension},
            { ParameterType.SectionModulus, UnitType.UT_Section_Modulus},
            { ParameterType.SectionProperty, UnitType.UT_Section_Property},
            { ParameterType.StructuralVelocity, UnitType.UT_Structural_Velocity},
            { ParameterType.WarpingConstant, UnitType.UT_Warping_Constant},
            { ParameterType.Weight, UnitType.UT_Weight},
            { ParameterType.WeightPerUnitLength, UnitType.UT_Weight_per_Unit_Length},
            { ParameterType.HVACThermalConductivity, UnitType.UT_HVAC_ThermalConductivity},
            { ParameterType.HVACSpecificHeat, UnitType.UT_HVAC_SpecificHeat},
            { ParameterType.HVACSpecificHeatOfVaporization, UnitType.UT_HVAC_SpecificHeatOfVaporization},
            { ParameterType.HVACPermeability, UnitType.UT_HVAC_Permeability},
            { ParameterType.ElectricalResistivity, UnitType.UT_Electrical_Resistivity},
            { ParameterType.MassPerUnitArea, UnitType.UT_MassPerUnitArea},
            { ParameterType.PipeDimension, UnitType.UT_Pipe_Dimension},
            { ParameterType.PipeMass, UnitType.UT_PipeMass},
            { ParameterType.PipeMassPerUnitLength, UnitType.UT_PipeMassPerUnitLength},
            { ParameterType.HVACTemperatureDifference, UnitType.UT_HVAC_TemperatureDifference},
            { ParameterType.PipingTemperatureDifference, UnitType.UT_Piping_TemperatureDifference},
            { ParameterType.ElectricalTemperatureDifference, UnitType.UT_Electrical_TemperatureDifference},
            { ParameterType.Slope, UnitType.UT_Slope},
            { ParameterType.Currency, UnitType.UT_Currency},
            { ParameterType.MassDensity, UnitType.UT_MassDensity},
            { ParameterType.ReinforcementLength, UnitType.UT_Reinforcement_Length},
            { ParameterType.ReinforcementArea, UnitType.UT_Reinforcement_Area},
            { ParameterType.ReinforcementVolume, UnitType.UT_Reinforcement_Volume}
        };
        #endregion
    }
}
