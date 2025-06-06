using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
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

        /// <summary>
        /// Internal constructor for the AdaptiveComponent wrapper
        /// </summary>
        /// <param name="pts">Points to use as reference</param>
        /// <param name="f">Face to use as reference</param>
        /// <param name="ft">familyType to place</param>
        private AdaptiveComponent(double[][] pts, ElementFaceReference f, FamilyType ft)
        {
            SafeInit(() => InitAdaptiveComponent(pts, f, ft));
        }

        /// <summary>
        /// Internal constructor for the AdaptiveComponent wrapper
        /// </summary>
        /// <param name="parms">Params on curve to reference</param>
        /// <param name="c">Curve to use as reference</param>
        /// <param name="ft">familyType to place</param>
        private AdaptiveComponent(double[] parms, Reference c, FamilyType ft)
        {
            SafeInit(() => InitAdaptiveComponent(parms, c, ft));
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

        /// <summary>
        /// Initialize an AdaptiveComponent element
        /// </summary>
        /// <param name="uvParams">Parameters to use as reference</param>
        /// <param name="f">Face to use as reference</param>
        /// <param name="ft">familyType to place</param>
        private void InitAdaptiveComponent(double[][] uvParams, ElementFaceReference f, FamilyType ft)
        {
            // if the family instance is present in trace...
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            // just mutate it...
            if (oldFam != null)
            {
                InternalSetFamilyInstance(oldFam);
                if (ft.InternalFamilySymbol.Id != oldFam.Symbol.Id)
                    InternalSetFamilySymbol(ft);
                InternalSetUvsAndFace(uvParams.ToUvs(), f.InternalReference);

                return;
            }

            // otherwise create a new family instance...
            TransactionManager.Instance.EnsureInTransaction(Document);

            using (Autodesk.Revit.DB.SubTransaction st = new SubTransaction(Document))
            {
                try
                {
                    st.Start();
                    var fam = AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance(Element.Document, ft.InternalFamilySymbol);
                    InternalSetFamilyInstance(fam);
                    InternalSetUvsAndFace(uvParams.ToUvs(), f.InternalReference);
                    st.Commit();
                }
                catch (Exception ex)
                {
                    st.RollBack();
                    throw new ArgumentException(Revit.Properties.Resources.Adaptive_Component_Creation_Failed + ex.Message);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(InternalElement);

        }

        /// <summary>
        /// Initialize an AdaptiveComponent element
        /// </summary>
        /// <param name="parms">Params on curve to reference</param>
        /// <param name="c">Curve to use as reference</param>
        /// <param name="ft">familyType to place</param>
        private void InitAdaptiveComponent(double[] parms, Reference c, FamilyType ft)
        {
            // if the family instance is present in trace...
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            // just mutate it...
            if (oldFam != null)
            {
                InternalSetFamilyInstance(oldFam);
                if (ft.InternalFamilySymbol.Id != oldFam.Symbol.Id)
                    InternalSetFamilySymbol(ft);
                InternalSetParamsAndCurve(parms, c);

                return;
            }

            // otherwise create a new family instance...
            TransactionManager.Instance.EnsureInTransaction(Document);

            using (Autodesk.Revit.DB.SubTransaction st = new SubTransaction(Document))
            {
                try
                {
                    st.Start();
                    var fam = AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance(Element.Document, ft.InternalFamilySymbol);
                    InternalSetFamilyInstance(fam);
                    InternalSetParamsAndCurve(parms, c);
                    st.Commit();
                }
                catch (Exception ex)
                {
                    st.RollBack();
                    throw new ArgumentException(Revit.Properties.Resources.Adaptive_Component_Creation_Failed + ex.Message);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(InternalElement);
        }

        #endregion

        #region Internal mutators

        /// <summary>
        /// Update the placement points of the adaptive component instance to the given points
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="pnts"></param>
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

        /// <summary>
        /// Set the family symbol for the internal family instance 
        /// </summary>
        /// <param name="ft"></param>
        private void InternalSetFamilySymbol(FamilyType ft)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            InternalFamilyInstance.Symbol = ft.InternalFamilySymbol;

            TransactionManager.Instance.TransactionTaskDone();

        }

        /// <summary>
        /// Set the positions of the InternalFamilyInstace from an array of uvs
        /// </summary>
        /// <param name="uvs"></param>
        /// <param name="faceReference"></param>
        private void InternalSetUvsAndFace(Autodesk.Revit.DB.UV[] uvs, Autodesk.Revit.DB.Reference faceReference)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(InternalFamilyInstance);

            if (placePointIds.Count() != uvs.Length)
                throw new Exception(Properties.Resources.InputUVParamsMismatch);

            // Set the position of each placement point
            int i = 0;
            foreach (var id in placePointIds)
            {
                var uv = uvs[i];
                var point = Document.GetElement(id) as Autodesk.Revit.DB.ReferencePoint;
                var peref = Document.Application.Create.NewPointOnFace(faceReference, uv);
                point.SetPointElementReference(peref);
                i++;
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Set the positions of the InternalFamilyInstace from an array of parameters and curve
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="c"></param>
        private void InternalSetParamsAndCurve(double[] parms, Autodesk.Revit.DB.Reference c)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(InternalFamilyInstance);

            if (placePointIds.Count() != parms.Length)
                throw new Exception(Properties.Resources.InputParamsMismatch);

            // Set the position of each placement point
            int i = 0;
            foreach (ElementId id in placePointIds)
            {
                var t = parms[i];
                var point = Document.GetElement(id) as Autodesk.Revit.DB.ReferencePoint;
                var ploc = new PointLocationOnCurve(PointOnCurveMeasurementType.NormalizedCurveParameter, t,
                                                    PointOnCurveMeasureFrom.Beginning);
                var peref = Document.Application.Create.NewPointOnEdge(c, ploc);
                point.SetPointElementReference(peref);
                i++;
            }

            TransactionManager.Instance.TransactionTaskDone();
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
        public static AdaptiveComponent ByParametersOnFace(double[][] uvs, ElementFaceReference faceReference, FamilyType familyType)
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

            return new AdaptiveComponent(uvs, ElementFaceReference.TryGetFaceReference(faceReference), familyType);
        }

        [IsVisibleInDynamoLibrary(false)]
        internal static AdaptiveComponent ByParametersOnFace(Autodesk.DesignScript.Geometry.UV[] uvs, ElementFaceReference faceReference, FamilyType familyType)
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

            return new AdaptiveComponent(uvs.Select(x => new[] { x.U, x.V }).ToArray(), ElementFaceReference.TryGetFaceReference(faceReference), familyType);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static AdaptiveComponent ByParametersOnFace(double[][] uvs, Surface surface, FamilyType familyType)
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

            return new AdaptiveComponent(uvs, ElementFaceReference.TryGetFaceReference(surface), familyType);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static AdaptiveComponent ByParametersOnCurveReference(double[] parameters, Revit.Elements.Element revitCurve, FamilyType familyType)
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

            return new AdaptiveComponent(parameters, ElementCurveReference.TryGetCurveReference(revitCurve).InternalReference, familyType);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static AdaptiveComponent ByParametersOnCurveReference(double[] parameters, ElementCurveReference revitCurve, FamilyType familyType)
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

            return new AdaptiveComponent(parameters, ElementCurveReference.TryGetCurveReference(revitCurve).InternalReference, familyType);
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
        /// Create an adaptive component by uv points on a face.
        /// </summary>
        /// <param name="uvs">An array of UV pairs</param>
        /// <param name="surface">The surface on which to place the AdaptiveComponent</param>
        /// <param name="familyType"></param>
        /// <returns></returns>
        public static AdaptiveComponent ByParametersOnFace(Autodesk.DesignScript.Geometry.UV[] uvs, Surface surface, FamilyType familyType)
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

            return new AdaptiveComponent(uvs.Select(x => new[] { x.U, x.V }).ToArray(), ElementFaceReference.TryGetFaceReference(surface), familyType);
        }

        /// <summary>
        /// Create an adaptive component referencing the parameters on a Curve reference
        /// </summary>
        /// <param name="parameters">The parameters on the curve</param>
        /// <param name="curve">The curve to reference</param>
        /// <param name="familyType">The family type to construct</param>
        /// <returns></returns>
        public static AdaptiveComponent ByParametersOnCurveReference(double[] parameters, Autodesk.DesignScript.Geometry.Curve curve, FamilyType familyType)
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

            return new AdaptiveComponent(parameters, ElementCurveReference.TryGetCurveReference(curve).InternalReference, familyType);
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
                var elementManager = ElementIDLifecycleManager<long>.GetInstance();
                foreach (var component in components)
                {
                    elementManager.UnRegisterAssociation(component.InternalElementId.Value, component);
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
