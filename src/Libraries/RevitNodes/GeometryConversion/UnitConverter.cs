﻿using System;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using RevitServices.Persistence;

namespace Revit.GeometryConversion
{
    [SupressImportIntoVM]
    public static class UnitConverter
    {
        public static double DynamoToHostFactor(ForgeTypeId specTypeId)
        {
            var unitTypeId =
                DocumentManager.Instance.CurrentDBDocument.GetUnits()
                    .GetFormatOptions(specTypeId)
                    .GetUnitTypeId();
           
            // Here we use the Revit API to return the conversion
            // factor between the display units in Revit and the internal
            // units (decimal feet). We are not converting a value, so 
            // we simply supply 1 to return the converstion factor.
            return UnitUtils.ConvertToInternalUnits(1, unitTypeId);
        }

        public static double HostToDynamoFactor(ForgeTypeId specTypeId)
        {
            // Here we invert the conversion factor to return
            // the conversion from internal units to display units.
            return 1/DynamoToHostFactor(specTypeId);
        }

        /// <summary>
        /// Convert from Revit API internal units (feet) to Revit Display units.
        /// 
        /// Can be used simply as geometry.InDynamoUnits() as the type is constrained
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static T InDynamoUnits<T>(this T geometry)
            where T : Autodesk.DesignScript.Geometry.Geometry
        {
            if (geometry == null)
            {
                throw new ArgumentNullException("geometry");
            }

            var result = (T)geometry.Scale(HostToDynamoFactor(SpecTypeId.Length));
            geometry.Dispose();
            return result;
        }

        /// <summary>
        /// Convert from Revit Display Units to Revit API internal units (feet)
        /// 
        /// Can be used simply as geometry.InHostUnits()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static T InHostUnits<T>(this T geometry)
            where T : Autodesk.DesignScript.Geometry.Geometry
        {
            if (geometry == null)
            {
                throw new ArgumentNullException("geometry");
            }

            var result = (T)geometry.Scale(DynamoToHostFactor(SpecTypeId.Length));
            return result;
        }
        
        /// <summary>
        /// Convert the geometry to Dynamo units if convert is true.
        /// The input geometry will be disposed and a converted geometry
        /// will be assigned to it.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static void ConvertToDynamoUnits<T>(ref T geometry)
            where T : Autodesk.DesignScript.Geometry.Geometry
        {
            var result = geometry.InDynamoUnits();
            geometry.Dispose();
            geometry = result;
        }

        /// <summary>
        /// Convert the geometry to host units if convert is true.
        /// The input geometry will be disposed and a converted geometry
        /// will be assigned to it.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static void ConvertToHostUnits<T>(ref T geometry)
            where T : Autodesk.DesignScript.Geometry.Geometry
        {
            var result = geometry.InHostUnits();
            geometry.Dispose();
            geometry = result;
        }
    }

}
