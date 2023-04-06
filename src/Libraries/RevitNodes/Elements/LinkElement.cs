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
using Autodesk.Revit.UI;
using System.Windows.Media.Media3D;


namespace Revit.Elements
{ 
    /// <summary>
    /// A Revit Link Instance
    /// </summary>
    public static class LinkElement
    {

        #region Helpers
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

        // helper for zooming to clicked green Id
        internal static void ZoomToLinkedElement(Element element)
        {
                    
            UIDocument uiDoc = DocumentManager.Instance.CurrentUIDocument;
            Autodesk.Revit.DB.View activeView = uiDoc.ActiveView;
            // get active UI view to use
            UIView uiview = uiDoc.GetOpenUIViews().FirstOrDefault<UIView>(uv => uv.ViewId.Equals(activeView.Id));
            var location = Revit.Elements.LinkElement.GetLinkElementLocation(element);
            Transform linkTransform = Revit.Elements.LinkElement.LinkTransform(element);
            
            // use the center of the BoundingBox as zoom center
            BoundingBoxXYZ bb = element.InternalElement.get_BoundingBox(null);
            XYZ bbCenter = (bb.Max + bb.Min) / 2;
            double zoomOffsetX = bb.Max.X - bbCenter.X;
            double zoomOffsetY= bb.Max.Y - bbCenter.Y;
            double zoomOffsetZ = bb.Max.Z - bbCenter.Z;
            XYZ locationPt = Revit.Elements.LinkElement.TransformPoint(bbCenter, linkTransform);
                        
            if (locationPt != null)
            {
                XYZ min = new XYZ(locationPt.X - zoomOffsetX, locationPt.Y - zoomOffsetY, locationPt.Z - zoomOffsetZ);
                XYZ max = new XYZ(locationPt.X + zoomOffsetX, locationPt.Y + zoomOffsetY, locationPt.Z + zoomOffsetZ);
                uiview.ZoomAndCenterRectangle(min, max);
            }


        }

        // return element location with transform
        internal static object GetLinkElementLocation(Element linkElement)
        {
            Autodesk.Revit.DB.Element revitElement = linkElement.InternalElement;
            Autodesk.Revit.DB.Location location = revitElement.Location;
            Autodesk.Revit.DB.Transform linkTransform = LinkTransform(linkElement);

            if (location is Autodesk.Revit.DB.LocationPoint)
            {
                var xyz = (location as LocationPoint).Point;
                var transformedxyz = TransformPoint(xyz, linkTransform);
                return transformedxyz;
            }
            else if (location is Autodesk.Revit.DB.LocationCurve)
            {
                var curve = (location as LocationCurve).Curve;
                var transformedCurve = curve.CreateTransformed(linkTransform);
                return transformedCurve;
            }
            else
            {
                return null;
            }

        }

        // helper method for transforming a point
        internal static XYZ TransformPoint(XYZ point, Transform transform)
        {
            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            //transform basis of the old coordinate system in the new coordinate // system
            XYZ b0 = transform.get_Basis(0);
            XYZ b1 = transform.get_Basis(1);
            XYZ b2 = transform.get_Basis(2);
            XYZ origin = transform.Origin;

            //transform the origin of the old coordinate system in the new 
            //coordinate system
            double xTemp = x * b0.X + y * b1.X + z * b2.X + origin.X;
            double yTemp = x * b0.Y + y * b1.Y + z * b2.Y + origin.Y;
            double zTemp = x * b0.Z + y * b1.Z + z * b2.Z + origin.Z;

            return new XYZ(xTemp, yTemp, zTemp);
        }
        #endregion


        #region Public methods
        // Action nodes
        public static Autodesk.DesignScript.Geometry.Geometry GetLocation(Element linkElement)
        {
            // get location (point or curve) with transform
            var location = GetLinkElementLocation(linkElement);
            if (location is XYZ)
            {
                return (location as XYZ).ToPoint();
            }
            else if (location is Autodesk.Revit.DB.Curve)
            {

                return (location as Autodesk.Revit.DB.Curve).ToProtoType();

            }
            else { return null; }


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

        #endregion


        #region Properties
        //Query nodes

        /// <summary>
        /// Return the Document of the given Linked element
        /// </summary>
        /// <returns name="linkDocument">linkDocument</returns>
        [NodeCategory("Query")]
        public static Revit.Application.Document Document(Element element)
        {
            Autodesk.Revit.DB.Element revitElement = element.InternalElement;

            var elementDocument = revitElement.Document;

            return new Revit.Application.Document(elementDocument);
        }

        /// <summary>
        /// Return the Link Transform
        /// </summary>
        [NodeCategory("Query")]
        public static Autodesk.Revit.DB.Transform LinkTransform(Element element)
        {
            var revitLinkInstances = GetLinkInstancesContainingLinkElement(element);

            foreach (var revitLinkInstance in revitLinkInstances)
            {
                return revitLinkInstance.GetTotalTransform();
            }
            return null;
        }

        /// <summary>
        /// Return the Inverted Link Transform
        /// </summary>
        [NodeCategory("Query")]
        public static Autodesk.Revit.DB.Transform LinkInverseTransform(Element element)
        {
            List<RevitLinkInstance> revitLinkInstances = GetLinkInstancesContainingLinkElement(element);

            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances)
            {
                return revitLinkInstance.GetTotalTransform().Inverse;
            }
            return null;

        }

        /// <summary>
        /// Return the Linked Element's Bounding Box
        /// </summary>
        /// <returns name="boundingBox">Bounding Box</returns>
        [NodeCategory("Query")]
        public static BoundingBox BoundingBox(Element linkElement)
        {

            List<object> linkedElementGeometry = (List<object>)GetGeometry(linkElement);
                   
            var geoList = new List<Geometry>();
            
            foreach (var geo in linkedElementGeometry)
                if (geo is Geometry)
                {
                    geoList.Add(geo as Geometry);
                }

            var minBBOx = Autodesk.DesignScript.Geometry.BoundingBox.ByMinimumVolume(geoList);
           
            return minBBOx;
        }
        #endregion
    }
}
