using System;
using System.Linq;

using Autodesk.Revit.DB;

using DynamoServices;

using Revit.GeometryConversion;

using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit ViewSection
    /// </summary>
    [RegisterForTrace]
    public class SectionView : View
    {

        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit element
        /// </summary>
        internal Autodesk.Revit.DB.ViewSection InternalViewSection
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalViewSection; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private SectionView(Autodesk.Revit.DB.ViewSection view)
        {
            SafeInit(() => InitSectionView(view));
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private SectionView( BoundingBoxXYZ bbox )
        {
            SafeInit(() => InitSectionView(bbox));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a SectionView element
        /// </summary>
        private void InitSectionView(Autodesk.Revit.DB.ViewSection view)
        {
            InternalSetSectionView(view);
        }

        /// <summary>
        /// Initialize a SectionView element
        /// </summary>
        private void InitSectionView(BoundingBoxXYZ bbox)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            ViewSection vd = CreateSectionView(bbox);

            InternalSetSectionView(vd);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, this.InternalElement);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the InternalViewSection property and the associated element id and unique id
        /// </summary>
        /// <param name="floor"></param>
        private void InternalSetSectionView(Autodesk.Revit.DB.ViewSection floor)
        {
            this.InternalViewSection = floor;
            this.InternalElementId = floor.Id;
            this.InternalUniqueId = floor.UniqueId;
        }

        #endregion

        #region Private helper methods

        private static ViewSection CreateSectionView(BoundingBoxXYZ bbox)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            // (sic) From the Dynamo legacy implementation
            var viewFam = DocumentManager.Instance.ElementsOfType<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == ViewFamily.Section);

            if (viewFam == null)
            {
                throw new Exception("There is no three dimensional view family in the document");
            }

            var viewSection = ViewSection.CreateSection( Document, viewFam.Id, bbox);

            TransactionManager.Instance.TransactionTaskDone();

            return viewSection;

        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit ViewSection by a bounding box
        /// </summary>
        /// <param name="box">The bounding box of the view in meters</param>
        /// <returns></returns>
        public static SectionView ByBoundingBox(Autodesk.DesignScript.Geometry.BoundingBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }

            return new SectionView(box.ToRevitType());
        }
        /// <summary>
        /// Create a Revit ViewSection by a specified corrdinate system, minPoint and maxPoint
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="minPoint"></param>
        /// <param name="maxPoint"></param>
        /// <returns></returns>
        public static SectionView ByCoordinateSystemMinPointMaxPoint(
            Autodesk.DesignScript.Geometry.CoordinateSystem cs,
            Autodesk.DesignScript.Geometry.Point minPoint,
            Autodesk.DesignScript.Geometry.Point maxPoint)
        {
            return new SectionView(GeometryPrimitiveConverter.ToRevitBoundingBox(cs, minPoint, maxPoint));
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a View from a user selected Element.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static SectionView FromExisting(Autodesk.Revit.DB.ViewSection view, bool isRevitOwned)
        {
            return new SectionView(view)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}

