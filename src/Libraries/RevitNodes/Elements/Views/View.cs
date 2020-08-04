using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using System.Linq;
using Revit.GeometryConversion;

namespace Revit.Elements.Views
{
    /// <summary>
    /// An abstract Revit View - All view types inherit from this type
    /// </summary>
    public abstract class View : Element
    {
        /// <summary>
        /// Obtain the reference Element as a View
        /// </summary>
        internal Autodesk.Revit.DB.View InternalView
        {
            get
            {
                Autodesk.Revit.DB.View internalElement = (Autodesk.Revit.DB.View)InternalElement;
                return internalElement.IsValidObject ? internalElement : null;
            }
        }

        /// <summary>
        /// Check if this type of view supports annotative elements
        /// </summary>
        /// <returns></returns>
        internal bool IsAnnotationView()
        {
            switch (this.InternalView.ViewType)
            {
                case ViewType.FloorPlan:
                case ViewType.EngineeringPlan:
                case ViewType.Detail:
                case ViewType.Section:
                case ViewType.Elevation:
                case ViewType.CeilingPlan:
                case ViewType.DraftingView:
                case ViewType.DrawingSheet:
                case ViewType.AreaPlan:
                case ViewType.Legend:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        /// Export the view as an image to the given path - defaults to png, but you can override 
        /// the file type but supplying a path with the appropriate extension.
        /// </summary>
        /// <param name="path">A valid path for the image.</param>
        /// <returns>A Bitmap Image.</returns>
        public Bitmap ExportAsImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Properties.Resources.View_ExportAsImage_Path_Invalid, nameof(path));
            }

            var fileType = ImageFileType.PNG;
            var extension = ".png";
            if (Path.HasExtension(path))
            {
                extension = Path.GetExtension(path);
                switch (extension.ToLower())
                {
                    case ".jpg":
                        fileType = ImageFileType.JPEGLossless;
                        break;
                    case ".png":
                        fileType = ImageFileType.PNG;
                        break;
                    case ".bmp":
                        fileType = ImageFileType.BMP;
                        break;
                    case ".tga":
                        fileType = ImageFileType.TARGA;
                        break;
                    case ".tif":
                        fileType = ImageFileType.TIFF;
                        break;
                }
            }

            var options = new ImageExportOptions
            {
                ExportRange = ExportRange.SetOfViews,
                FilePath = path,
                FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = fileType,
                ImageResolution = ImageResolution.DPI_150,
                ShadowViewsFileType = fileType,
                ShouldCreateWebSite = false,
                ViewName = Guid.NewGuid().ToString(),
                Zoom = 100,
                ZoomType = ZoomFitType.Zoom
            };

            options.SetViewsAndSheets(new List<ElementId> { InternalView.Id });

            Document.ExportImage(options);

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var revitName = Autodesk.Revit.DB.ImageExportOptions.GetFileName(doc, InternalView.Id);
            var directoryName = Path.GetDirectoryName(path);
            var actualFileName = string.Empty;
            var destinationFileName = string.Empty;

            if (directoryName != null)
            {
                actualFileName = Path.Combine(
                    directoryName,
                    Path.GetFileNameWithoutExtension(path) + revitName + extension);

                destinationFileName = Path.Combine(
                    directoryName,
                    Path.GetFileNameWithoutExtension(path) + extension);
            }

            if (string.IsNullOrEmpty(actualFileName) || string.IsNullOrEmpty(destinationFileName))
                throw new ArgumentException(Properties.Resources.View_ExportAsImage_Path_Invalid, nameof(path));

            // rename the file
            if (File.Exists(actualFileName))
            {
                try
                {
                    File.Delete(destinationFileName);
                    File.Move(actualFileName, destinationFileName);
                }
                catch (Exception ex)
                {
                    throw new Exception(Properties.Resources.ViewExportImageLockedError, ex);
                }
            }

