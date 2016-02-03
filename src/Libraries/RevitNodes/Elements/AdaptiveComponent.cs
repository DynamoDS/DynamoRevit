using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using Revit.GeometryObjects;
using Revit.GeometryReferences;
using RevitServices.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Point = Autodesk.DesignScript.Geometry.Point;
using Reference = Autodesk.Revit.DB.Reference;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit Adaptive Component
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class AdaptiveComponent : AbstractFamilyInstance
    {
        #region Private constructors

        /// <summary>
        /// Internal constructor for existing Elements.
        /// </summary>
        /// <param name="familyInstance"></param>
        private AdaptiveComponent(Autodesk.Revit.DB.FamilyInstance familyInstance)
        {
            SafeInit(() => InitAdaptiveComponent(familyInstance));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize an AdaptiveComponent element
        /// </summary>
        /// <param name="familyInstance"></param>
        private void InitAdaptiveComponent(Autodesk.Revit.DB.FamilyInstance familyInstance)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            InternalSetFamilyInstance(familyInstance);
            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Internal mutators

        /// <summary>
        /// Update the placement points of the adaptive component instance to the given points
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="pnt"></param>
        private static void UpdatePlacementPoints(Autodesk.Revit.DB.FamilyInstance fi, XYZ[] pnts)
        {
            IList<ElementId> placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(fi);

            if (placePointIds.Count() != pnts.Count())
                throw new Exception(Properties.Resources.InputPointParamsMismatch);

            int count = placePointIds.Count;
            for (int i = 0; i < count; i++ )
            {
                var point = (Autodesk.Revit.DB.ReferencePoint)Document.GetElement(placePointIds[i]);
                point.Position = pnts[i];
            }
        }

        #endregion

        #region Public properties
        /// <search>
        /// symbol
        /// </search>
        public new FamilyType Type
        {
            get
            {
                return FamilyType.FromExisting(this.InternalFamilyInstance.Symbol, true);
            }
        }

        public List<Point> Locations
        {
            get
            {
                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                DocumentManager.Regenerate();
                TransactionManager.Instance.TransactionTaskDone();

                var pts = new List<Point>();
                if (AdaptiveComponentInstanceUtils.IsAdaptiveComponentInstance(InternalFamilyInstance))
                {
                    var ids = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(InternalFamilyInstance);
                    foreach (var id in ids)
                    {
                        var p = DocumentManager.Instance.CurrentDBDocument.GetElement(id) as Autodesk.Revit.DB.ReferencePoint;
                        pts.Add(p.Position.ToPoint());
                    }
                }
                else
                {
                    var ptRefs = InternalFamilyInstance.GetFamilyPointPlacementReferences();
                    pts.AddRange(ptRefs.Select(x =>
                    {
                        var xyz = x.Location.Origin;
                        return Point.ByCoordinates(xyz.X, xyz.Y, xyz.Z);
                    }));
                }
                return pts;
            }
        }

        #endregion

        #region Hidden public static constructors

        // These constructors are hidden, but allow this constructor to be more tolerant
        // without breaking replication guides

        [IsVisibleInDynamoLibrary(false)]
        [AllowRankReduction]
        public static AdaptiveComponent[] ByParametersOnFace(double[][][] uvs, ElementFaceReference faceReference, FamilyType familyType)
        {
            if (uvs == null)
            {
                throw new ArgumentNullException("uvs");
            }

            if (faceReference == null)
            {
                throw new ArgumentNullException("faceReference");
            }

            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            var result = GeometryObjectSelector.ByReferenceStableRepresentation(
                        faceReference.InternalReference.ConvertToStableRepresentation(Document));
            var surface = result as Surface;
            if (surface == null)
            {
                var surfaces = result as List<Surface>;
                if (surfaces != null && surfaces.Count() == 1)
                {
                    surface = surfaces[0];
                }
                else if (surfaces.Count() > 1)
                {
                    throw new ArgumentException(Revit.Properties.Resources.MultipleSurfacesIntroducedAfterConversion);
                }
            }
            if (surface == null)
            {
                throw new ArgumentException("faceReference");
            }

            return InternalByPoints(uvs.Select(x => x.Select(y => surface.PointAtParameter(y[0], y[1])).ToArray()).
                ToArray(), familyType);
        }

        [IsVisibleInDynamoLibrary(false)]
        [AllowRankReduction]
        public static AdaptiveComponent[] ByParametersOnFace(Autodesk.DesignScript.Geometry.UV[][] uvs, ElementFaceReference faceReference, FamilyType familyType)
        {
            if (uvs == null)
            {
                throw new ArgumentNullException("uvs");
            }

            if (faceReference == null)
            {
                throw new ArgumentNullException("faceReference");
            }

            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            var result = GeometryObjectSelector.ByReferenceStableRepresentation(
                        faceReference.InternalReference.ConvertToStableRepresentation(Document));
            var surface = result as Surface;
            if (surface == null)
            {
                var surfaces = result as List<Surface>;
                if (surfaces != null && surfaces.Count() == 1)
                {
                    surface = surfaces[0];
                }
                else if (surfaces.Count() > 1)
                {
                    throw new ArgumentException(Revit.Properties.Resources.MultipleSurfacesIntroducedAfterConversion);
                }
            }
            if (surface == null)
            {
                throw new ArgumentException("faceReference");
            }

            return InternalByPoints(uvs.Select(x => x.Select(y => surface.PointAtParameter(y.U, y.V)).ToArray()).ToArray(), familyType);
        }

        [IsVisibleInDynamoLibrary(false)]
        [AllowRankReduction]
        public static AdaptiveComponent[] ByParametersOnFace(double[][][] uvs, Surface surface, FamilyType familyType)
        {
            if (uvs == null)
            {
                throw new ArgumentNullException("uvs");
            }

            if (surface == null)
            {
                throw new ArgumentNullException("surface");
            }

            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            return InternalByPoints(uvs.Select(x => x.Select(y => surface.PointAtParameter(y[0], y[1])).ToArray()).
                ToArray(), familyType);
        }

        [IsVisibleInDynamoLibrary(false)]
        [AllowRankReduction]
        public static AdaptiveComponent[] ByParametersOnCurveReference(double[][] parameters, Revit.Elements.Element revitCurve, FamilyType familyType)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (revitCurve == null)
            {
                throw new ArgumentNullException("revitCurve");
            }

            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            var curves = revitCurve.Curves;
            if (curves == null || curves.Length == 0)
            {
                throw new ArgumentException("revitCurve");
            }

            if (curves.Length > 1)
            {
                throw new ArgumentException(Revit.Properties.Resources.MultipleCurvesIntroducedAfterConversion);
            }

            return InternalByPoints(parameters.Select(x => x.Select(y => curves[0].PointAtParameter(y)).ToArray()).ToArray(), familyType);
        }

        [IsVisibleInDynamoLibrary(false)]
        [AllowRankReduction]
        public static AdaptiveComponent[] ByParametersOnCurveReference(double[][] parameters, ElementCurveReference revitCurve, FamilyType familyType)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (revitCurve == null)
            {
                throw new ArgumentNullException("revitCurve");
            }

            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            var result = GeometryObjectSelector.ByReferenceStableRepresentation(
                        revitCurve.InternalReference.ConvertToStableRepresentation(Document));
            var curve = result as Autodesk.DesignScript.Geometry.Curve;
            if (curve == null)
            {
                var curves = result as List<Autodesk.DesignScript.Geometry.Curve>;
                if (curves != null && curves.Count() == 1)
                {
                    curve = curves[0];
                }
                else if (curves.Count() > 1)
                {
                    throw new ArgumentException(Revit.Properties.Resources.MultipleCurvesIntroducedAfterConversion);
                }
            }
            if (curve == null)
            {
                throw new ArgumentException("revitCurve");
            }

            return InternalByPoints(parameters.Select(x => x.Select(y => curve.PointAtParameter(y)).ToArray()).ToArray(), familyType);
        }

        #endregion

        #region Static constructors

        /// <summary>
        /// Create a list of adaptive components from two-dimensional array of points
        /// </summary>
        /// <param name="points">a two-dimensional array of points</param>
        /// <param name="familyType">a family type to use to create the adaptive components</param>
        /// <returns></returns>
        [AllowRankReduction]
        public static AdaptiveComponent[] ByPoints(Point[][] points, FamilyType familyType)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            if (familyType == null)
            {
                throw new ArgumentNullException("familtType");
            }

            return InternalByPoints(points, familyType);
        }

        /// <summary>
        /// Create a list of adaptive components from two-dimensional array of uv points on a face.
        /// </summary>
        /// <param name="uvs">a two-dimensional array of UV pairs</param>
        /// <param name="surface">a surface on which to place the adaptive component</param>
        /// <param name="familyType"></param>
        /// <returns></returns>
        [AllowRankReduction]
        public static AdaptiveComponent[] ByParametersOnFace(Autodesk.DesignScript.Geometry.UV[][] uvs, Surface surface, FamilyType familyType)
        {
            if (uvs == null)
            {
                throw new ArgumentNullException("uvs");
            }

            if (surface == null)
            {
                throw new ArgumentNullException("surface");
            }

            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            var points = uvs.Select(x => x.Select(y => surface.PointAtParameter(y.U, y.V)).ToArray()).ToArray();
            return InternalByPoints(points, familyType);
        }

        /// <summary>
        /// Create a list of adaptive components from two-dimensional array of parameters on a curve.
        /// </summary>
        /// <param name="parameters">a two-dimensional parameters on the curve</param>
        /// <param name="curve">a curve to reference</param>
        /// <param name="familyType">a family type to construct</param>
        /// <returns></returns>
        [AllowRankReduction]
        public static AdaptiveComponent[] ByParametersOnCurveReference(double[][] parameters, Autodesk.DesignScript.Geometry.Curve curve, FamilyType familyType)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }

            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            var points = parameters.Select(x => x.Select(y => curve.PointAtParameter(y)).ToArray()).ToArray();
            return InternalByPoints(points, familyType);
        }

        /// <summary>
        /// Create a list of adaptive components from two-dimensional array of points
        /// </summary>
        /// <param name="points">a two-dimensional array of points</param>
        /// <param name="familyType">a family type to use to create the adaptive components</param>
        /// <returns></returns>
        private static AdaptiveComponent[] InternalByPoints(Point[][] points, FamilyType familyType)
        {
            var oldInstances = ElementBinder.GetElementsFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);
            int countToBeCreated = points.Count();
            int countOfOldInstances = 0;
            if (oldInstances != null) countOfOldInstances = oldInstances.Count();
            int reusableCount = Math.Min(countToBeCreated, countOfOldInstances);

            TransactionManager.Instance.EnsureInTransaction(Document);

            List<Autodesk.Revit.DB.FamilyInstance> instances = new List<Autodesk.Revit.DB.FamilyInstance>();
            List<AdaptiveComponent> components = new List<AdaptiveComponent>();

            try
            {
                // Reuse the adaptive components that can be reused if possible
                for (int i = 0; i < reusableCount; i++)
                {
                    var fi = oldInstances.ElementAt(i);
                    var comp = new AdaptiveComponent(fi);
                    components.Add(comp);

                    //Update the family symbol
                    if (familyType.InternalFamilySymbol.Id != fi.Symbol.Id)
                    {
                        fi.Symbol = familyType.InternalFamilySymbol;
                    }

                    UpdatePlacementPoints(fi, points[i].ToXyzs());
                    instances.Add(fi);
                }

                // Delete the redundant adaptive components if any
                for (int i = reusableCount; i < countOfOldInstances; i++)
                {
                    var fi = oldInstances.ElementAt(i);
                    Document.Delete(fi.Id);

                }

                // Create new adaptive components
                if (countToBeCreated > countOfOldInstances)
                {
                    var remainingPoints = points.Skip(reusableCount).ToArray();
                    // Prepare the creation data for batch processing
                    int numOfComponents = remainingPoints.Count();
                    List<FamilyInstanceCreationData> creationDatas = new List<FamilyInstanceCreationData>(numOfComponents);
                    for (int i = 0; i < numOfComponents; ++i)
                    {
                        int numOfPoints = remainingPoints[i].Length;
                        var aPoints = remainingPoints[i].ToXyzs();

                        var creationData = DocumentManager.Instance.CurrentUIApplication.Application.Create.
                            NewFamilyInstanceCreationData(familyType.InternalFamilySymbol, aPoints);

                        if (creationData != null)
                        {
                            creationDatas.Add(creationData);
                        }
                    }

                    // Create elements based on the creation data in a batch
                    ICollection<ElementId> elements;
                    if (creationDatas.Count > 0)
                    {
                        if (Document.IsFamilyDocument)
                        {
                            elements = DocumentManager.Instance.CurrentDBDocument.FamilyCreate.NewFamilyInstances2(creationDatas);
                        }
                        else
                        {
                            elements = DocumentManager.Instance.CurrentDBDocument.Create.NewFamilyInstances2(creationDatas);
                        }

                        foreach (var id in elements)
                        {
                            Autodesk.Revit.DB.FamilyInstance instance;
                            if (ElementUtils.TryGetElement<Autodesk.Revit.DB.FamilyInstance>(
                                DocumentManager.Instance.CurrentDBDocument, id, out instance))
                            {
                                instances.Add(instance);
                                components.Add(new AdaptiveComponent(instance));
                            }
                        }
                    }
                }

                ElementBinder.SetElementsForTrace(instances);
            }
            catch(Exception e)
            {
                // Unregister the elements from the element life cycle manager and delete the elements
                var elementManager = ElementIDLifecycleManager<int>.GetInstance();
                foreach (var component in components)
                {
                    elementManager.UnRegisterAssociation(component.InternalElementId.IntegerValue, component);
                }
                foreach (var instance in instances)
                {
                    Document.Delete(instance.Id);
                }

                if (e is Autodesk.Revit.Exceptions.ArgumentException)
                {
                    throw new ArgumentException("The arguments have issues", e);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                TransactionManager.Instance.TransactionTaskDone();
            }

            return components.ToArray();
        }

        #endregion
        
        #region Internal static constructor

        /// <summary>
        /// Construct from an existing instance of an AdaptiveComponent. 
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static AdaptiveComponent FromExisting(Autodesk.Revit.DB.FamilyInstance familyInstance, bool isRevitOwned)
        {
            if (familyInstance == null)
            {
                throw new ArgumentNullException("familyInstance");
            }

            // Not all family instances are adaptive components
            if (!AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol(familyInstance))
            {
                throw new Exception(Properties.Resources.NotAdaptiveComponentError);
            }

            return new AdaptiveComponent(familyInstance)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
