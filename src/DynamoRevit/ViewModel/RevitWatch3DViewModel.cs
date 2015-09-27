using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.Revit.DB;
using Dynamo.Models;
using Dynamo.Utilities;
using Dynamo.Wpf.ViewModels.Watch3D;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Threading;
using RevitServices.Transactions;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using Line = Autodesk.Revit.DB.Line;
using Point = Autodesk.DesignScript.Geometry.Point;
using Solid = Autodesk.DesignScript.Geometry.Solid;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace Dynamo.Applications.ViewModel
{
    public class RevitWatch3DViewModel : Watch3DViewModelBase
    {
        private ElementId keeperId = ElementId.InvalidElementId;
        private ElementId directShapeId = ElementId.InvalidElementId;
        private MethodInfo method;

        private RevitWatch3DViewModel(Watch3DViewModelStartupParams parameters) : base(parameters)
        {
            
        }

        public static RevitWatch3DViewModel Start(Watch3DViewModelStartupParams parameters)
        {
            var vm = new RevitWatch3DViewModel(parameters);
            vm.OnStartup();
            return vm;
        }

        protected override void OnStartup()
        {
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
                    Draw();
                    break;
            }
        }
        
        #region private methods

        private void Draw()
        {
            var graphicItems = model.CurrentWorkspace.Nodes
                .Where(n => n.IsVisible)
                .SelectMany(n => n.GeneratedGraphicItems(engineManager.EngineController));

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
                geoms.Add(DocumentManager.Instance.CurrentUIApplication.Application.Create.NewPoint(point.ToXyz()));
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
                geoms.AddRange(surf.ToRevitType());
                return;
            }

            var solid = item as Solid;
            if (solid != null)
            {
                geoms.AddRange(solid.ToRevitType());
            }
        }

        private IEnumerable<GeometryObject> Tessellate(Curve curve)
        {
            var result = new List<GeometryObject>();

            // use the ASM tesselation of the curve
            var pkg = renderPackageFactory.CreateRenderPackage();
            curve.Tessellate(pkg, renderPackageFactory.TessellationParameters);

            // get necessary info to enumerate and convert the lines
            var lineCount = pkg.LineVertexCount * 3 - 3;
            var verts = pkg.LineStripVertices.ToList();

            // we scale the tesselation rather than the curve
            var conv = UnitConverter.DynamoToHostFactor(UnitType.UT_Length);

            // add the revit Lines to geometry collection
            for (var i = 0; i < lineCount; i += 3)
            {
                var xyz0 = new XYZ(verts[i] * conv, verts[i + 1] * conv, verts[i + 2] * conv);
                var xyz1 = new XYZ(verts[i + 3] * conv, verts[i + 4] * conv, verts[i + 5] * conv);

                result.Add(Line.CreateBound(xyz0, xyz1));
            }

            return result;
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
