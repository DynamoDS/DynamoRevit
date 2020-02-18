using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Revit.Elements.Views;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    public class Viewport : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal reference to the Viewport
        /// </summary>
        internal Autodesk.Revit.DB.Viewport InternalViewport
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalViewport; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for the Element
        /// </summary>
        /// <param name="viewport"></param>
        private Viewport(Autodesk.Revit.DB.Viewport viewport)
        {
            SafeInit(() => InitViewport(viewport));
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Initialize a Viewport element
        /// </summary>
        /// <param name="viewport"></param>
        private void InitViewport(Autodesk.Revit.DB.Viewport viewport)
        {
            InternalSetViewport(viewport);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the Viewport property, element id, and unique id
        /// </summary>
        /// <param name="viewport"></param>
        private void InternalSetViewport(Autodesk.Revit.DB.Viewport viewport)
        {
            this.InternalViewport = viewport;
            this.InternalElementId = viewport.Id;
            this.InternalUniqueId = viewport.UniqueId;
        }

        #endregion

        #region Public properties

        #endregion

        #region Public static constructors

        /// <summary>
        /// Creates a new Viewport at a given location on a sheet.
        /// </summary>
        /// <param name="sheet">The Sheet on which the new Viewport will be placed.</param>
        /// <param name="view">The view shown in the Viewport.</param>
        /// <param name="location">The new Viewport will be centered on this point.</param>
        /// <returns>The new Viewport.</returns>
        public static Viewport BySheetViewLocation(Sheet sheet, Revit.Elements.Views.View view, Autodesk.DesignScript.Geometry.Point location)
        {
            ElementId sheetId = sheet.InternalView.Id;
            ElementId viewId = view.InternalView.Id;
            XYZ viewLocation = GeometryPrimitiveConverter.ToRevitType(location);

            if (!Autodesk.Revit.DB.Viewport.CanAddViewToSheet(Document, sheetId, viewId))
                throw new InvalidOperationException(Properties.Resources.ViewAlreadyPlacedOnSheet);
            if (IsViewEmpty(view.InternalView))
                throw new InvalidOperationException(Properties.Resources.EmptyView);

            TransactionManager.Instance.EnsureInTransaction(Document);
            var viewport = Autodesk.Revit.DB.Viewport.Create(Document, sheetId, viewId, viewLocation);
            TransactionManager.Instance.TransactionTaskDone();
            return viewport.ToDSType(true) as Viewport;
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Viewport from a user selected Element.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Viewport FromExisting(Autodesk.Revit.DB.Viewport viewport, bool isRevitOwned)
        {
            return new Viewport(viewport)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        private static bool IsViewEmpty(Autodesk.Revit.DB.View view)
        {
            using (FilteredElementCollector collector = new FilteredElementCollector(Document, view.Id))
            {
                return !collector.Any();
            }
        }
    }
}
