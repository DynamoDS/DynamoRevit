using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using Pt = Autodesk.DesignScript.Geometry.Point;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    /// <summary>
    /// MassLevelData
    /// </summary>
    public class Roof : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the RoofBase
        /// </summary>
        internal Autodesk.Revit.DB.RoofBase InternalRoof
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalRoof; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Roof(Autodesk.Revit.DB.RoofBase roof)
        {
            SafeInit(() => InitRoof(roof));
        }
      
        /// <summary>
        /// Private constructor
        /// </summary>
        private Roof(CurveArray curves, Autodesk.Revit.DB.ReferencePlane reference, Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.RoofType roofType, double extrusionStart, double extrusionEnd)
        {
            SafeInit(() => InitRoof(curves, reference, level, roofType, extrusionStart, extrusionEnd));
        }

        private Roof(CurveArray curves, Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.RoofType roofType)
        {
            SafeInit(() => InitRoof(curves, level, roofType));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a roof element
        /// </summary>
        private void InitRoof(Autodesk.Revit.DB.RoofBase roof)
        {
            InternalSetRoof(roof);
        }

        /// <summary>
        /// Initialize a roof element
        /// </summary>
        private void InitRoof(CurveArray curves, Autodesk.Revit.DB.ReferencePlane reference, Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.RoofType roofType, double extrusionStart, double extrusionEnd)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.RoofBase roof = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.RoofBase>(Document);
            if (roof == null)
            {
                roof = Document.Create.NewExtrusionRoof(curves, reference, level, roofType, extrusionStart, extrusionEnd);
            }
            else
            {
                roof = Document.Create.NewExtrusionRoof(curves, reference, level, roofType, extrusionStart, extrusionEnd);
            }

            InternalSetRoof(roof);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalRoof);
        }

        /// <summary>
        /// Initialize a roof element
        /// </summary>
        private void InitRoof(CurveArray curves, Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.RoofType roofType)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.RoofBase roof = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.RoofBase>(Document);
            if (roof == null)
            {
                ModelCurveArray footprint = new ModelCurveArray();
                roof = Document.Create.NewFootPrintRoof(curves, level, roofType, out footprint);
            }
            else
            {
                ModelCurveArray footprint = new ModelCurveArray();
                roof = Document.Create.NewFootPrintRoof(curves, level, roofType, out footprint);
            }

            InternalSetRoof(roof);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalRoof);
        }

        #endregion

        #region Private mutators


        /// <summary>
        /// Set the InternalRoof property and the associated element id and unique id
        /// </summary>
        /// <param name="Roof"></param>
        private void InternalSetRoof(Autodesk.Revit.DB.RoofBase roof)
        {
            InternalRoof = roof;
            InternalElementId = roof.Id;
            InternalUniqueId = roof.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Roof given it's curve outline and Level
        /// </summary>
        /// <param name="outline"></param>
        /// <param name="RoofType"></param>
        /// <param name="level"></param>
        /// <returns>The Roof</returns>
        public static Roof ByOutlineTypeAndLevel(Curve[] outline, RoofType roofType, Level level)
        {
            var polycurve = PolyCurve.ByJoinedCurves(outline);

            if (!polycurve.IsClosed)
            {
                throw new ArgumentException(Properties.Resources.OpenInputPolyCurveError);
            }

            var ca = new CurveArray();
            polycurve.Curves().ForEach(x => ca.Append(x.ToRevitType()));

            var roof = new Roof(ca, level.InternalLevel,roofType.InternalRoofType);
            DocumentManager.Regenerate();
            return roof;
        }

        /// <summary>
        /// Extrude Roof by Outline, Referenceplane
        /// </summary>
        /// <param name="outline"></param>
        /// <param name="roofType"></param>
        /// <param name="level"></param>
        /// <param name="plane"></param>
        /// <param name="extrusionStart"></param>
        /// <param name="extrusionEnd"></param>
        /// <returns></returns>
        public static Roof ByOutlineExtrusionTypeAndLevel(PolyCurve outline, RoofType roofType, Level level, ReferencePlane plane, double extrusionStart, double extrusionEnd)
        {

            if (!outline.IsClosed)
            {
                throw new ArgumentException(Properties.Resources.OpenInputPolyCurveError);
            }

            var ca = new CurveArray();
            outline.Curves().ForEach(x => ca.Append(x.ToRevitType()));

            var roof = new Roof(ca, plane.InternalReferencePlane, level.InternalLevel, roofType.InternalRoofType, extrusionStart, extrusionEnd);
            DocumentManager.Regenerate();
            return roof;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Get Slab Shape Points
        /// </summary>
        public IEnumerable<Pt> Points
        {
            get 
            {
                if (this.InternalRoof.SlabShapeEditor == null)
                {
                    throw new Exception(Properties.Resources.InvalidShapeEditor);
                }

                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                this.InternalRoof.SlabShapeEditor.Enable();
                TransactionManager.Instance.TransactionTaskDone();

                List<Pt> points = new List<Pt>();
                foreach (SlabShapeVertex v in this.InternalRoof.SlabShapeEditor.SlabShapeVertices)
                {
                    points.Add(v.Position.ToPoint());
                }
                return points;
            }
        }

        /// <summary>
        /// Add Point to Slab Shape
        /// </summary>
        public void AddPoint(Pt point)
        {
            if (this.InternalRoof.SlabShapeEditor == null)
            {
                throw new Exception(Properties.Resources.InvalidShapeEditor);
            }

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRoof.SlabShapeEditor.Enable();
            this.InternalRoof.SlabShapeEditor.DrawPoint(point.ToXyz());
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Move existing point by offset
        /// </summary>
        public void MovePoint(Pt point, double offset)
        {
            if (this.InternalRoof.SlabShapeEditor == null)
            {
                throw new Exception(Properties.Resources.InvalidShapeEditor);
            }

            SlabShapeVertex vertex = null;

            foreach (SlabShapeVertex v in this.InternalRoof.SlabShapeEditor.SlabShapeVertices)
            {
                if (point.IsAlmostEqualTo(v.Position.ToPoint()))
                {
                    vertex = v;
                }
            }

            if (vertex != null && offset != 0)
            {
                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                this.InternalRoof.SlabShapeEditor.Enable();
                this.InternalRoof.SlabShapeEditor.ModifySubElement(vertex, offset);
                TransactionManager.Instance.TransactionTaskDone();
            }
        }


        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Roof from a user selected Element.
        /// </summary>
        /// <param name="roof"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Roof FromExisting(Autodesk.Revit.DB.RoofBase roof, bool isRevitOwned)
        {
            return new Roof(roof)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
