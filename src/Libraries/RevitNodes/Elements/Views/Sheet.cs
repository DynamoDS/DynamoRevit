using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using DynamoServices;
using Nuclex.Game.Packing;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Revit.GeometryConversion;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit ViewSheet
    /// </summary>
    [RegisterForTrace]
    public class Sheet : View
    {

        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit element
        /// </summary>
        internal Autodesk.Revit.DB.ViewSheet InternalViewSheet
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalViewSheet; }
        }

        public override void Dispose()
        {
            if (DuplicateViews != null) 
            {
                if (this.DuplicateViews.Any())
                {
                    foreach (var view in DuplicateViews)
                        view.Dispose();
                }
            }
            
            DuplicateViews = null;
            base.Dispose();
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Sheet(Autodesk.Revit.DB.ViewSheet view)
        {
            SafeInit(() => InitSheet(view), true);
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private Sheet(string nameOfSheet, string numberOfSheet, IEnumerable<Autodesk.Revit.DB.View> views)
        {
            SafeInit(() => InitSheet(nameOfSheet, numberOfSheet, views));
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="sheetNumber"></param>
        /// <param name="titleBlockFamilySymbol"></param>
        /// <param name="views"></param>
        private Sheet(string sheetName, string sheetNumber, Autodesk.Revit.DB.FamilySymbol titleBlockFamilySymbol, IEnumerable<Autodesk.Revit.DB.View> views)
        {
            SafeInit(() => InitSheet(sheetName, sheetNumber, titleBlockFamilySymbol, views));
        }

        private Sheet(string sheetName, string sheetNumber, Autodesk.Revit.DB.FamilySymbol titleBlockFamilySymbol)
        {
            SafeInit(() => InitSheet(sheetName, sheetNumber, titleBlockFamilySymbol));
        }

        private Sheet(string sheetName, string sheetNumber, Autodesk.Revit.DB.FamilySymbol titleBlockFamilySymbol, IEnumerable<Autodesk.Revit.DB.View> views, IEnumerable<XYZ> locations)
        {
            SafeInit(() => InitSheet(sheetName,sheetNumber,titleBlockFamilySymbol,views,locations));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a Sheet element
        /// </summary>
        private void InitSheet(Autodesk.Revit.DB.ViewSheet view)
        {
            InternalSetViewSheet(view);
        }

        /// <summary>
        /// Initialize a Sheet element
        /// </summary>
        private void InitSheet(string nameOfSheet, string numberOfSheet, IEnumerable<Autodesk.Revit.DB.View> views)
        {

            //Phase 1 - Check to see if the object exists and should be rebound
            var oldEle =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ViewSheet>(Document);

            // Rebind to Element
            if (oldEle != null)
            {
                InternalSetViewSheet(oldEle);
                InternalSetSheetName(nameOfSheet);
                InternalSetSheetNumber(numberOfSheet);
                InternalAddViewsToSheetView(views);
                return;
            }

            //Phase 2 - There was no existing Element, create new one
            TransactionManager.Instance.EnsureInTransaction(Document);

            // create sheet without title block
            var sheet = Autodesk.Revit.DB.ViewSheet.Create(Document, ElementId.InvalidElementId);

            InternalSetViewSheet(sheet);
            InternalSetSheetName(nameOfSheet);
            InternalSetSheetNumber(numberOfSheet);
            InternalAddViewsToSheetView(views);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        /// <summary>
        /// Initialize a Sheet element
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="sheetNumber"></param>
        /// <param name="titleBlockFamilySymbol"></param>
        /// <param name="views"></param>
        private void InitSheet(string sheetName, string sheetNumber, Autodesk.Revit.DB.FamilySymbol titleBlockFamilySymbol, IEnumerable<Autodesk.Revit.DB.View> views)
        {

            //Phase 1 - Check to see if the object exists
            var oldEle =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ViewSheet>(Document);

            // Rebind to Element
            if (oldEle != null)
            {
                InternalSetViewSheet(oldEle);
                InternalSetSheetName(sheetName);
                InternalSetSheetNumber(sheetNumber);
                InternalSetTitleBlock(titleBlockFamilySymbol.Id);
                InternalAddViewsToSheetView(views);

                return;
            }

            //Phase 2 - There was no existing Element, create new one
            TransactionManager.Instance.EnsureInTransaction(Document);

            // create sheet with title block ID
            var sheet = Autodesk.Revit.DB.ViewSheet.Create(Document, titleBlockFamilySymbol.Id);

            InternalSetViewSheet(sheet);
            InternalSetSheetName(sheetName);
            InternalSetSheetNumber(sheetNumber);
            InternalAddViewsToSheetView(views);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(this.InternalElement);

        }

        /// <summary>
        /// Initialize a Sheet element with a name, number and specific titleblock.
        /// </summary>
        /// <param name="sheetName">name of the sheet.</param>
        /// <param name="sheetNumber">sheet number.</param>
        /// <param name="titleBlockFamilySymbol">sheet titleblock.</param>
        private void InitSheet(string sheetName, string sheetNumber, Autodesk.Revit.DB.FamilySymbol titleBlockFamilySymbol)
        {
            //Phase 1 - Check to see if the object exists
            var oldEle =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ViewSheet>(Document);

            // Rebind to Element
            if (oldEle != null)
            {
                InternalSetViewSheet(oldEle);
                InternalSetSheetName(sheetName);
                InternalSetSheetNumber(sheetNumber);
                InternalSetTitleBlock(titleBlockFamilySymbol.Id);

                return;
            }

            //Phase 2 - There was no existing Element, create new one
            TransactionManager.Instance.EnsureInTransaction(Document);

            // create sheet with title block ID
            var sheet = Autodesk.Revit.DB.ViewSheet.Create(Document, titleBlockFamilySymbol.Id);

            InternalSetViewSheet(sheet);
            InternalSetSheetName(sheetName);
            InternalSetSheetNumber(sheetNumber);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="sheetNumber"></param>
        /// <param name="titleBlockFamilySymbol"></param>
        /// <param name="views"></param>
        /// <param name="locations"></param>
        private void InitSheet(string sheetName, string sheetNumber, Autodesk.Revit.DB.FamilySymbol titleBlockFamilySymbol, IEnumerable<Autodesk.Revit.DB.View> views, IEnumerable<XYZ> locations)
        {
            //Phase 1 - Check to see if the object exists
            var oldEles =
                ElementBinder.GetElementsFromTrace<Autodesk.Revit.DB.ViewSheet>(Document);

            List<Autodesk.Revit.DB.Element> TraceElements = new List<Autodesk.Revit.DB.Element>();
            List<Autodesk.Revit.DB.Viewport> BindingViewports = new List<Autodesk.Revit.DB.Viewport>();
            if (ElementBinder.GetElementsFromTrace<Autodesk.Revit.DB.Viewport>(Document) != null)
                BindingViewports.AddRange(ElementBinder.GetElementsFromTrace<Autodesk.Revit.DB.Viewport>(Document).ToList());

            // Rebind to Element
            if (oldEles != null && oldEles.Any())
            {
                var oldEle = oldEles.First();
                InternalSetViewSheet(oldEle);
                InternalSetSheetName(sheetName);
                InternalSetSheetNumber(sheetNumber);
                InternalSetTitleBlock(titleBlockFamilySymbol.Id);
                TraceElements.Add(oldEle);
                TraceElements.AddRange(InternalSetViewsToSheetView(views, locations, BindingViewports));

                ElementBinder.SetElementsForTrace(TraceElements);

                return;
            }

            //Phase 2 - There was no existing Element, create new one
            TransactionManager.Instance.EnsureInTransaction(Document);

            // create sheet with title block ID
            var sheet = Autodesk.Revit.DB.ViewSheet.Create(Document, titleBlockFamilySymbol.Id);

            InternalSetViewSheet(sheet);
            InternalSetSheetName(sheetName);
            InternalSetSheetNumber(sheetNumber);
            TraceElements.Add(this.InternalElement);
            TraceElements.AddRange(InternalSetViewsToSheetView(views, locations, BindingViewports));

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementsForTrace(TraceElements);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// This method sets the collection of views with locations to the existing ViewSheet
        /// </summary>
        /// <param name="views"></param>
        /// <param name="locations"></param>
        /// <param name="currentViewports"></param>
        private List<Autodesk.Revit.DB.Element> InternalSetViewsToSheetView(IEnumerable<Autodesk.Revit.DB.View> views, IEnumerable<XYZ> locations, List<Autodesk.Revit.DB.Viewport> currentViewports)
        {
            var dict = new Dictionary<Autodesk.Revit.DB.ElementId, XYZ>();
            for (int i = 0; i < views.Count(); i++)
            {
                dict.Add(views.ElementAt(i).Id, locations.ElementAt(i));
            }

            var sheet = InternalViewSheet;
            List<Autodesk.Revit.DB.Element> TraceViewports = new List<Autodesk.Revit.DB.Element>();
            //var currentViewports = InternalViewSheet.GetAllViewports().Select(x => Document.GetElement(x)).OfType<Autodesk.Revit.DB.Viewport>().ToList();
            var viewsId = views.Select(x => x.Id);
            var currentViewId = currentViewports.Select(x => x.ViewId);
            var shouldDeleteViewports = currentViewports.FindAll(x => !viewsId.Contains(x.ViewId));
            var shouldResetLocationViewports = currentViewports.FindAll(x => viewsId.Contains(x.ViewId));
            var shouldAddViews = views.ToList().FindAll(x => !currentViewId.Contains(x.Id)).Select(x => x.Id);

            TransactionManager.Instance.EnsureInTransaction(Document);

            if (shouldDeleteViewports != null && shouldDeleteViewports.Any())
            {
                foreach (var viewport in shouldDeleteViewports)
                    Document.Delete(viewport.Id);
            }
            if (shouldResetLocationViewports != null && shouldResetLocationViewports.Any())
            {
                foreach (var viewport in shouldResetLocationViewports)
                {
                    var boxCenter = dict[viewport.ViewId];
                    viewport.SetBoxCenter(boxCenter);
                    TraceViewports.Add(viewport);
                }
            }
            if (shouldAddViews != null && shouldAddViews.Any())
            {
                foreach (var viewId in shouldAddViews)
                {
                    var boxCenter = dict[viewId];
                    var viewport = Autodesk.Revit.DB.Viewport.Create(Document, sheet.Id, viewId, boxCenter);
                    TraceViewports.Add(viewport);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();

            return TraceViewports;
        }

        /// <summary>
        /// This method adds the collection of views to the existing ViewSheet and packs them 
        /// </summary>
        /// <param name="views"></param>
        private void InternalAddViewsToSheetView(IEnumerable<Autodesk.Revit.DB.View> views)
        {
            var sheet = InternalViewSheet;

            TransactionManager.Instance.EnsureInTransaction(Document);

            // (sic) from Dynamo Legacy
            var width = sheet.Outline.Max.U - sheet.Outline.Min.U;
            var height = sheet.Outline.Max.V - sheet.Outline.Min.V;
            var packer = new CygonRectanglePacker(width, height);
            int count = 0;

            foreach (var view in views)
            {
                var viewWidth = view.Outline.Max.U - view.Outline.Min.U;
                var viewHeight = view.Outline.Max.V - view.Outline.Min.V;

                Autodesk.Revit.DB.UV placement = null;
                if (packer.TryPack(viewWidth, viewHeight, out placement))
                {
                    var dbViews = sheet.GetAllPlacedViews().Select(x => Document.GetElement(x)).
                        OfType<Autodesk.Revit.DB.View>();
                    if (dbViews.Contains(view))
                    {
                        //move the view
                        //find the corresponding viewport
                        var enumerable =
                            DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Viewport>()
                                .Where(x => x.SheetId == sheet.Id && x.ViewId == view.Id).ToArray();

                        if (!enumerable.Any())
                            continue;

                        var viewport = enumerable.First();
                        viewport.SetBoxCenter(new XYZ(placement.U + viewWidth / 2, placement.V + viewHeight / 2, 0));
                    }
                    else
                    {
                        //place the view on the sheet
                        if (Autodesk.Revit.DB.Viewport.CanAddViewToSheet(Document, sheet.Id, view.Id))
                        {
                            var viewport = Autodesk.Revit.DB.Viewport.Create(Document, sheet.Id, view.Id,
                                                           new XYZ(placement.U + viewWidth / 2, placement.V + viewHeight / 2, 0));
                        }
                    }
                }
                else
                {
                    throw new Exception(String.Format(Properties.Resources.ViewCantPackOnSheet,
                        count, width, height, viewWidth, viewHeight));
                }

                count++;
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Set the InternalViewSheet property and the associated element id and unique id
        /// </summary>
        /// <param name="floor"></param>
        private void InternalSetViewSheet(Autodesk.Revit.DB.ViewSheet floor)
        {
            this.InternalViewSheet = floor;
            this.InternalElementId = floor.Id;
            this.InternalUniqueId = floor.UniqueId;
        }

        /// <summary>
        /// Set the name of the sheet
        /// </summary>
        /// <param name="name"></param>
        private void InternalSetSheetName(string name)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var param = InternalViewSheet.get_Parameter(BuiltInParameter.SHEET_NAME);
            param.Set(name);

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Set the sheet number of the sheet
        /// </summary>
        /// <param name="number"></param>
        private void InternalSetSheetNumber(string number)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var param = InternalViewSheet.get_Parameter(BuiltInParameter.SHEET_NUMBER);
            param.Set(number);

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Set the title block id for the view
        /// </summary>
        /// <param name="newTitleBlockId"></param>
        private void InternalSetTitleBlock(ElementId newTitleBlockId)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            // element collector result
            new FilteredElementCollector(Document, InternalViewSheet.Id).OfCategory(BuiltInCategory.OST_TitleBlocks)
                .ToElements().ToArray().ForEach(x => x.ChangeTypeId(newTitleBlockId));

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get the SheetName of the Sheet
        /// </summary>
        public string SheetName
        {
            get
            {
                return InternalViewSheet.Name;
            }
        }

        /// <summary>
        /// Get the SheetNumber of the Sheet
        /// </summary>
        public string SheetNumber
        {
            get
            {
                return InternalViewSheet.SheetNumber;
            }
        }

        /// <summary>
        /// Set the SheetNumber of the Sheet
        /// </summary>
        /// <param name="sheetNumber"></param>
        /// <returns></returns>
        public Sheet SetSheetNumber(string sheetNumber)
        {
            InternalSetSheetNumber(sheetNumber);
            return this;
        }

        /// <summary>
        /// Set the SheetName of the Sheet
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public Sheet SetSheetName(string sheetName)
        {
            InternalSetSheetName(sheetName);
            return this;
        }

        /// <summary>
        /// Get the Views on a Sheet
        /// </summary>
        public View[] Views
        {
            get
            {
                return
                    InternalViewSheet.GetAllPlacedViews().Select(x => Document.GetElement(x)).OfType<Autodesk.Revit.DB.View>()
                        .Select(x => (View)ElementWrapper.ToDSType(x, true))
                        .ToArray();
            }
        }

        /// <summary>
        /// Get Viewports from a given sheet
        /// </summary>
        public Viewport[] Viewports
        {
            get
            {
                return InternalViewSheet.GetAllViewports().Select(x => Document.GetElement(x)).OfType<Autodesk.Revit.DB.Viewport>()
                        .Select(x => (Viewport)ElementWrapper.ToDSType(x, true))
                        .ToArray();
            }
        }

        /// <summary>
        /// Get ScheduleGraphics from a given sheet
        /// </summary>
        public ScheduleOnSheet[] Schedules
        {
            get
            {
                var elements = new FilteredElementCollector(Document, this.InternalViewSheet.Id).OfType<Autodesk.Revit.DB.ScheduleSheetInstance>();
                return elements.Select(x => (ScheduleOnSheet)ElementWrapper.ToDSType(x, true))
                        .ToArray();
            }
        }

        /// <summary>
        /// Get TitleBlocks of the Sheet
        /// </summary>
        /// <returns name="titleblock">The sheet's titleblock.</returns>
        public Element[] TitleBlock
        {
            get
            {
                var elements = new FilteredElementCollector(Document, InternalElementId).OfCategory(BuiltInCategory.OST_TitleBlocks);
                return elements.ToElements().Select(e => e.ToDSType(true)).ToArray();
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Sheet.  
        /// This method will automatically pack the views onto the sheet. 
        /// </summary>
        /// <param name="sheetName">Sheet Name as String.</param>
        /// <param name="sheetNumber">Sheet Number as String.</param>
        /// <param name="titleBlockFamilyType">Titleblock that will be assigned to created Sheet.</param>
        /// <param name="views">Views to be placed on Sheet.</param>
        /// <returns></returns>
        public static Sheet ByNameNumberTitleBlockAndViews(string sheetName, string sheetNumber, FamilyType titleBlockFamilyType, View[] views)
        {
            if (sheetName == null)
            {
                throw new ArgumentNullException("sheetName");
            }

            if (sheetNumber == null)
            {
                throw new ArgumentNullException("sheetNumber");
            }

            if (titleBlockFamilyType == null)
            {
                throw new ArgumentNullException("titleBlockFamilyType");
            }

            if (views == null)
            {
                throw new ArgumentNullException("views");
            }

            if (views.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.Sheet_NoViewsError);
            }

            return new Sheet(sheetName, sheetNumber, titleBlockFamilyType.InternalFamilySymbol, views.Select(x => x.InternalView));
        }

        /// <summary>
        /// Create a Revit Sheet.  
        /// This method will automatically pack the view onto the sheet.
        /// </summary>
        /// <param name="sheetName">Sheet Name as String.</param>
        /// <param name="sheetNumber">Sheet Number as String.</param>
        /// <param name="titleBlockFamilyType">Titleblock that will be assigned to created Sheet.</param>
        /// <param name="view">Views to be placed on Sheet.</param>
        /// <returns></returns>
        public static Sheet ByNameNumberTitleBlockAndView(string sheetName, string sheetNumber, FamilyType titleBlockFamilyType, View view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            return Sheet.ByNameNumberTitleBlockAndViews(sheetName, sheetNumber, titleBlockFamilyType, new[] { view });
        }

        /// <summary>
        /// Create a Revit Sheet by the sheet name, number and a title block FamilyType.
        /// </summary>
        /// <param name="sheetName">Sheet Name as String.</param>
        /// <param name="sheetNumber">Sheet Number as String.</param>
        /// <param name="titleBlockFamilyType">Titleblock that will be assigned to created Sheet.</param>
        /// <returns>The new empty Revit sheet.</returns>
        public static Sheet ByNameNumberTitleBlock(string sheetName, string sheetNumber, FamilyType titleBlockFamilyType)
        {
            if (sheetName == null)
                throw new ArgumentNullException(nameof(sheetName));

            if (sheetNumber == null)
                throw new ArgumentNullException(nameof(sheetNumber));

            if (titleBlockFamilyType == null)
                throw new ArgumentNullException(nameof(titleBlockFamilyType));

            return new Sheet(sheetName, sheetNumber, titleBlockFamilyType.InternalFamilySymbol);
        }

        /// <summary>
        /// Create a Revit Sheet.
        /// This method will automatically set the views with locations onto the sheet.
        /// </summary>
        /// <param name="sheetName">Sheet Name as String.</param>
        /// <param name="sheetNumber">Sheet Number as String.</param>
        /// <param name="titleBlockFamilyType">Titleblock that will be assigned to created Sheet.</param>
        /// <param name="views">Views to be placed on Sheet.</param>
        /// <param name="locations">View's location on Sheet.</param>
        /// <returns></returns>
        public static Sheet ByNameNumberTitleBlockViewsAndLocations(string sheetName, string sheetNumber, FamilyType titleBlockFamilyType, View[] views, Autodesk.DesignScript.Geometry.Point[] locations)
        {
            if (sheetName == null)
            {
                throw new ArgumentNullException("sheetName");
            }

            if (sheetNumber == null)
            {
                throw new ArgumentNullException("sheetNumber");
            }

            if (titleBlockFamilyType == null)
            {
                throw new ArgumentNullException("titleBlockFamilyType");
            }

            if (views == null)
            {
                throw new ArgumentNullException("views");
            }

            if (locations == null)
            {
                throw new ArgumentNullException("locations");
            }

            if (views.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.Sheet_NoViewsError);
            }

            if (locations.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.Sheet_NoLocationSet);
            }

            if (views.Length != locations.Length)
            {
                throw new ArgumentException(Properties.Resources.Sheet_ViewLocationMismatch);
            }

            return new Sheet(sheetName, sheetNumber, titleBlockFamilyType.InternalFamilySymbol, views.Select(x => x.InternalView), locations.Select(x =>x.ToXyz()));
        }

        /// <summary>
        /// Create a Revit Sheet.
        /// This method will automatically set the view with location onto the sheet.
        /// </summary>
        /// <param name="sheetName">Sheet Name as String.</param>
        /// <param name="sheetNumber">Sheet Number as String.</param>
        /// <param name="titleBlockFamilyType">Titleblock that will be assigned to created Sheet.</param>
        /// <param name="view">View to be placed on Sheet.</param>
        /// <param name="location">View's location on Sheet.</param>
        /// <returns></returns>
        public static Sheet ByNameNumberTitleBlockViewAndLocation(string sheetName, string sheetNumber, FamilyType titleBlockFamilyType, View view, Autodesk.DesignScript.Geometry.Point location)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (location == null) 
            {
                throw new ArgumentNullException("location");
            }

            return ByNameNumberTitleBlockViewsAndLocations(sheetName, sheetNumber, titleBlockFamilyType, new[] { view }, new[] { location });
        }

        /// <summary>
        /// Duplicates A Sheet. 
        /// </summary>
        /// <param name="sheet">The Sheet to be Duplicated.</param>
        /// <param name="duplicateWithContents">Set to true that Duplicate sheet with contents</param>
        /// <param name="duplicateWithView">Set to true that Duplicate sheet with views.</param>
        /// <param name="viewDuplicateOption">Enter View Duplicate Option: Duplicate, AsDependent or WithDetailing.</param>
        /// <param name="prefix"></param>
        /// <param name="suffix">When prefix and suffix are both empty, suffix will set a default value - " - Copy".</param>
        /// <returns></returns>
        public static Sheet DuplicateSheet(Sheet sheet, bool duplicateWithContents = false, bool duplicateWithView = false, string viewDuplicateOption = "Duplicate", string prefix = "", string suffix = "")
        {            
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet));

            ViewDuplicateOption Option = (ViewDuplicateOption)Enum.Parse(typeof(ViewDuplicateOption), viewDuplicateOption);

            if (String.IsNullOrEmpty(prefix) && String.IsNullOrEmpty(suffix))
            {
                suffix = " - Copy";
            }

            Sheet newSheet = null;

            try
            {
                RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);                

                var oldElements = ElementBinder.GetElementsFromTrace<Autodesk.Revit.DB.Element>(Document);
                List<ElementId> elementIds = new List<ElementId>();
                var newSheetNumber = prefix + sheet.SheetNumber + suffix;
                var newSheetName = sheet.SheetName;
                List<Autodesk.Revit.DB.Element> TraceElements = new List<Autodesk.Revit.DB.Element>();

                if (oldElements != null)
                {
                    foreach (var element in oldElements)
                    {
                        elementIds.Add(element.Id);
                        if(element is ViewSheet)
                        {
                            var oldSheet = (element as ViewSheet).ToDSType(false) as Sheet;
                            if (oldSheet.SheetNumber.Equals(newSheetNumber))
                            {
                                if ((duplicateWithView && oldElements.Count() > 1) || (!duplicateWithView && oldElements.Count() == 1))  
                                {
                                    newSheet = oldSheet;
                                    TraceElements.AddRange(oldElements);
                                }
                                if(newSheet != null)
                                {
                                    if (duplicateWithContents)
                                        DuplicateSheetAnnotations(sheet, newSheet);
                                    else
                                        DeleteSheetAnnotations(newSheet);
                                }

                            }
                        }
                    }
#if !DESIGN_AUTOMATION
                    if(newSheet == null)
                    {
                        Autodesk.Revit.UI.UIDocument uIDocument = new Autodesk.Revit.UI.UIDocument(Document);
                        var openedViews = uIDocument.GetOpenUIViews().ToList();
                        var shouldClosedViews = openedViews.FindAll(x => elementIds.Contains(x.ViewId));
                        if (shouldClosedViews.Count > 0)
                        {
                            foreach (var v in shouldClosedViews)
                            {
                                if (uIDocument.GetOpenUIViews().ToList().Count() > 1)
                                    v.Close();
                                else
                                    throw new InvalidOperationException(string.Format(Properties.Resources.CantCloseLastOpenView, v.ToString()));
                            }
                        }
                        Document.Delete(elementIds);
                    }  
#endif
                }

                if (newSheet == null && TraceElements.Count == 0)
                {
                    // Create a new Sheet with different SheetNumber
                    var titleBlockElement = sheet.TitleBlock.First() as FamilyInstance;
                    FamilySymbol TitleBlock = titleBlockElement.InternalFamilyInstance.Symbol;
                    FamilyType titleBlockFamilyType = ElementWrapper.Wrap(TitleBlock, true);

                    if (!CheckUniqueSheetNumber(newSheetNumber))
                    {
                        throw new ArgumentException(String.Format(Properties.Resources.SheetNumberExists, newSheetNumber));
                    }

                    var viewSheet = Autodesk.Revit.DB.ViewSheet.Create(Document, TitleBlock.Id);
                    newSheet = new Sheet(viewSheet);
                    newSheet.InternalSetSheetName(newSheetName);
                    newSheet.InternalSetSheetNumber(newSheetNumber);
                    SetSheetInformation(sheet, newSheet);

                    TraceElements.Add(newSheet.InternalElement);

                    // Copy Annotation Elements from sheet to new sheet by ElementTransformUtils.CopyElements
                    if(duplicateWithContents)
                        DuplicateSheetAnnotations(sheet, newSheet);
                    
                    if (duplicateWithView)
                    {
                        // Copy ScheduleSheetInstance except RevisionSchedule from sheet to new sheet by ElementTransformUtils.CopyElements
                        DuplicateScheduleSheetInstance(sheet, newSheet);

                        // Duplicate Viewport in sheet and place on new sheet
                        TraceElements.AddRange(DuplicateViewport(sheet, newSheet, Option, prefix, suffix));
                    }
                        
                }                
                                
                ElementBinder.SetElementsForTrace(TraceElements);
                RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            }
            catch (Exception e)
            {
                if(newSheet != null)
                {
                    newSheet.Dispose();
                }
                throw e;
            }
            
            return newSheet;
        }

#endregion

        #region Internal static constructors

        /// <summary>
        /// Create a View from a user selected Element.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Sheet FromExisting(Autodesk.Revit.DB.ViewSheet view, bool isRevitOwned)
        {
            return new Sheet(view)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Private Helper

        private static void SetSheetInformation(Sheet oldSheet, Sheet newSheet)
        {
            List<BuiltInParameter> Filters = new List<BuiltInParameter>
            {
                BuiltInParameter.SHEET_APPROVED_BY,
                BuiltInParameter.SHEET_DESIGNED_BY,
                BuiltInParameter.SHEET_CHECKED_BY,
                BuiltInParameter.SHEET_DRAWN_BY
            };
            foreach(var parameter in Filters)
            {
                var oldParam = oldSheet.InternalViewSheet.get_Parameter(parameter);
                var value = oldParam.AsString();
                var newParam = newSheet.InternalViewSheet.get_Parameter(parameter);
                newParam.Set(value);
            }
        }

        private static void DuplicateSheetAnnotations(Sheet oldSheet, Sheet newSheet)
        {
            List<BuiltInCategory> Filters = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_IOSDetailGroups,
                BuiltInCategory.OST_Dimensions,
                BuiltInCategory.OST_GenericAnnotation,
                BuiltInCategory.OST_Lines,
                BuiltInCategory.OST_TextNotes
            };
            List<ElementId> list = new List<ElementId>();
            List<ElementId> currentList = new List<ElementId>();
            foreach(var category in Filters)
            {
                list.AddRange(new FilteredElementCollector(Document, oldSheet.InternalElementId).OfCategory(category).ToElementIds());
                currentList.AddRange(new FilteredElementCollector(Document, newSheet.InternalElementId).OfCategory(category).ToElementIds());
            }
            if(list.Any<ElementId>())
            {
                if (currentList.Any<ElementId>() && currentList.Count == list.Count)
                    return;
                else if (currentList.Any<ElementId>() && currentList.Count != list.Count)
                    DeleteSheetAnnotations(newSheet);
                ElementTransformUtils.CopyElements(oldSheet.InternalViewSheet, list, newSheet.InternalViewSheet, null, null);
            }
        }

        private static void DeleteSheetAnnotations(Sheet sheet)
        {
            List<BuiltInCategory> Filters = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_IOSDetailGroups,
                BuiltInCategory.OST_Dimensions,
                BuiltInCategory.OST_GenericAnnotation,
                BuiltInCategory.OST_Lines,
                BuiltInCategory.OST_TextNotes
            };
            List<ElementId> list = new List<ElementId>();
            foreach (var category in Filters)
            {
                list.AddRange(new FilteredElementCollector(Document, sheet.InternalElementId).OfCategory(category).ToElementIds());
            }
            if(list.Any<ElementId>())
            {
                foreach(var id in list)
                {
                    Document.Delete(id);
                }
            }
        }

        private static void DuplicateScheduleSheetInstance(Sheet oldSheet, Sheet newSheet)
        {
            List<ElementId> list = new List<ElementId>();
            foreach(var schedule in oldSheet.Schedules)
            {
                if(!schedule.InternalScheduleOnSheet.IsTitleblockRevisionSchedule)
                {
                    list.Add(schedule.InternalElement.Id);
                }
            }
            if (list.Any<ElementId>())
            {
                ElementTransformUtils.CopyElements(oldSheet.InternalViewSheet, list, newSheet.InternalViewSheet, null, null);                
            }
        }

        /// <summary>
        /// A list that temporarily stores Views generated by Duplicate, will dispose when sheet disposing.
        /// </summary>
        private List<Revit.Elements.Views.View> DuplicateViews = null;

        private static List<Autodesk.Revit.DB.Element> DuplicateViewport(Sheet oldSheet, Sheet newSheet, ViewDuplicateOption viewDuplicateOption, string prefix, string suffix)
        {
            var Viewports = oldSheet.Viewports.ToList();
            List<Revit.Elements.Views.View> viewList = new List<View>();
            List<Autodesk.Revit.DB.Element> elements = new List<Autodesk.Revit.DB.Element>();
            List<Autodesk.DesignScript.Geometry.Point> locationList = new List<Autodesk.DesignScript.Geometry.Point>();
            foreach (var viewport in Viewports) 
            {                
                if (viewport.View.InternalView.CanViewBeDuplicated(viewDuplicateOption))
                {
                    var newViewName = prefix + viewport.View.Name + suffix;
                    if (!CheckUniqueViewName(newViewName))
                        throw new ArgumentException(String.Format(Properties.Resources.ViewNameExists, newViewName));
                    var newViewId = viewport.View.InternalView.Duplicate(viewDuplicateOption);
                    var newView = (Document.GetElement(newViewId) as Autodesk.Revit.DB.View).ToDSType(false) as View;
                    var param = newView.InternalView.get_Parameter(BuiltInParameter.VIEW_NAME);
                    param.Set(prefix + viewport.View.Name + suffix);
                    viewList.Add(newView);
                    elements.Add(newView.InternalElement);
                }
                else
                {
                    throw new Exception(String.Format(Properties.Resources.ViewCantBeDuplicated, viewport.View.Name));
                }

                locationList.Add(viewport.BoxCenter);
            }

            newSheet.DuplicateViews = viewList;

            if (viewList.Count() == locationList.Count() && viewList.Any())
            {
                for (int i = 0; i < viewList.Count(); i++)
                {
                    ElementId sheetId = newSheet.InternalView.Id;
                    ElementId viewId = viewList[i].InternalView.Id;
                    XYZ viewLocation = Revit.GeometryConversion.GeometryPrimitiveConverter.ToRevitType(locationList[i]);

                    if (!Autodesk.Revit.DB.Viewport.CanAddViewToSheet(Document, sheetId, viewId))
                    {
                        throw new InvalidOperationException(Properties.Resources.ViewAlreadyPlacedOnSheet);
                    }
                    Autodesk.Revit.DB.Viewport.Create(Document, sheetId, viewId, viewLocation);
                }
            }

            return elements;
        }

        private static Boolean CheckUniqueSheetNumber(String SheetNumber)
        {
            bool IsUnique = true;

            var sheets = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(ViewSheet))
                .ToList();
            foreach (var s in sheets)
            {
                if((s as ViewSheet).SheetNumber.Equals(SheetNumber))
                {
                    IsUnique = false;
                    break;
                }
            }

            return IsUnique;
        }

        private static Boolean CheckUniqueViewName(String ViewName)
        {
            bool IsUnique = true;

            var views = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.View))
                .ToList();
            foreach (var v in views)
            {
                if (v.Name.Equals(ViewName))
                {
                    IsUnique = false;
                    break;
                }
            }

            return IsUnique;
        }

        #endregion
    }
}
