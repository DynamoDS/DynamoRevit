using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using RVT = Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Dynamo.Utilities;
using Dynamo.Models;
using Revit.GeometryConversion;

namespace Revit.Elements
{

    /// <summary>
    /// Nodes exposing Revit Document Base and Survey Point
    /// </summary>
    public static class Coordinates
    {
        /// <summary>
        /// Get Base or SurveyPoint
        /// </summary>
        /// <param name="surveypoint"></param>
        /// <returns></returns>  
        private static Point GetBaseOrSurveyPoint(bool surveypoint)
        {
            // Get Base or Survey point category
            RVT.BuiltInCategory category = (surveypoint)? RVT.BuiltInCategory.OST_SharedBasePoint : RVT.BuiltInCategory.OST_ProjectBasePoint;

            // Get Revit document
            RVT.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            
            // Get all elements of the previously selected category
            Autodesk.Revit.DB.FilteredElementCollector collector = new RVT.FilteredElementCollector(doc).OfCategory(category);
            
            // Get the first element (should only be one)
            RVT.BasePoint element = (RVT.BasePoint)collector.ToElements().FirstOrDefault();

            if (element == null)
            {
                throw new Exception(Properties.Resources.CannotGetBaseOrSurveyPoint);
            }

            // Get the elements bounding box
            RVT.BoundingBoxXYZ box = element.get_BoundingBox(null);
            
            // Since the boundingbox is a point only, return Min or Max
            return box.Max.ToPoint();
        }

        /// <summary>
        /// Get Project Rotation
        /// </summary>
        /// <returns>Rotation in degrees</returns>
        public static double ProjectRotation()
        {
            // Get project base point
            RVT.Document doc = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
            Autodesk.Revit.DB.FilteredElementCollector collector = new RVT.FilteredElementCollector(doc).OfCategory(RVT.BuiltInCategory.OST_ProjectBasePoint);            
            RVT.BasePoint element = (RVT.BasePoint)collector.ToElements().FirstOrDefault();

            if (element == null)
            {
                throw new Exception(Properties.Resources.CannotGetBaseOrSurveyPoint);
            }

            // Return the rotation parameter as double
            RVT.Parameter param = element.get_Parameter(RVT.BuiltInParameter.BASEPOINT_ANGLETON_PARAM);
            return param.AsDouble().ToDegrees();
        }

        /// <summary>
        /// Get Project Base Point
        /// </summary>
        /// <returns>Project Base Point</returns>
        public static Point BasePoint()
        {
            return GetBaseOrSurveyPoint(false);
        }

        /// <summary>
        /// Get Survey Point
        /// </summary>
        /// <returns>Survey Point</returns>
        public static Point SurveyPoint()
        {
            return GetBaseOrSurveyPoint(true);
        }


    }


}


