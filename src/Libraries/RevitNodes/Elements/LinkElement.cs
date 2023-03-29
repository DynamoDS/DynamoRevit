using Autodesk.Revit.DB;
using System;
using Revit.Elements;
using System.Collections.Generic;
using System.Linq;
using DynamoUnits;
using Dynamo.Graph.Nodes;
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

        public static Autodesk.Revit.DB.Transform LinkTransform(Element element) 
        {
            var revitLinkInstances = GetLinkInstancesContainingLinkElement(element);
                        
            foreach (var revitLinkInstance in revitLinkInstances)
            {
                return revitLinkInstance.GetTotalTransform();
            }
            return null;
        }
      
      
        public static Autodesk.Revit.DB.Transform LinkInverseTransform(Element element)
        {
            List<RevitLinkInstance> revitLinkInstances = GetLinkInstancesContainingLinkElement(element);

            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances) 
            {
                return revitLinkInstance.GetTotalTransform().Inverse;
            }
            return null;
                
        }
        
        public static BoundingBox BoundingBox (Element linkElement)
        {
            
            Autodesk.Revit.DB.Element revitElement =linkElement.InternalElement;
            BoundingBoxXYZ boundingBox = revitElement.get_BoundingBox(null);

            return boundingBox.ToProtoType();
        }

        public static object GetGeometry(Element linkElement)
        {
            // how to set the info state? 
            var info = Dynamo.Graph.Nodes.ElementState.Info;
            info = ElementState.Info;
            Autodesk.Revit.DB.Transform linkTransform = LinkTransform(linkElement);
            var geoSet = new List<object>();
            Autodesk.Revit.DB.Element revitElement = linkElement.InternalElement;
            Options options = new Options();
            GeometryElement linkElementGeometry = revitElement.get_Geometry(options);

            foreach (var geometryInstance in linkElementGeometry)
            {
                if (geometryInstance is PolyLine)
                {
                    geoSet.Add((geometryInstance as PolyLine).ToProtoType());
                }
                else if (geometryInstance is Autodesk.Revit.DB.Mesh)
                {
                    var mesh = geometryInstance as Autodesk.Revit.DB.Mesh;

                    geoSet.Add(mesh.ToProtoType());
                }
                else if (geometryInstance is GeometryInstance)
                {
                    var instanceGeometry = (geometryInstance as GeometryInstance).GetInstanceGeometry(linkTransform);
                    foreach (var geo in instanceGeometry)
                    {
                        if (geo is Autodesk.Revit.DB.Solid)
                        {
                            var solid = geo as Autodesk.Revit.DB.Solid;
                            if ((int)solid.Id != -1)
                            {

                                geoSet.Add(solid.ToProtoType());
                            }
                        }

                        else if (geo is Autodesk.Revit.DB.Curve)
                        {
                            var curve = geo as Autodesk.Revit.DB.Curve;

                            geoSet.Add(curve.ToProtoType());
                        }

                        else if (geo is Autodesk.Revit.DB.Mesh)
                        {
                            var mesh = geo as Autodesk.Revit.DB.Mesh;

                            geoSet.Add(mesh.ToProtoType());
                        }
                    }
                }
                else
                {
                    if ((geometryInstance is Autodesk.Revit.DB.Solid) && !(geometryInstance.Id == -1))
                        { geoSet.Add(geometryInstance); }
                    
                }
                    
            }


            return geoSet;
        }

        public static Autodesk.DesignScript.Geometry.Geometry GetLocation(Element linkElement) 
        {
            Autodesk.Revit.DB.Element revitElement = linkElement.InternalElement;
            Autodesk.Revit.DB.Location location = revitElement.Location;
            if (location is Autodesk.Revit.DB.LocationPoint)
            {
                var pt = (location as LocationPoint).Point;
                return pt.ToPoint();
            }
            else if (location is Autodesk.Revit.DB.LocationCurve)
            {
                var curve = (location as LocationCurve).Curve;
                return curve.ToProtoType();
            }
            else
            {
            
                return null;
            }
            
        }
    }
}
