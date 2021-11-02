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

        /// <summary>
        /// Set Internal Element from a exsiting element.
        /// </summary>
        /// <param name="element"></param>
        internal override void SetInternalElement(Autodesk.Revit.DB.Element element)
        {
            InternalSetViewport(element as Autodesk.Revit.DB.Viewport);
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for the Element
        /// </summary>
        /// <param name="viewport"></param>
        private Viewport(Autodesk.Revit.DB.Viewport viewport)
        {
            SafeInit(() => InitViewport(viewport), true);
        }

        /// <summary>
        /// Create a new Viewport by Sheet, View and Location
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="view"></param>
        /// <param name="location"></param>
        private Viewport(Sheet sheet, Revit.Elements.Views.View view, Autodesk.DesignScript.Geometry.Point location)
        {
            SafeInit(() => InitViewport(sheet, view, location));
        }

        #endregion

        #region Helpers for Private constructors

        /// <summary>
        /// Initialize a Viewport element
        /// </summary>
        /// <param name="viewport"></param>
        private void InitViewport(Autodesk.Revit.DB.Viewport viewport)
        {
            InternalSetViewport(viewport);
        }

        /// <summary>
        /// Initialize a Viewport element
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="view"></param>
        /// <param name="location"></param>
        private void InitViewport(Sheet sheet, Revit.Elements.Views.View view, Autodesk.DesignScript.Geometry.Point location)
        {
            ElementId sheetId = sheet.InternalView.Id;
            ElementId viewId = view.InternalView.Id;
            XYZ viewLocation = GeometryPrimitiveConverter.ToRevitType(location);

            TransactionManager.Instance.EnsureInTransaction(Document);

            var viewportElement = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Viewport>(Document);

            if (viewportElement == null)
            {
                if (!Autodesk.Revit.DB.Viewport.CanAddViewToSheet(Document, sheetId, viewId))
                    throw new InvalidOperationException(Properties.Resources.ViewAlreadyPlacedOnSheet);
                viewportElement = Autodesk.Revit.DB.Viewport.Create(Document, sheetId, viewId, viewLocation);
            }
            else
            {
                if (!Autodesk.Revit.DB.Viewport.CanAddViewToSheet(Document, sheetId, viewId))
                {
                    viewportElement.SetBoxCenter(viewLocation);
                }                    
                else
                {
                    viewportElement = Autodesk.Revit.DB.Viewport.Create(Document, sheetId, viewId, viewLocation);
                }
            }

            InternalSetViewport(viewportElement);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, this.InternalElement);
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
        /// <summary>
        /// Get ViewSheet on which the viewport appears
        /// </summary>
        public Revit.Elements.Views.Sheet Sheet
        {
            get
            {
                var sheet = (Autodesk.Revit.DB.ViewSheet)Document.GetElement(InternalViewport.SheetId);
                return (Revit.Elements.Views.Sheet)ElementWrapper.ToDSType(sheet, true);
            }
        }

        /// <summary>
        /// Get the associated View 
        /// </summary>
        public Revit.Elements.Views.View View
        {
            get
            {
                var view = (Autodesk.Revit.DB.View)Document.GetElement(InternalViewport.ViewId);
                return (Revit.Elements.Views.View) ElementWrapper.ToDSType(view, true);
            }
        }

        /// <summary>
        /// Returns the center of the outline of the viewport on the sheet, excluding the viewport label. 
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point BoxCenter
        {
            get
            {
                return InternalViewport.GetBoxCenter().ToPoint();
            }
        }

        /// <summary>
        /// Moves this viewport so that the center of the box outline (excluding the viewport label) is at a given point.
        /// </summary>
        /// <param name="point"></param>
        public Viewport SetBoxCenter(Autodesk.DesignScript.Geometry.Point point)
        {
            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            this.InternalViewport.SetBoxCenter(point.ToXyz());
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Returns the outline of the viewport on the sheet, excluding the viewport label.
        /// </summary>
        public BoundingBox BoxOutline
        {
            get
            {
                var outline = InternalViewport.GetBoxOutline();
                return BoundingBox.ByCorners(outline.MinimumPoint.ToPoint(), outline.MaximumPoint.ToPoint());
            }
        }

        /// <summary>
        /// Gets the outline viewport's label on the sheet.
        /// </summary>
        public BoundingBox LabelOutline
        {
            get
            {
                var outline = InternalViewport.GetLabelOutline();
                return BoundingBox.ByCorners(outline.MinimumPoint.ToPoint(), outline.MaximumPoint.ToPoint());
            }
        }

        /// <summary>
        ///     The offset is a two-dimensional vector from left bottom corner of the viewport
        ///     with Rotation set to None to the left end of the viewport label line. The Z coordinate
        ///     is ignored.
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point LabelOffset
        {
            get
            {
                return this.InternalViewport.LabelOffset.ToPoint();
            }
        }

        /// <summary>
        /// Set LabelOffset of Viewport
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Viewport SetLabelOffset(Autodesk.DesignScript.Geometry.Point point)
        {
            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            this.InternalViewport.LabelOffset = point.ToXyz();
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// The length of the viewport label line in sheet space, measured in feet.
        /// </summary>
        public double LabelLineLength
        {
            get
            {
                return InternalViewport.LabelLineLength;
            }
        }

        /// <summary>
        /// Set LabelLineLength of Viewport
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public Viewport SetLabelLineLength(double length)
        {
            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            this.InternalViewport.LabelLineLength = length;
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }
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
            if (sheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            if (view == null) 
            {
                throw new ArgumentNullException("view");
            }
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            if (IsViewEmpty(view.InternalView))
                throw new InvalidOperationException(Properties.Resources.EmptyView);

            return new Viewport(sheet, view, location);
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
