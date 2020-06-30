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
    public class ScheduleOnSheet : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal reference to the ScheduleSheetInstance
        /// </summary>
        internal Autodesk.Revit.DB.ScheduleSheetInstance InternalScheduleOnSheet
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalScheduleOnSheet; }
        }
        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for the Element
        /// </summary>
        /// <param name="ScheduleOnSheet"></param>
        private ScheduleOnSheet(Autodesk.Revit.DB.ScheduleSheetInstance ScheduleOnSheet)
        {
            SafeInit(() => InitScheduleOnSheet(ScheduleOnSheet));
        }

        /// <summary>
        /// Create a new ScheduleOnSheet by sheet, scheduleView and location
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="scheduleView"></param>
        /// <param name="location"></param>
        private ScheduleOnSheet(Sheet sheet, Revit.Elements.Views.ScheduleView scheduleView, Autodesk.DesignScript.Geometry.Point location)
        {
            SafeInit(() => InitScheduleOnSheet(sheet, scheduleView, location));
        }

        #endregion

        #region Helpers for Private constructors

        /// <summary>
        /// Initialize a ScheduleOnSheet element
        /// </summary>
        /// <param name="ScheduleOnSheet"></param>
        private void InitScheduleOnSheet(Autodesk.Revit.DB.ScheduleSheetInstance ScheduleOnSheet)
        {
            InternalSetScheduleOnSheet(ScheduleOnSheet);
        }

        /// <summary>
        /// Initialize a ScheduleOnSheet element
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="scheduleView"></param>
        /// <param name="location"></param>
        private void InitScheduleOnSheet(Sheet sheet, Revit.Elements.Views.ScheduleView scheduleView, Autodesk.DesignScript.Geometry.Point location)
        {
            ElementId sheetId = sheet.InternalView.Id;
            ElementId scheduleViewId = scheduleView.InternalView.Id;
            XYZ scheduleLocation = GeometryPrimitiveConverter.ToRevitType(location);

            TransactionManager.Instance.EnsureInTransaction(Document);

            var scheduleOnSheetElement = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ScheduleSheetInstance>(Document);

            scheduleOnSheetElement = Autodesk.Revit.DB.ScheduleSheetInstance.Create(Document, sheetId, scheduleViewId, scheduleLocation);            

            InternalSetScheduleOnSheet(scheduleOnSheetElement);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, this.InternalElement);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the ScheduleOnSheet property, element id, and unique id
        /// </summary>
        /// <param name="ScheduleOnSheet"></param>
        private void InternalSetScheduleOnSheet(Autodesk.Revit.DB.ScheduleSheetInstance ScheduleOnSheet)
        {
            this.InternalScheduleOnSheet = ScheduleOnSheet;
            this.InternalElementId = ScheduleOnSheet.Id;
            this.InternalUniqueId = ScheduleOnSheet.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Location on the sheet where the ScheduleInstance is placed (in sheet coordinates). 
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point Location
        {
            get
            {
                return InternalScheduleOnSheet.Point.ToPoint();
            }
        }

        /// <summary>
        /// Set the Location on the sheet where the ScheduleInstance is placed (in sheet coordinates).
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public ScheduleOnSheet SetLocation(Autodesk.DesignScript.Geometry.Point point)
        {
            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            this.InternalScheduleOnSheet.Point = point.ToRevitType();
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Get ViewSheet on which the ScheduleView appears
        /// </summary>
        public Revit.Elements.Views.Sheet Sheet
        {
            get
            {
                var sheet = Document.GetElement(InternalScheduleOnSheet.OwnerViewId) as Autodesk.Revit.DB.ViewSheet;
                return ElementWrapper.ToDSType(sheet, true) as Revit.Elements.Views.Sheet;
            }
        }

        /// <summary>
        /// The "master" schedule that generates this ScheduleInstance. 
        /// </summary>
        public Revit.Elements.Views.ScheduleView ScheduleView
        {
            get
            {
                var scheduleView = Document.GetElement(InternalScheduleOnSheet.ScheduleId);
                return ElementWrapper.ToDSType(scheduleView, true) as Revit.Elements.Views.ScheduleView;
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Creates a new ScheduleOnSheet at a given location on a sheet.
        /// </summary>
        /// <param name="sheet">The Sheet on which the new ScheduleOnSheet will be placed.</param>
        /// <param name="scheduleView">The scheduleView shown in the ScheduleOnSheet.</param>
        /// <param name="location">The new ScheduleOnSheet will be centered on this point.</param>
        /// <returns>The new ScheduleOnSheet</returns>
        public static ScheduleOnSheet BySheetScheduleLocation(Sheet sheet, Revit.Elements.Views.ScheduleView scheduleView, Autodesk.DesignScript.Geometry.Point location)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            if (scheduleView == null)
            {
                throw new ArgumentNullException("scheduleView");
            }
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            if(IsScheduleEmpty(scheduleView.InternalViewSchedule))
            {
                throw new InvalidOperationException(Properties.Resources.EmptySchedule);
            }

            return new ScheduleOnSheet(sheet, scheduleView, location);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a ScheduleOnSheet from a user selected Element.
        /// </summary>
        /// <param name="ScheduleOnSheet"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static ScheduleOnSheet FromExisting(Autodesk.Revit.DB.ScheduleSheetInstance ScheduleOnSheet, bool isRevitOwned)
        {
            return new ScheduleOnSheet(ScheduleOnSheet)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        private static bool IsScheduleEmpty(Autodesk.Revit.DB.ViewSchedule schedule)
        {
            using (FilteredElementCollector collector = new FilteredElementCollector(Document, schedule.Id))
            {
                return !collector.Any();
            }
        }
    }
}
