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
using DBPropertyLine = Autodesk.Revit.DB.PropertyLine;
using Pt = Autodesk.DesignScript.Geometry.Point;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit PropertyLine element. PropertyLine elements describe site boundaries
    /// either by an arbitrary set of horizontal sketch curves, or by a table of
    /// distance/bearing entries.
    /// </summary>
    public class PropertyLine : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal handle on the Revit PropertyLine.
        /// </summary>
        internal DBPropertyLine InternalPropertyLine
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the wrapped element.
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalPropertyLine; }
        }

        #endregion

        #region Private constructors

        private PropertyLine(DBPropertyLine propertyLine)
        {
            SafeInit(() => InitPropertyLine(propertyLine), true);
        }

        private PropertyLine(IList<CurveLoop> curveLoops)
        {
            SafeInit(() => InitPropertyLine(curveLoops));
        }

        private PropertyLine(IList<Autodesk.Revit.DB.PropertyTableEntry> entries)
        {
            SafeInit(() => InitPropertyLine(entries));
        }

        #endregion

        #region Helpers for private constructors

        private void InitPropertyLine(DBPropertyLine propertyLine)
        {
            InternalSetPropertyLine(propertyLine);
        }

        private void InitPropertyLine(IList<CurveLoop> curveLoops)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            DBPropertyLine propertyLine = DBPropertyLine.Create(Document, curveLoops);
            InternalSetPropertyLine(propertyLine);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalPropertyLine);
        }

        private void InitPropertyLine(IList<Autodesk.Revit.DB.PropertyTableEntry> entries)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            DBPropertyLine propertyLine = DBPropertyLine.Create(Document, entries);
            InternalSetPropertyLine(propertyLine);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalPropertyLine);
        }

        #endregion

        #region Private mutators

        private void InternalSetPropertyLine(DBPropertyLine propertyLine)
        {
            InternalPropertyLine = propertyLine;
            InternalElementId = propertyLine.Id;
            InternalUniqueId = propertyLine.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the boundary curve loops of a sketch-based PropertyLine as PolyCurves.
        /// Throws when invoked on a table-based PropertyLine.
        /// </summary>
        public PolyCurve[] Boundary
        {
            get
            {
                IList<CurveLoop> loops = InternalPropertyLine.GetBoundary();
                return loops.Select(ConvertCurveLoopToPolyCurve).ToArray();
            }
        }

        /// <summary>
        /// The start point of the PropertyLine, in Dynamo units.
        /// </summary>
        public Pt Start
        {
            get { return InternalPropertyLine.GetStart().ToPoint(); }
        }

        /// <summary>
        /// The PropertyTableEntry rows of a table-based PropertyLine.
        /// Throws when invoked on a sketch-based PropertyLine.
        /// </summary>
        public PropertyTableEntry[] PropertyTable
        {
            get
            {
                IList<Autodesk.Revit.DB.PropertyTableEntry> entries = InternalPropertyLine.GetPropertyTable();
                return entries.Select(PropertyTableEntry.FromExisting).ToArray();
            }
        }

        /// <summary>
        /// The area of the PropertyLine. Returns -1 when the PropertyLine is not a closed loop.
        /// </summary>
        public double Area => InternalPropertyLine.Area;

        /// <summary>
        /// True when the PropertyLine forms a closed loop.
        /// </summary>
        public bool IsClosedLoop => InternalPropertyLine.IsClosedLoop();

        /// <summary>
        /// True when the PropertyLine is sketch-based; false when it is table-based.
        /// </summary>
        public bool IsSketchBased => InternalPropertyLine.IsSketchBased();

        /// <summary>
        /// True when a sketch-based PropertyLine can be converted to a table-based one
        /// (i.e. its sketch only contains lines and arcs).
        /// </summary>
        public bool IsValidToConvert => InternalPropertyLine.IsValidToConvert();

        /// <summary>
        /// The Sketch element backing a sketch-based PropertyLine, or null for a table-based one.
        /// </summary>
        public Sketch Sketch
        {
            get
            {
                ElementId sketchId = InternalPropertyLine.SketchId;
                if (sketchId == null || sketchId == ElementId.InvalidElementId)
                {
                    return null;
                }

                var revitSketch = Document.GetElement(sketchId) as Autodesk.Revit.DB.Sketch;
                return revitSketch == null ? null : Sketch.FromExisting(revitSketch, true);
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Creates a sketch-based PropertyLine from a single PolyCurve outline.
        /// The outline does not need to be closed, but it must lie on a plane parallel to the XY plane.
        /// </summary>
        public static PropertyLine ByCurveLoops(PolyCurve outline)
        {
            if (outline == null)
            {
                throw new ArgumentNullException(nameof(outline));
            }

            return ByPolyCurves(new[] { outline });
        }

        /// <summary>
        /// Creates a sketch-based PropertyLine from a list of curves that will be joined into a single
        /// PolyCurve outline.
        /// </summary>
        public static PropertyLine ByCurveLoops(Curve[] outlineCurves)
        {
            if (outlineCurves == null)
            {
                throw new ArgumentNullException(nameof(outlineCurves));
            }

            PolyCurve outline = PolyCurve.ByJoinedCurves(outlineCurves, 0.001, false);
            return ByCurveLoops(outline);
        }

        /// <summary>
        /// Creates a sketch-based PropertyLine from one or more PolyCurve outlines.
        /// Each outline must be planar and lie on a plane parallel to the XY plane; the outlines
        /// must not intersect each other.
        /// </summary>
        public static PropertyLine ByPolyCurves(PolyCurve[] outlines)
        {
            if (outlines == null)
            {
                throw new ArgumentNullException(nameof(outlines));
            }

            if (outlines.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.PropertyLineInvalidBoundary, nameof(outlines));
            }

            List<CurveLoop> loops = ConvertOutlinesToCurveLoops(outlines);

            if (!DBPropertyLine.IsValidBoundary(loops))
            {
                throw new ArgumentException(Properties.Resources.PropertyLineInvalidBoundary, nameof(outlines));
            }

            var propertyLine = new PropertyLine(loops);
            DocumentManager.Regenerate();
            return propertyLine;
        }

        /// <summary>
        /// Creates a table-based PropertyLine from a sequence of PropertyTableEntry rows.
        /// </summary>
        public static PropertyLine ByPropertyTableEntries(PropertyTableEntry[] entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            if (entries.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.PropertyLineInvalidPropertyTable, nameof(entries));
            }

            List<Autodesk.Revit.DB.PropertyTableEntry> revitEntries =
                entries.Select(e => e?.InternalPropertyTableEntry
                                    ?? throw new ArgumentException(Properties.Resources.PropertyLineInvalidPropertyTable, nameof(entries)))
                       .ToList();

            if (!DBPropertyLine.IsValidPropertyTable(Document, revitEntries))
            {
                throw new ArgumentException(Properties.Resources.PropertyLineInvalidPropertyTable, nameof(entries));
            }

            var propertyLine = new PropertyLine(revitEntries);
            DocumentManager.Regenerate();
            return propertyLine;
        }

        /// <summary>
        /// Returns true when the supplied PolyCurves form a valid PropertyLine boundary
        /// (planar, horizontal, non-self-intersecting, no unbounded circles or ellipses).
        /// </summary>
        public static bool IsValidBoundary(PolyCurve[] outlines)
        {
            if (outlines == null || outlines.Length == 0)
            {
                return false;
            }

            try
            {
                List<CurveLoop> loops = ConvertOutlinesToCurveLoops(outlines);
                return DBPropertyLine.IsValidBoundary(loops);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true when the supplied PropertyTableEntry rows form a valid table-based PropertyLine.
        /// </summary>
        public static bool IsValidPropertyTable(PropertyTableEntry[] entries)
        {
            if (entries == null || entries.Length == 0 || entries.Any(e => e == null))
            {
                return false;
            }

            List<Autodesk.Revit.DB.PropertyTableEntry> revitEntries =
                entries.Select(e => e.InternalPropertyTableEntry).ToList();
            return DBPropertyLine.IsValidPropertyTable(Document, revitEntries);
        }

        #endregion

        #region Public mutators

        /// <summary>
        /// Replaces the boundary of a sketch-based PropertyLine with the supplied outlines.
        /// </summary>
        public PropertyLine SetBoundary(PolyCurve[] outlines)
        {
            if (outlines == null)
            {
                throw new ArgumentNullException(nameof(outlines));
            }

            List<CurveLoop> loops = ConvertOutlinesToCurveLoops(outlines);

            if (!DBPropertyLine.IsValidBoundary(loops))
            {
                throw new ArgumentException(Properties.Resources.PropertyLineInvalidBoundary, nameof(outlines));
            }

            TransactionManager.Instance.EnsureInTransaction(Document);
            InternalPropertyLine.SetBoundary(loops);
            TransactionManager.Instance.TransactionTaskDone();

            DocumentManager.Regenerate();
            return this;
        }

        /// <summary>
        /// Sets the start point of a table-based PropertyLine.
        /// Throws when invoked on a sketch-based PropertyLine.
        /// </summary>
        public PropertyLine SetStart(Pt start)
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (InternalPropertyLine.IsSketchBased())
            {
                throw new InvalidOperationException(Properties.Resources.PropertyLineNotTableBased);
            }

            TransactionManager.Instance.EnsureInTransaction(Document);
            InternalPropertyLine.SetStart(start.ToXyz());
            TransactionManager.Instance.TransactionTaskDone();

            DocumentManager.Regenerate();
            return this;
        }

        /// <summary>
        /// Replaces the table entries of a table-based PropertyLine.
        /// Throws when invoked on a sketch-based PropertyLine.
        /// </summary>
        public PropertyLine SetPropertyTable(PropertyTableEntry[] entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            if (InternalPropertyLine.IsSketchBased())
            {
                throw new InvalidOperationException(Properties.Resources.PropertyLineNotTableBased);
            }

            List<Autodesk.Revit.DB.PropertyTableEntry> revitEntries =
                entries.Select(e => e?.InternalPropertyTableEntry
                                    ?? throw new ArgumentException(Properties.Resources.PropertyLineInvalidPropertyTable, nameof(entries)))
                       .ToList();

            TransactionManager.Instance.EnsureInTransaction(Document);
            InternalPropertyLine.SetPropertyTable(revitEntries);
            TransactionManager.Instance.TransactionTaskDone();

            DocumentManager.Regenerate();
            return this;
        }

        /// <summary>
        /// Converts a sketch-based PropertyLine to a table-based PropertyLine. The underlying sketch
        /// must contain only lines and arcs (see <see cref="IsValidToConvert"/>).
        /// </summary>
        public PropertyLine ConvertToTable()
        {
            if (!InternalPropertyLine.IsSketchBased())
            {
                throw new InvalidOperationException(Properties.Resources.PropertyLineConvertFailed);
            }

            if (!InternalPropertyLine.IsValidToConvert())
            {
                throw new InvalidOperationException(Properties.Resources.PropertyLineConvertFailed);
            }

            TransactionManager.Instance.EnsureInTransaction(Document);
            InternalPropertyLine.ConvertToTable();
            TransactionManager.Instance.TransactionTaskDone();

            DocumentManager.Regenerate();
            return this;
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Wraps a Revit-owned PropertyLine in the Dynamo PropertyLine wrapper.
        /// </summary>
        internal static PropertyLine FromExisting(DBPropertyLine propertyLine, bool isRevitOwned)
        {
            return new PropertyLine(propertyLine)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Geometry helpers

        private static List<CurveLoop> ConvertOutlinesToCurveLoops(PolyCurve[] outlines)
        {
            var loops = new List<CurveLoop>(outlines.Length);
            foreach (PolyCurve outline in outlines)
            {
                if (outline == null)
                {
                    throw new ArgumentException(Properties.Resources.PropertyLineInvalidBoundary, nameof(outlines));
                }

                var loop = new CurveLoop();
                foreach (Curve curve in outline.Curves())
                {
                    loop.Append(curve.ToRevitType());
                }
                loops.Add(loop);
            }
            return loops;
        }

        private static PolyCurve ConvertCurveLoopToPolyCurve(CurveLoop loop)
        {
            var protoCurves = loop.Select(c => c.ToProtoType(true)).ToArray();
            try
            {
                return PolyCurve.ByJoinedCurves(protoCurves, 0.001, false);
            }
            finally
            {
                foreach (Curve c in protoCurves)
                {
                    c.Dispose();
                }
            }
        }

        #endregion
    }
}
