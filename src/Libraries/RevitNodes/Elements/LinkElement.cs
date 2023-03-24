using Autodesk.Revit.DB;
using System;
using Revit.Elements;
using System.Collections.Generic;
using System.Linq;
using DynamoUnits;
using View = Revit.Elements.Views.View;
using System.Text;
using System.Threading.Tasks;
using Level = Revit.Elements.Level;
using RevitServices.Persistence;
using Autodesk.Revit.UI;
using System.Windows;
using Autodesk.DesignScript.Geometry;
using Revit.GeometryConversion;
using System.Windows.Media.Imaging;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace Revit.Elements
{ 
    /// <summary>
    /// A Revit Link Instance
    /// </summary>
    public static class LinkElement
    {
        /// <summary>
        /// Return the Document of the given Linked element
        /// </summary>
        /// <returns name="linkDocument">linkDocument</returns>
        public static Revit.Application.Document Document(Element element)
        {
            Autodesk.Revit.DB.Element revitElement = element.InternalElement;

            var elementDocument = revitElement.Document;

            return new Revit.Application.Document(elementDocument); 
        }

        private static List<RevitLinkInstance> GetLinkInstancesContainingLinkElement(Element linkElement)
        {
            Autodesk.Revit.DB.Element revitElement = linkElement.InternalElement;
            // get the document of the linked element
            Document linkDocToMatch = revitElement.Document;
            Document currentDocument = Application.Document.Current.InternalDocument;
            var allLinkInstances = new FilteredElementCollector(currentDocument)
                .OfCategory(BuiltInCategory.OST_RvtLinks)
                .WhereElementIsNotElementType()
                .ToElements()
                .Cast<RevitLinkInstance>()
                .ToList();
              
            // list to store all instances that match the linked element's document
            List<RevitLinkInstance> matchingLinkInstances = new List<RevitLinkInstance>();
            foreach (RevitLinkInstance linkInstance in allLinkInstances)
            {
                Document linkDoc = linkInstance.GetLinkDocument();              
                if (linkDoc.Equals(linkDocToMatch))
                {                   
                    matchingLinkInstances.Add(linkInstance);
                }
            }         
            return matchingLinkInstances;
        }

        public static List<Transform> LinkTransform(Element element) 
        {
            var revitLinkInstances = GetLinkInstancesContainingLinkElement(element);
            
            List<Transform> listOfTransforms = new List<Transform>();
            foreach (var revitLinkInstance in revitLinkInstances)
            {
                listOfTransforms.Add(revitLinkInstance.GetTotalTransform());
            }
            return listOfTransforms;
        }
      
      
        public static List<Transform> LinkInverseTransform(Element element)
        {
            List<RevitLinkInstance> revitLinkInstances = GetLinkInstancesContainingLinkElement(element);
            List<Transform> listOfInverseTransforms = new List<Transform>();
            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances) 
            {
                listOfInverseTransforms.Add(revitLinkInstance.GetTotalTransform().Inverse);         
            }
            return listOfInverseTransforms;
        }
        
        public static BoundingBox BoundingBox (Element linkElement)
        {
            Autodesk.Revit.DB.Element revitElement =linkElement.InternalElement;
            BoundingBoxXYZ boundingBox = revitElement.get_BoundingBox(null);

            return boundingBox.ToProtoType();
        }

        public static object GetGeometry(Element linkElement)
        {
            Autodesk.Revit.DB.Element revitElement = linkElement.InternalElement;
            Options options = new Options();
            GeometryObject linkElementGeometry = revitElement.get_Geometry(options);
            object dSGeometry = linkElementGeometry.Convert();

            return dSGeometry;
        }

        public static Geometry GetLocation(Element linkElement) 
        {
            Autodesk.Revit.DB.Element revitElement = linkElement.InternalElement;
            Autodesk.Revit.DB.Location location = revitElement.Location;
            if (location is Autodesk.Revit.DB.LocationPoint)
            {
                XYZ pt = (location as LocationPoint).Point;
                return pt.ToPoint();
            }
            else if (location is Autodesk.Revit.DB.LocationCurve)
            {
                Autodesk.Revit.DB.Curve curve = (location as LocationCurve).Curve;
                return curve.ToProtoType();
            }
            else
            {
                // IS THIS RIGHT? 
                return null;
            }
            
        }
    }
}
