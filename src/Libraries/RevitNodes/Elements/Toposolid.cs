using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using Pt = Autodesk.DesignScript.Geometry.Point;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit Toposolid
    /// </summary>
    public class Toposolid : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit Toposolid
        /// </summary>
        internal Autodesk.Revit.DB.Toposolid InternalToposolid
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalToposolid; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Toposolid(Autodesk.Revit.DB.Toposolid toposolid)
        {
            SafeInit(() => InitToposolid(toposolid), true);
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private Toposolid(List<CurveLoop> profiles, List<XYZ> points, Autodesk.Revit.DB.ToposolidType toposolidType, Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitToposolid(profiles, points, toposolidType, level));
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private Toposolid(List<CurveLoop> profiles, Autodesk.Revit.DB.ToposolidType toposolidType, Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitToposolid(profiles, toposolidType, level));
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private Toposolid(List<XYZ> points, Autodesk.Revit.DB.ToposolidType toposolidType, Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitToposolid(points, toposolidType, level));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a toposolid element
        /// </summary>
        private void InitToposolid(Autodesk.Revit.DB.Toposolid toposolid)
        {
            InternalSetToposolid(toposolid);
        }

        /// <summary>
        /// Initialize a toposolid element
        /// </summary>
        private void InitToposolid(List<CurveLoop> profiles, List<XYZ> points, Autodesk.Revit.DB.ToposolidType toposolidType, Autodesk.Revit.DB.Level level)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.Toposolid toposolid = Autodesk.Revit.DB.Toposolid.Create(Document, profiles, points, toposolidType.Id, level.Id);

            InternalSetToposolid(toposolid);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalToposolid);
        }

        /// <summary>
        /// Initialize a toposolid element
        /// </summary>
        private void InitToposolid(List<CurveLoop> profiles, Autodesk.Revit.DB.ToposolidType toposolidType, Autodesk.Revit.DB.Level level)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.Toposolid toposolid = Autodesk.Revit.DB.Toposolid.Create(Document, profiles, toposolidType.Id, level.Id);

            InternalSetToposolid(toposolid);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalToposolid);
        }

        /// <summary>
        /// Initialize a toposolid element
        /// </summary>
        private void InitToposolid(List<XYZ> points, Autodesk.Revit.DB.ToposolidType toposolidType, Autodesk.Revit.DB.Level level)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.Toposolid toposolid = Autodesk.Revit.DB.Toposolid.Create(Document, points, toposolidType.Id, level.Id);

            InternalSetToposolid(toposolid);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalToposolid);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the InternalToposolid property and the associated element id and unique id
        /// </summary>
        /// <param name="toposolid"></param>
        private void InternalSetToposolid(Autodesk.Revit.DB.Toposolid toposolid)
        {
            InternalToposolid = toposolid;
            InternalElementId = toposolid.Id;
            InternalUniqueId = toposolid.UniqueId;
        }

        #endregion

        #region public properties
        /// <summary>
        /// Get Slab Shape Points
        /// </summary>
        public IEnumerable<Pt> Points
        {
            get
            {
                if (this.InternalToposolid.GetSlabShapeEditor() == null)
                {
                    throw new Exception(Properties.Resources.InvalidShapeEditor);
                }

                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                this.InternalToposolid.GetSlabShapeEditor().Enable();
                TransactionManager.Instance.TransactionTaskDone();

                List<Pt> points = new List<Pt>();
                foreach (SlabShapeVertex v in this.InternalToposolid.GetSlabShapeEditor().SlabShapeVertices)
                {
                    points.Add(v.Position.ToPoint());
                }
                return points;
            }
        }
        #endregion

        #region Public static constructors

        public static Toposolid ByOutlinePointsTypeAndLevel(Curve[] outlineCurves, IEnumerable<Pt> points, ToposolidType toposolidType, Level level)
        {
            if (outlineCurves == null)
            {
                throw new ArgumentNullException("outlineCurves");
            }

            var toposolid = ByOutlinePointsTypeAndLevel(PolyCurve.ByJoinedCurves(outlineCurves, 0.001, false), points, toposolidType, level);

            return toposolid;
        }

        public static Toposolid ByOutlinePointsTypeAndLevel(PolyCurve outline, IEnumerable<Pt> points, ToposolidType toposolidType, Level level)
        {
            if (outline == null)
            {
                throw new ArgumentNullException("outline");
            }

            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            if (toposolidType == null)
            {
                throw new ArgumentNullException("toposolidType");
            }

            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            if (!outline.IsClosed)
            {
                throw new ArgumentException(Properties.Resources.OpenInputPolyCurveError);
            }

            CurveLoop loop = new CurveLoop();
            outline.Curves().ForEach(x => loop.Append(x.ToRevitType()));

            List<CurveLoop> loops = new List<CurveLoop> { loop };

            if (!BoundaryValidation.IsValidHorizontalBoundary(loops))
            {
                throw new ArgumentException(Properties.Resources.NotHorizontalInputPolyCurveError);
            }

            List<XYZ> xyzList = points.Select(x => x.ToRevitType()).ToList();

            var toposolid = new Toposolid(loops, xyzList, toposolidType.InternalToposolidType, level.InternalLevel);
            DocumentManager.Regenerate();
            return toposolid;
        }

        public static Toposolid ByOutlineTypeAndLevel(Curve[] outlineCurves, ToposolidType toposolidType, Level level)
        {
            if (outlineCurves == null)
            {
                throw new ArgumentNullException("outlineCurves");
            }

            var toposolid = ByOutlineTypeAndLevel(PolyCurve.ByJoinedCurves(outlineCurves, 0.001, false), toposolidType, level);

            return toposolid;
        }

        public static Toposolid ByOutlineTypeAndLevel(PolyCurve outline,  ToposolidType toposolidType, Level level)
        {
            if (outline == null)
            {
                throw new ArgumentNullException("outline");
            }

            if (toposolidType == null)
            {
                throw new ArgumentNullException("toposolidType");
            }

            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            if (!outline.IsClosed)
            {
                throw new ArgumentException(Properties.Resources.OpenInputPolyCurveError);
            }

            CurveLoop loop = new CurveLoop();
            outline.Curves().ForEach(x => loop.Append(x.ToRevitType()));

            List<CurveLoop> loops = new List<CurveLoop> { loop };

            if (!BoundaryValidation.IsValidHorizontalBoundary(loops))
            {
                throw new ArgumentException(Properties.Resources.NotHorizontalInputPolyCurveError);
            }

            var toposolid = new Toposolid(loops, toposolidType.InternalToposolidType, level.InternalLevel);
            DocumentManager.Regenerate();
            return toposolid;
        }

        public static Toposolid ByPointsTypeAndLevel(IEnumerable<Pt> points, ToposolidType toposolidType, Level level)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            if (toposolidType == null)
            {
                throw new ArgumentNullException("toposolidType");
            }

            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            List<XYZ> xyzList = points.Select(x => x.ToRevitType()).ToList();

            var toposolid = new Toposolid(xyzList, toposolidType.InternalToposolidType, level.InternalLevel);
            DocumentManager.Regenerate();
            return toposolid;
        }
        
        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Toposolid from a user selected Element.
        /// </summary>
        /// <param name="toposolid"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Toposolid FromExisting(Autodesk.Revit.DB.Toposolid toposolid, bool isRevitOwned)
        {
            return new Toposolid(toposolid)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
