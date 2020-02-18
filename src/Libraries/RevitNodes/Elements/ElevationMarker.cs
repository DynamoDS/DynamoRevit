using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.Elements.Views;
using Revit.GeometryConversion;
using RevitServices.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit.Elements
{
    public class ElevationMarker : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the ElevationMarker
        /// </summary>
        internal Autodesk.Revit.DB.ElevationMarker InternalMarker
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalMarker; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private ElevationMarker(Autodesk.Revit.DB.ElevationMarker marker)
        {
            SafeInit(() => InitElevationMarker(marker));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a ElevationMarker element
        /// </summary>
        private void InitElevationMarker(Autodesk.Revit.DB.ElevationMarker marker)
        {
            InternalSetElevationMarker(marker);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the InternalMarker property and the associated element id and unique id
        /// </summary>
        /// <param name="marker"></param>
        private void InternalSetElevationMarker(Autodesk.Revit.DB.ElevationMarker marker)
        {
            InternalMarker = marker;
            InternalElementId = marker.Id;
            InternalUniqueId = marker.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Creates a new ElevationMarker.
        /// </summary>
        /// <param name="viewFamilyType">The ViewFamilyType that will be used by all elevations hosted on the new ElevationMarker.</param>
        /// <param name="location">The desired origin for the ElevationMarker.</param>
        /// <param name="initialViewScale">The view scale will be automatically applied to new elevations created on the ElevationMarker. The scale is the ratio of true model size to paper size (e.g. input 100 for 1:100 scale).</param>
        /// <returns>The new ElevationMarker element.</returns>
        public static ElevationMarker ByViewTypeLocation(Element viewFamilyType, Autodesk.DesignScript.Geometry.Point location, int initialViewScale)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            var elevationMarker = Autodesk.Revit.DB.ElevationMarker.CreateElevationMarker(Document, viewFamilyType.InternalElement.Id, location.ToXyz(), initialViewScale);
            TransactionManager.Instance.TransactionTaskDone();
            return new ElevationMarker(elevationMarker);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new elevation ViewSection on the ElevationMarker at the desired index.
        /// </summary>
        /// <param name="planView">The PlanView in which the ElevationMarker is visible. The new elevation ViewSection will derive its extents and inherit settings from the ViewPlan.</param>
        /// <param name="index">The index on the ElevationMarker where the new elevation ViewSection will be placed. The elevation marker can have up to four views, indexed from 0 to 3.</param>
        /// <returns>The new elevation ViewSection.</returns>
        public SectionView CreateElevationByMarkerIndex(Revit.Elements.Views.View planView, int index)
        {
            var view = planView as PlanView;
            if (view == null)
                throw new InvalidOperationException(Properties.Resources.NotPlanView);

            TransactionManager.Instance.EnsureInTransaction(Document);
            Autodesk.Revit.DB.ViewSection sectionView = this.InternalMarker.CreateElevation(Document, view.InternalViewPlan.Id, index);
            TransactionManager.Instance.TransactionTaskDone();
            return sectionView.ToDSType(true) as SectionView;
        }

        /// <summary>
        /// Gets the ViewSection for the index of the ElevationMarker.
        /// </summary>
        /// <param name="index">The index of the ElevationMarker for which a ViewSection will be returned.</param>
        /// <returns>ViewSection at the ElevationMarker index</returns>
        public SectionView GetView(int index)
        {
            ElementId viewId = this.InternalMarker.GetViewId(index);
            if (viewId == null)
                throw new InvalidOperationException("ElevationMarker has no views");

            return Document.GetElement(viewId).ToDSType(true) as SectionView;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current number of views hosted by this ElevationMarker.
        /// </summary>
        public int CurrentViewCount
        {
            get { return this.InternalMarker.CurrentViewCount; }
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a ElevationMarker from a user selected Element.
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static ElevationMarker FromExisting(Autodesk.Revit.DB.ElevationMarker marker, bool isRevitOwned)
        {
            return new ElevationMarker(marker)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
