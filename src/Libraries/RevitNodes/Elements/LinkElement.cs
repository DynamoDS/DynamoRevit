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
using Autodesk.DesignScript.Runtime;
using System.ComponentModel;

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
       
        public static object GetGeometry(Element linkElement, string detailLevel = "Medium")
        {
            // how to set the info state? 
            var info = Dynamo.Graph.Nodes.ElementState.Info;
            info = ElementState.Info;
            Autodesk.Revit.DB.Transform linkTransform = LinkTransform(linkElement);
            var geoSet = new List<object>();
            Autodesk.Revit.DB.Element revitElement = linkElement.InternalElement;

            Options options = new Options();
            switch (detailLevel) {
                case "Coarse":
                    options.DetailLevel = ViewDetailLevel.Coarse; 
                    break;
                case "Medium":
                    options.DetailLevel = ViewDetailLevel.Medium;
                    break;
                case "Fine":
                    options.DetailLevel = ViewDetailLevel.Fine;
                    break;
                default:
                    options.DetailLevel = ViewDetailLevel.Undefined;
                    break;
            }
            GeometryElement linkElementGeometry = revitElement.get_Geometry(options);

            foreach (var geometryInstance in linkElementGeometry)
            {
                switch (geometryInstance)
                {
                    case PolyLine p:
                        var transformedLine = p.GetTransformed(linkTransform);
                        geoSet.Add(transformedLine.ToProtoType());
                        break;
                    case Autodesk.Revit.DB.Mesh m:
                        var protoMesh = m.ToProtoType();
                        var CS = linkTransform.ToCoordinateSystem();
                        var meshVertices = protoMesh.VertexPositions;
                        var meshIndices = protoMesh.FaceIndices;
                        var newVertList = new List<Point>();

                        foreach (Point vertex in meshVertices)
                        {
                            var transformedVertex = (Point)vertex.Transform(CS);
                            newVertList.Add(transformedVertex);
                        }

                        var newMesh = Autodesk.DesignScript.Geometry.Mesh.ByPointsFaceIndices(newVertList, meshIndices);
                        geoSet.Add(newMesh);
                        break;
                    case GeometryInstance gi:
                        var instanceGeometry = gi.GetInstanceGeometry(linkTransform);
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
                        break;
                    case Autodesk.Revit.DB.Solid s:
                        //if (!(geometryInstance.Id == -1)) break;
                        var transformedSolid = Autodesk.Revit.DB.SolidUtils.CreateTransformed(s, linkTransform);
                        geoSet.Add(transformedSolid.ToProtoType());
                        break;
                    case null:
                        geoSet.Add(null); // Add null in case we didn't find anything?
                        break;
                    default:
                        break;
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
            var linkedElementGeometry = (List<object>)GetGeometry(linkElement);
            var geoList = new List<Geometry>();

            foreach (var geo in linkedElementGeometry)
            {
                if (geo is Geometry)
                {
                    geoList.Add(geo as Geometry);
                }
            }

            if (!geoList.Any())
                return null;

            var minBBOx = Autodesk.DesignScript.Geometry.BoundingBox.ByMinimumVolume(geoList);

            return minBBOx;
        }
        #endregion

        #region RayBounce

        /// <summary>
        /// Returns positions and elements hit by ray bounce from the specified origin point and direction
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="maxBounces"></param>
        /// <param name="view"></param>
        /// <param name="findLinkedLements"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "points", "elements" })]
        public static Dictionary<string, object> ByRayBounce(Point origin, Autodesk.DesignScript.Geometry.Vector direction, int maxBounces, Elements.Views.View3D view)
        {
            var startpt = origin.ToXyz();
            var rayCount = 0;

            var bouncePts = new List<Point> { origin };
            var bounceElements = new List<Elements.Element>();

            for (int ctr = 1; ctr <= maxBounces; ctr++)
            {
                var referenceIntersector = new ReferenceIntersector((View3D)view.InternalElement);
                referenceIntersector.FindReferencesInRevitLinks = true;
                IList<ReferenceWithContext> references = referenceIntersector.Find(startpt, direction.ToXyz())
                    .Where(r => r.GetReference().LinkedElementId != ElementId.InvalidElementId)
                    .ToList();

                ReferenceWithContext rClosest = null;
                rClosest = FindClosestReference(references);
                if (rClosest == null)
                {
                    break;
                }
                else
                {
                    var reference = rClosest.GetReference();
                    var linkedRef = reference.CreateReferenceInLink();
                    var linkedInstance = DocumentManager.Instance.CurrentDBDocument.GetElement(reference.ElementId) as Autodesk.Revit.DB.RevitLinkInstance;
                    var linkedReferenceElement = linkedInstance.GetLinkDocument().GetElement(reference.LinkedElementId);
                    var referenceObject = linkedReferenceElement.GetGeometryObjectFromReference(linkedRef);

                    bounceElements.Add(linkedReferenceElement.ToDSType(true));
                    var endpt = reference.GlobalPoint;
                    if (startpt.IsAlmostEqualTo(endpt))
                    {
                        break;
                    }
                    else
                    {
                        rayCount = rayCount + 1;
                        var currFace = referenceObject as Autodesk.Revit.DB.Face;
                        var endptUV = reference.UVPoint;
                        var FaceNormal = currFace.ComputeDerivatives(endptUV).BasisZ;  // face normal where ray hits
                        FaceNormal = rClosest.GetInstanceTransform().OfVector(FaceNormal); // transformation to get it in terms of document coordinates instead of the parent symbol
                        var directionMirrored = direction.ToXyz() - 2 * direction.ToXyz().DotProduct(FaceNormal) * FaceNormal; //http://www.fvastro.org/presentations/ray_tracing.htm
                        direction = directionMirrored.ToVector(); // get ready to shoot the next ray
                        startpt = endpt;
                        bouncePts.Add(endpt.ToPoint());
                    }
                }
            }

            return new Dictionary<string, object>
            {
                { "points", bouncePts },
                { "elements", bounceElements }
            };
        }

        /// <summary>
        /// Find the first intersection with a face
        /// </summary>
        /// <param name="references"></param>
        /// <returns></returns>
        private static ReferenceWithContext FindClosestReference(IEnumerable<ReferenceWithContext> references)
        {
            ReferenceWithContext rClosest = null;

            var face_prox = Double.PositiveInfinity;
            var edge_prox = Double.PositiveInfinity;

            foreach (ReferenceWithContext r in references)
            {
                var reference = r.GetReference();
                var linkedRef = reference.CreateReferenceInLink();
                var linkedInstance = DocumentManager.Instance.CurrentDBDocument.GetElement(reference.ElementId) as Autodesk.Revit.DB.RevitLinkInstance;
                var linkedReferenceElement = linkedInstance.GetLinkDocument().GetElement(reference.LinkedElementId);
                var referenceGeometryObject = linkedReferenceElement.GetGeometryObjectFromReference(linkedRef);

                Autodesk.Revit.DB.Face currFace = null;
                currFace = referenceGeometryObject as Autodesk.Revit.DB.Face;
                Autodesk.Revit.DB.Edge edge = null;
                edge = referenceGeometryObject as Autodesk.Revit.DB.Edge;
                if (currFace != null)
                {
                    if ((r.Proximity < face_prox) && (r.Proximity > Double.Epsilon))
                    {
                        rClosest = r;
                        face_prox = Math.Abs(r.Proximity);
                    }
                }
                else if (edge != null)
                {
                    if ((r.Proximity < edge_prox) && (r.Proximity > Double.Epsilon))
                    {
                        edge_prox = Math.Abs(r.Proximity);
                    }
                }
            }
            if (edge_prox <= face_prox)
            {
                // stop bouncing if there is an edge at least as close as the nearest face - there is no single angle of reflection for a ray striking a line
                //m_outputInfo.Add("there is an edge at least as close as the nearest face - there is no single angle of reflection for a ray striking a line");
                rClosest = null;
            }

            return rClosest;
        }

        #endregion
    }
}
