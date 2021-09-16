using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Curve = Autodesk.DesignScript.Geometry.Curve;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit Ceiling
    /// </summary>
    public class Ceiling : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit ceiling
        /// </summary>
        internal Autodesk.Revit.DB.Ceiling InternalCeiling
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalCeiling; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Ceiling(Autodesk.Revit.DB.Ceiling ceiling)
        {
            SafeInit(() => InitCeiling(ceiling), true);
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private Ceiling(List<CurveLoop> profiles, Autodesk.Revit.DB.CeilingType ceilingType, Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitCeiling(profiles, ceilingType, level));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a ceiling element
        /// </summary>
        private void InitCeiling(Autodesk.Revit.DB.Ceiling ceiling)
        {
            InternalSetCeiling(ceiling);
        }

        /// <summary>
        /// Initialize a ceiling element
        /// </summary>
        private void InitCeiling(List<CurveLoop> profiles, Autodesk.Revit.DB.CeilingType ceilingType, Autodesk.Revit.DB.Level level)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            // we assume the ceiling is not structural here, this may be a bad assumption
            Autodesk.Revit.DB.Ceiling ceiling = Autodesk.Revit.DB.Ceiling.Create(Document, profiles, ceilingType.Id, level.Id);

            InternalSetCeiling(ceiling);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalCeiling);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the InternalCeiling property and the associated element id and unique id
        /// </summary>
        /// <param name="ceiling"></param>
        private void InternalSetCeiling(Autodesk.Revit.DB.Ceiling ceiling)
        {
            InternalCeiling = ceiling;
            InternalElementId = ceiling.Id;
            InternalUniqueId = ceiling.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Ceiling given its curve outline and Level
        /// </summary>
        /// <param name="outlineCurves"></param>
        /// <param name="ceilingType"></param>
        /// <param name="level"></param>
        /// <returns>The ceiling</returns>
        public static Ceiling ByOutlineTypeAndLevel(Curve[] outlineCurves, CeilingType ceilingType, Level level)
        {
            if (outlineCurves == null)
            {
                throw new ArgumentNullException("outlineCurves");
            }

            var ceiling = ByOutlineTypeAndLevel(PolyCurve.ByJoinedCurves(outlineCurves), ceilingType, level);
            DocumentManager.Regenerate();
            return ceiling;
        }

        /// <summary>
        /// Create a Revit Ceiling given its curve outline and Level
        /// </summary>
        /// <param name="outline"></param>
        /// <param name="ceilingType"></param>
        /// <param name="level"></param>
        /// <returns>The ceiling</returns>
        public static Ceiling ByOutlineTypeAndLevel(PolyCurve outline, CeilingType ceilingType, Level level)
        {
            if (outline == null)
            {
                throw new ArgumentNullException("outline");
            }

            if (ceilingType == null)
            {
                throw new ArgumentNullException("ceilingType");
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

            var ceiling = new Ceiling(loops, ceilingType.InternalCeilingType, level.InternalLevel);
            DocumentManager.Regenerate();
            return ceiling;
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Ceiling from a user selected Element.
        /// </summary>
        /// <param name="ceiling"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Ceiling FromExisting(Autodesk.Revit.DB.Ceiling ceiling, bool isRevitOwned)
        {
            return new Ceiling(ceiling)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
