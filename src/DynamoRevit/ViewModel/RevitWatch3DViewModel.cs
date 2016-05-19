using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.Revit.DB;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.Wpf.ViewModels.Watch3D;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Threading;
using RevitServices.Transactions;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using Line = Autodesk.Revit.DB.Line;
using Point = Autodesk.DesignScript.Geometry.Point;
using Resources = Dynamo.Applications.Properties.Resources;
using Solid = Autodesk.DesignScript.Geometry.Solid;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace Dynamo.Applications.ViewModel
{
    public class RevitWatch3DViewModel : DefaultWatch3DViewModel
    {
        private ElementId keeperId = ElementId.InvalidElementId;
        private ElementId directShapeId = ElementId.InvalidElementId;
        private MethodInfo method;

        public override string PreferenceWatchName { get { return "IsRevitBackgroundPreviewActive"; } }

        public RevitWatch3DViewModel(Watch3DViewModelStartupParams parameters) : base(parameters)
        {
            Name = Resources.BackgroundPreviewName;
            Draw();
        }

        protected override void OnShutdown()
        {
            DynamoRevitApp.AddIdleAction(DeleteKeeperElement);
        }

        protected override void OnClear()
        {
            IdlePromise.ExecuteOnIdleAsync(
                () =>
                {
                    TransactionManager.Instance.EnsureInTransaction(
                        DocumentManager.Instance.CurrentDBDocument);

                    if (keeperId != ElementId.InvalidElementId)
                    {
                        DocumentManager.Instance.CurrentUIDocument.Document.Delete(keeperId);
                        keeperId = ElementId.InvalidElementId;
                    }

                    TransactionManager.Instance.ForceCloseTransaction();
                });
        }

        protected override void OnActiveStateChanged()
        {
            if (active)
            {
                Draw();
            }
            else
            {
                OnClear();
            }
        }

        protected override void OnEvaluationCompleted(object sender, EvaluationCompletedEventArgs e)
        {
            Draw();
        }

        protected override void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = sender as NodeModel;
            if (node == null)
            {
                return;
            }

            switch (e.PropertyName)
            {
                case "IsVisible":
                    Draw(node);
                    break;
            }
        }
        
        #region private methods

        private void Draw(NodeModel node = null)
        {
            if (!Active) return;
            IEnumerable<IGraphicItem> graphicItems = new List<IGraphicItem>();

            if (node != null)
            {
                if (node.IsVisible)
                {
                    graphicItems = node.GeneratedGraphicItems(engineManager.EngineController);
                }
            }
            else
            {
                graphicItems = model.CurrentWorkspace.Nodes
                 .Where(n => n.IsVisible)
                 .SelectMany(n => n.GeneratedGraphicItems(engineManager.EngineController));
            }

            var geoms = new List<GeometryObject>();
            foreach (var item in graphicItems)
            {
                RevitGeometryObjectFromGraphicItem(item, ref geoms);
            }

            Draw(geoms);
        }

        private void Draw(IEnumerable<GeometryObject> geoms)
        {
            if (method == null)
            {
                method = GetTransientDisplayMethod();

                if (method == null)
                    return;
            }

            IdlePromise.ExecuteOnIdleAsync(
                () =>
                {
                    TransactionManager.Instance.EnsureInTransaction(
                        DocumentManager.Instance.CurrentDBDocument);

                    if (keeperId != ElementId.InvalidElementId &&
                        DocumentManager.Instance.CurrentDBDocument.GetElement(keeperId) != null)
                    {
                        DocumentManager.Instance.CurrentUIDocument.Document.Delete(keeperId);
                        keeperId = ElementId.InvalidElementId;
                    }

                    var argsM = new object[4];
                    argsM[0] = DocumentManager.Instance.CurrentUIDocument.Document;
                    argsM[1] = ElementId.InvalidElementId;
                    argsM[2] = geoms;
                    argsM[3] = ElementId.InvalidElementId;
                    keeperId = (ElementId)method.Invoke(null, argsM);

                    TransactionManager.Instance.ForceCloseTransaction();
                });
        }

        private void RevitGeometryObjectFromGraphicItem(IGraphicItem item, ref List<GeometryObject> geoms)
        {
            var geom = item as PolyCurve;
            if (geom != null)
            {
                // We extract the curves explicitly rather than using PolyCurve's ToRevitType
                // extension method.  There is a potential issue with CurveLoop which causes
                // this method to introduce corrupt GNodes.  
                foreach (var c in geom.Curves())
                {
                    // Tesselate the curve.  This greatly improves performance when
                    // we're dealing with NurbsCurve's with high knot count, commonly
                    // results of surf-surf intersections.
                    geoms.AddRange(Tessellate(c));
                }
                        
                return;
            }

            var point = item as Point;
            if (point != null)
            {
                Autodesk.Revit.DB.Point pnt = null;
                try
                {
                    pnt = Autodesk.Revit.DB.Point.Create(point.ToXyz());
                }
                catch(Exception)
                {
                }
                finally
                {
                    if (pnt != null)
                    {
                        geoms.Add(pnt);
                    }
                }
                return;
            }

            var curve = item as Curve;
            if (curve != null)
            {
                // Tesselate the curve.  This greatly improves performance when
                // we're dealing with NurbsCurve's with high knot count, commonly
                // results of surf-surf intersections.
                geoms.AddRange(Tessellate(curve));
                return;
            }

            var surf = item as Surface;
            if (surf != null)
            {
                geoms.AddRange(Tessellate(surf));
                return;
            }

            var solid = item as Solid;
            if (solid != null)
            {
                geoms.AddRange(Tessellate(solid));
            }
        }

        /// <summary>
        /// Tessellate the curve:
        /// 1). If there are more than 2 points, create a polyline out of the points;
        /// 2). If there are exactly 2 points, create a line;
        /// 3). If there's exception thrown during the tessellation process, attempt to create 
        /// a line from start and end points. If that fails, a point will be created instead.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        private IEnumerable<GeometryObject> Tessellate(Curve curve)
        {
            var result = new List<GeometryObject>();
            try
            {
                // we scale the tesselation rather than the curve
                var conv = UnitConverter.DynamoToHostFactor(UnitType.UT_Length);

                // use the ASM tesselation of the curve
                var pkg = renderPackageFactory.CreateRenderPackage();
                curve.Tessellate(pkg, renderPackageFactory.TessellationParameters);

                // get necessary info to enumerate and convert the lines
                //var lineCount = pkg.LineVertexCount * 3 - 3;
                var verts = pkg.LineStripVertices.ToList();

                if (verts.Count > 2)
                {
                    var scaledXYZs = new List<XYZ>();
                    for (var i = 0; i < verts.Count; i += 3)
                    {
                        scaledXYZs.Add(new XYZ(verts[i] * conv, verts[i + 1] * conv, verts[i + 2] * conv));
                    }
                    result.Add(PolyLine.Create(scaledXYZs));
                }
                else if (verts.Count == 2)
                {
                    result.Add(Line.CreateBound(curve.StartPoint.ToXyz(), curve.EndPoint.ToXyz()));
                }
            }
            catch (Exception)
            {
                // Add a red bounding box geometry to identify that some errors occur
                var bbox = curve.BoundingBox;
                result.AddRange(ProtoToRevitMesh.CreateBoundingBoxMeshForErrors(bbox.MinPoint, bbox.MaxPoint));

                try
                {
                    result.Add(Line.CreateBound(curve.StartPoint.ToXyz(), curve.EndPoint.ToXyz()));
                }
                catch (Exception)
                {
                    try
                    {
                        result.Add(Autodesk.Revit.DB.Point.Create(curve.StartPoint.ToXyz()));
                    }
                    catch (ArgumentException)
                    {
                        //if either the X, Y or Z of the point is infinite, no need to add it for preview
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Tessellate the surface by calling the ToRevitType function.
        /// If it fails, each edge of the surface will be tessellated instead by calling
        /// the correspoinding Tessellate method.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        private List<GeometryObject> Tessellate(Surface surface)
        {
            List<GeometryObject> rtGeoms = new List<GeometryObject>();

            try
            {
                rtGeoms.AddRange(surface.ToRevitType());
            }
            catch (Exception)
            {
                // Add a red bounding box geometry to identify that some errors occur
                var bbox = surface.BoundingBox;
                rtGeoms.AddRange(ProtoToRevitMesh.CreateBoundingBoxMeshForErrors(bbox.MinPoint, bbox.MaxPoint));

                foreach (var edge in surface.Edges)
                {
                    if (edge != null)
                    {
                        var curveGeometry = edge.CurveGeometry;
                        rtGeoms.AddRange(Tessellate(curveGeometry));
                        curveGeometry.Dispose();
                        edge.Dispose();
                    }
                }
            }

            return rtGeoms;
        }

        /// <summary>
        /// Tessellate the solid by calling the ToRevitType function.
        /// If it fails, the surface geometry of each face of the solid will be tessellated
        /// instead by calling the correspoinding Tessellate method.
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        private List<GeometryObject> Tessellate(Solid solid)
        {
            List<GeometryObject> rtGeoms = new List<GeometryObject>();

            try
            {
                rtGeoms.AddRange(solid.ToRevitType());
            }
            catch(Exception)
            {
                // Add a red bounding box geometry to identify that some errors occur
                var bbox = solid.BoundingBox;
                rtGeoms.AddRange(ProtoToRevitMesh.CreateBoundingBoxMeshForErrors(bbox.MinPoint, bbox.MaxPoint));

                foreach (var face in solid.Faces)
                {
                    if (face != null)
                    {
                        var surfaceGeometry = face.SurfaceGeometry();
                        rtGeoms.AddRange(Tessellate(surfaceGeometry));
                        surfaceGeometry.Dispose();
                        face.Dispose();
                    }
                }
            }

            return rtGeoms;
        }

        /// <summary>
        /// This method access Revit API, therefore it needs to be called only 
        /// by idle thread (i.e. in an 'UIApplication.Idling' event handler).
        /// </summary>
        private void DeleteKeeperElement()
        {
            var dbDoc = DocumentManager.Instance.CurrentDBDocument;
            if (null == dbDoc)
                return;

            if (keeperId == ElementId.InvalidElementId) return;

            TransactionManager.Instance.EnsureInTransaction(dbDoc);
            DocumentManager.Instance.CurrentUIDocument.Document.Delete(keeperId);
            TransactionManager.Instance.ForceCloseTransaction();
        }

        internal static MethodInfo GetTransientDisplayMethod()
        {
            var geometryElementType = typeof(GeometryElement);
            var geometryElementTypeMethods =
                geometryElementType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            var method = geometryElementTypeMethods.FirstOrDefault(x => x.Name == "SetForTransientDisplay");

            return method;
        }
        #endregion
    }
}