            Bitmap bmp;
            try
            {
                using (var fs = new FileStream(destinationFileName, FileMode.Open))
                {
                    bmp = new Bitmap(Image.FromStream(fs));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.ViewExportImageError, ex);
            }

            return bmp;
        }

        #region ViewType
        private static string ViewTypeString(ViewType vt)
        {
            switch (vt)
            {
                case ViewType.ThreeD:
                    return "3D View";
                case ViewType.AreaPlan:
                    return "Area Plan";
                case ViewType.CeilingPlan:
                    return "Ceiling Plan";
                case ViewType.EngineeringPlan:
                    return "Engineering Plan";
                case ViewType.FloorPlan:
                    return "Floor Plan";
                case ViewType.Elevation:
                    return "Elevation";
                default:
                    return "Section View";
            }
        }
        #endregion

        #region Discipline

        private static string ViewDisciplineString(ViewDiscipline vd)
        {
            string discipline = "";
            switch(vd)
            {
                case ViewDiscipline.Architectural:
                case ViewDiscipline.Coordination:
                case ViewDiscipline.Electrical:
                case ViewDiscipline.Mechanical:
                case ViewDiscipline.Plumbing:
                case ViewDiscipline.Structural:
                    discipline = vd.ToString();
                    break;
                default:
                    discipline = Properties.Resources.InvalidDiscipline;
                    break;
            }

            return discipline;
        }

        /// <summary>
        ///  The Discipline of the view. 
        /// </summary>
        public string Discipline
        {
            get
            {
                return ViewDisciplineString(InternalView.Discipline);
            }
        }

        /// <summary>
        ///  Set Discipline of View.
        /// </summary>
        /// <param name="discipline"></param>
        /// <returns></returns>
        public View SetDiscipline(string discipline)
        {
            ViewDiscipline viewDiscipline;
            viewDiscipline = (ViewDiscipline)Enum.Parse(typeof(ViewDiscipline), discipline);

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            var param = InternalView.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE);
            param.Set((int)viewDiscipline);
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        #endregion

        #region View DisplayStyle

        private static string DisplayStyleString(DisplayStyle ds)
        {
            string displaystyle = "";
            switch (ds)
            {
                case DisplayStyle.FlatColors:
                case DisplayStyle.HLR:
                case DisplayStyle.Realistic:
                case DisplayStyle.RealisticWithEdges:
                case DisplayStyle.Rendering:
                case DisplayStyle.Shading:
                case DisplayStyle.ShadingWithEdges:
                case DisplayStyle.Undefined:
                case DisplayStyle.Wireframe:
                    displaystyle = ds.ToString();
                    break;
                default:
                    displaystyle = Properties.Resources.InvalidDisplayStyle;
                    break;
            }

            return displaystyle;
        }

        /// <summary>
        ///  The DisplayStyle of the view. Returns DisplayStyle.Wireframe if the view has no display style.
        /// </summary>
        public string Displaystyle
        {
            get
            {
                return DisplayStyleString(InternalView.DisplayStyle);
            }
        }

