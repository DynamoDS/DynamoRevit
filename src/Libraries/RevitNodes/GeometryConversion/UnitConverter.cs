using System;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using RevitServices.Persistence;

namespace Revit.GeometryConversion
{
    [SupressImportIntoVM]
    public static class UnitConverter
    {
        //public static double DynamoToHostFactor
        //{
        //    get { return Length.FromDouble(1.0).ConvertToHostUnits(); }
        //}

        //public static double HostToDynamoFactor
        //{
        //    get { return 1 / DynamoToHostFactor; }
        //}

        public static double DynamoToHostFactor(UnitType unitType)
        {
            var revitDisplayUnits =
                DocumentManager.Instance.CurrentDBDocument.GetUnits()
                    .GetFormatOptions(unitType)
                    .DisplayUnits;
            return UnitUtils.ConvertToInternalUnits(1, revitDisplayUnits);
        }

        public static double HostToDynamoFactor(UnitType unitType)
        {
            return 1/DynamoToHostFactor(unitType);
        }

        /// <summary>
        /// Convert from Revit API internal units (feet) to Dynamo internal units (meters) 
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

            var result = (T)geometry.Scale(HostToDynamoFactor(UnitType.UT_Length));
            return result;
        }

        /// <summary>
        /// Convert from Dynamo internal units (meters) to Revit API internal units (feet)
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

            var result = (T)geometry.Scale(DynamoToHostFactor(UnitType.UT_Length));
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