        /// <summary>
        ///  Set DisplayStyle of View.
        /// </summary>
        /// <param name="displayStyle"></param>
        /// <returns></returns>
        public View SetDisplayStyle(string displayStyle)
        {
            DisplayStyle displaystyle;
            displaystyle = (DisplayStyle)Enum.Parse(typeof(DisplayStyle), displayStyle);

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            InternalView.DisplayStyle = displaystyle;
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #endregion

        public override string ToString()
        {
            return GetType().Name + "(Name = " + InternalView?.Name + " )";
        }

        /// <summary>
        /// The bounds of the view in paper space (in feet).
        /// </summary>
        public Autodesk.DesignScript.Geometry.BoundingBox Outline
        {
            get
            {
                var outline = InternalView.Outline;
                var Min = outline.Min.ToProtoType();
                var Max = outline.Max.ToProtoType();
                var minPoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(Min.U, Min.V);
                var maxPoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(Max.U, Max.V);

                return Autodesk.DesignScript.Geometry.BoundingBox.ByCorners(minPoint, maxPoint);
            }
        }

        /// <summary>
        /// Returns the origin of the screen.
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point Origin
        {
            get
            {
                return InternalView.Origin.ToPoint();
            }
        }

        #region Filter

        /// <summary>
        ///     Add a Filter to a View. The Filter will be added even if View has a View Template applied, which normally would prevent user from adding
        ///     Filters without first disabling or modifying the View Template.
        /// </summary>
        /// <param name="parameterFilter">Parameter filter</param>
        /// <returns name="view">View</returns>
        public Revit.Elements.Views.View AddFilter(Revit.Filter.ParameterFilterElement parameterFilter)
        {
            if (!this.InternalView.IsFilterApplied(parameterFilter.InternalElement.Id))
            {
                RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
                this.InternalView.AddFilter(parameterFilter.InternalElement.Id);
                RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            }
            return this;
        }

        /// <summary>
        ///     Get View Filters
        /// </summary>
        /// <returns name="filter">View Filters</returns>
        public IEnumerable<Revit.Filter.ParameterFilterElement> Filters
        {
            get
            {
                List<Revit.Filter.ParameterFilterElement> filters = new List<Filter.ParameterFilterElement>();
                foreach (ElementId id in this.InternalView.GetFilters())
                {
                    Element element = Revit.Elements.ElementSelector.ByElementId(id.IntegerValue);
                    filters.Add((Revit.Filter.ParameterFilterElement)element);
                }
                return filters;
            }
        }

        /// <summary>
        ///     Set Filter overrides. If View doesn't have specified Filter, it will be first added to the View and then its settings will be overriden.
        ///     This behavior will persist even if View has a View Template applied which normally would prevent user from adding Filters without first
        ///     disabling or modifying the View Template.
        /// </summary>
        /// <param name="parameterFilter">Parameter Filter</param>
        /// <param name="overrides">Graphic Overrides Settings</param>
        /// <param name="hide">If True given Filter will be hidden.</param>
        /// <returns name="view">View</returns>
        public Revit.Elements.Views.View SetFilterOverrides(Revit.Filter.ParameterFilterElement parameterFilter, Revit.Filter.OverrideGraphicSettings overrides, bool hide = false)
        {
            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            this.InternalView.SetFilterOverrides(parameterFilter.InternalElement.Id, overrides.InternalOverrideGraphicSettings);
            this.InternalView.SetFilterVisibility(parameterFilter.InternalElement.Id, hide);
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        ///     Get Filter overrides
        /// </summary>
        /// <returns name="overrides">Filter overrides</returns>
        public Revit.Filter.OverrideGraphicSettings FilterOverrides(Revit.Filter.ParameterFilterElement parameterFilter)
        {
            OverrideGraphicSettings overrides = this.InternalView.GetFilterOverrides(parameterFilter.InternalElement.Id);
            return new Revit.Filter.OverrideGraphicSettings(overrides);
        }

        #endregion

        #region View Templates

        /// <summary>
        ///     Checks if View is a View Template.
        /// </summary>
        /// <returns></returns>
        public bool IsViewTemplate()
        {
            return this.InternalView.IsTemplate;
        }

        #endregion

        #region Graphic Overrides

        /// <summary>
        ///     Set Category Overrides.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="overrides">Graphics Overrides Settings.</param>
        /// <param name="hide">If True givent Category will be hidden.</param>
        /// <returns name="view">View</returns>
        public Revit.Elements.Views.View SetCategoryOverrides(Category category, Revit.Filter.OverrideGraphicSettings overrides, bool hide = false)
        {
            Autodesk.Revit.DB.ElementId catId = new ElementId(category.Id);
            if (!this.InternalView.IsCategoryOverridable(catId))
            {
                throw new ArgumentException(Properties.Resources.CategoryVisibilityOverrideError);
            }

            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            this.InternalView.SetCategoryOverrides(catId, overrides.InternalOverrideGraphicSettings);
            this.InternalView.SetCategoryHidden(catId, hide); // Revit 2017 specific method
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        #endregion

        #region Scale

        /// <summary>
        ///     Set View Scale.
        /// </summary>
        /// <param name="scale">View scale is the ration of true model size to paper size.</param>
        /// <returns name="view">View</returns>
        public Revit.Elements.Views.View SetScale(int scale=100)
        {
            if (Autodesk.Revit.DB.View.IsValidViewScale(scale))
            {
                if (this.InternalView.Scale != scale)
                {
                    RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
                    this.InternalView.Scale = scale;
                    RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
                }

                return this;
            }
            else
            {
                throw new ArgumentNullException("scale");
            }
        }

        /// <summary>
        /// The scale of the view.
        /// </summary>
        public int Scale
        {
            get
            {
                return InternalView.Scale;
            }
        }

        #endregion

        #region Duplicate

        /// <summary>
        /// Duplicates A view. 
        /// </summary>
        /// <param name="view">The View to be Duplicated</param>
        /// <param name="viewDuplicateOption">Enter View Duplicate Option: 0 = Duplicate. 1 = AsDependent. 2 = WithDetailing.</param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static Revit.Elements.Views.View DuplicateView(View view, int viewDuplicateOption = 0, string prefix = "", string suffix = "")
        {
            View newView = null;

            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (view is Sheet)
                throw new ArgumentException(Properties.Resources.DuplicateViewCantApplySheet);

            ViewDuplicateOption Option = 0;
            switch(viewDuplicateOption)
            {
                case 0:
                    Option = ViewDuplicateOption.Duplicate;
                    break;
                case 1:
                    Option = ViewDuplicateOption.AsDependent;
                    break;
                case 2:
                    Option = ViewDuplicateOption.WithDetailing;                    
                    break;
                default:
                    throw new ArgumentException(Properties.Resources.ViewDuplicateOptionOutofRange);
            }

            try
            {
                RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
                var viewElement = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.View>(Document);

                int count = 0;
                string newViewName = "";
                if (!String.IsNullOrEmpty(prefix) || !String.IsNullOrEmpty(suffix))
                {
                    newViewName = prefix + view.Name + suffix;                    
                }
                Autodesk.Revit.UI.UIDocument uIDocument = new Autodesk.Revit.UI.UIDocument(Document);
                var openedViews = uIDocument.GetOpenUIViews().ToList();

                if (viewElement != null)
                {
                    count++;
                    var oldViewName = viewElement.Name;
                    if (oldViewName.Equals(newViewName))
                        newView = viewElement.ToDSType(false) as View;
                    else
                        count--;
                }
                
                if (count == 0) 
                {
                    if (!CheckUniqueViewName(newViewName))
                        throw new ArgumentException(String.Format(Properties.Resources.ViewNameExists, newViewName));
                    if (view.InternalView.CanViewBeDuplicated(Option))
                    {
                        var viewID = view.InternalView.Duplicate(Option);
                        var viewEle = Document.GetElement(viewID) as Autodesk.Revit.DB.View;
                        newView = ElementWrapper.ToDSType(viewEle, false) as View;
                    }
                    else
                    {
                        throw new Exception(String.Format(Properties.Resources.ViewCantBeDuplicated, view.Name));
                    }

                    if (!String.IsNullOrEmpty(newViewName))
                    {
                        var param = newView.InternalView.get_Parameter(BuiltInParameter.VIEW_NAME);
                        param.Set(newViewName);
                    }
                    if (viewElement != null)
                    {
                        var shouldClosedViews = openedViews.FindAll(x => viewElement.Id == x.ViewId);
                        if (shouldClosedViews.Count > 0)
                        {
                            foreach (var v in shouldClosedViews)
                            {
                                if (uIDocument.GetOpenUIViews().ToList().Count() > 1)
                                    v.Close();
                                else
                                    throw new InvalidOperationException(string.Format(Properties.Resources.CantCloseLastOpenView, viewElement.ToString()));
                            }
                        }
                    }                    
                }                

                ElementBinder.CleanupAndSetElementForTrace(Document, newView.InternalElement);

                RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            }
            catch (Exception e)
            {
                //if (e is Autodesk.Revit.Exceptions.InvalidOperationException)
                //    throw e;
                if (newView != null) 
                {
                    newView.Dispose();
                }
                throw e;
            }            

            return newView;
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

        #region CropBox

        /// <summary>
        /// The Crop Box applied to the view, or an outline encompassing the crop region applied to the view.
        /// </summary>
        public Autodesk.DesignScript.Geometry.BoundingBox CropBox
        {
            get
            {
                var cropBox = InternalView.CropBox;

                return Autodesk.DesignScript.Geometry.BoundingBox.ByCorners(cropBox.Min.ToPoint(), cropBox.Max.ToPoint());
            }
        }

        /// <summary>
        /// Whether or not the Crop Box/Region is active for the view.
        /// </summary>
        public bool CropBoxActive
        {
            get
            {
                return InternalView.CropBoxActive;
            }
        }

        /// <summary>
        /// Whether or not the Crop Box/Region is visible for the view.
        /// </summary>
        public bool CropBoxVisible
        {
            get
            {
                return InternalView.CropBoxVisible;
            }
        }

        /// <summary>
        /// Set CropBox Active status.
        /// </summary>
        /// <param name="IsActive"></param>
        /// <returns></returns>
        public View SetCropBoxActive(bool IsActive)
        {
            if (this.InternalView.CropBoxActive == IsActive)
                return this;
            else
            {
                RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
                InternalView.CropBoxActive = IsActive;
                RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
                return this;
            }
        }

        /// <summary>
        /// Set CropBox visible status.
        /// </summary>
        /// <param name="IsVisible"></param>
        /// <returns></returns>
        public View SetCropBoxVisible(bool IsVisible)
        {
            if (this.InternalView.CropBoxVisible == IsVisible)
                return this;
            else
            {
                RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
                InternalView.CropBoxVisible = IsVisible;
                RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
                return this;
            }
        }

        /// <summary>
        /// Set CropBox for a view.
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <returns></returns>
        public View SetCropBox(Autodesk.DesignScript.Geometry.BoundingBox boundingBox)
        {
            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            InternalView.CropBox = boundingBox.ToRevitType();
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #endregion

        #region Direction

        /// <summary>
        /// The direction towards the viewer.
        /// </summary>
        public Autodesk.DesignScript.Geometry.Vector ViewDirection
        {
            get
            {
                return InternalView.ViewDirection.ToVector();
            }
        }

        /// <summary>
        /// The direction towards the right side of the screen.
        /// </summary>
        public Autodesk.DesignScript.Geometry.Vector RightDirection
        {
            get
            {
                return InternalView.RightDirection.ToVector();
            }
        }

        #endregion

        #region View SketchPlane

        /// <summary>
        ///  The sketch plane assigned to the view for model curve creation. 
        /// </summary>
        public SketchPlane SketchPlane
        {
            get
            {
                if (InternalView.SketchPlane != null)
                    return InternalView.SketchPlane.ToDSType(true) as SketchPlane;
                else
                    return null;
            }
        }

        /// <summary>
        ///  Set SketchPlane of View.
        /// </summary>
        /// <param name="sketchPlane"></param>
        /// <returns></returns>
        public View SetSketchPlane(SketchPlane sketchPlane)
        {
            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            InternalView.SketchPlane = sketchPlane.InternalSketchPlane;
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        #endregion
    }
}
