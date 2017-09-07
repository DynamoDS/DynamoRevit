using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Autodesk.Revit.DB;

namespace Revit.Elements.Views
{
    /// <summary>
    ///     An abstract Revit View - All view types inherit from this type
    /// </summary>
    public abstract class View : Element
    {
        /// <summary>
        ///     Obtain the reference Element as a View
        /// </summary>
        internal Autodesk.Revit.DB.View InternalView
        {
            get
            {
                return (Autodesk.Revit.DB.View) InternalElement;
            }
        }

        /// <summary>
        ///     Check if this type of view supports annotative elements
        /// </summary>
        /// <returns></returns>
        internal bool IsAnnotationView()
        {
            switch (this.InternalView.ViewType)
            {
                case Autodesk.Revit.DB.ViewType.FloorPlan:
                case Autodesk.Revit.DB.ViewType.EngineeringPlan:
                case Autodesk.Revit.DB.ViewType.Detail:
                case Autodesk.Revit.DB.ViewType.Section:
                case Autodesk.Revit.DB.ViewType.Elevation:
                case Autodesk.Revit.DB.ViewType.CeilingPlan:
                case Autodesk.Revit.DB.ViewType.DraftingView:
                case Autodesk.Revit.DB.ViewType.AreaPlan:
                case Autodesk.Revit.DB.ViewType.DrawingSheet:
                    return true;

                default: return false;
            }
        }

        /// <summary>
        ///     Export the view as an image to the given path - defaults to png, but you can override 
        ///     the file type but supplying a path with the appropriate extension
        /// </summary>
        /// <param name="path">A valid path for the image</param>
        /// <returns>The image</returns>
        public Bitmap ExportAsImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Properties.Resources.View_ExportAsImage_Path_Invalid, "path");
            }

            var fileType = ImageFileType.PNG;
            string extension = ".png";
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

            options.SetViewsAndSheets(new List<ElementId>{InternalView.Id});

            Document.ExportImage(options);

            var pathName = Path.Combine(
                            Path.GetDirectoryName(path),
                            Path.GetFileNameWithoutExtension(path));

            // Revit outputs file with a bunch of crap in the file name, let's construct that
            var actualFn = string.Format("{0} - {1} - {2}{3}", pathName, ViewTypeString(InternalView.ViewType),
                InternalView.ViewName, extension);

            // and the intended destination
            var destFn = pathName + extension;

            // rename the file
            if (File.Exists(destFn)) File.Delete(destFn);
            File.Move(actualFn, destFn);
            
            Bitmap bmp;
            try
            {
                using (var fs = new FileStream(destFn, FileMode.Open))
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

        public override string ToString()
        {
            return GetType().Name + "(Name = " + InternalView.ViewName + " )";
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
        /// <param name="visible">If True given Filter will show filtered elements.</param>
        /// <returns name="view">View</returns>
        public Revit.Elements.Views.View SetFilterOverrides(Revit.Filter.ParameterFilterElement parameterFilter, Revit.Filter.OverrideGraphicSettings overrides, bool visible = true)
        {
            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            this.InternalView.SetFilterOverrides(parameterFilter.InternalElement.Id, overrides.InternalOverrideGraphicSettings);
            this.InternalView.SetFilterVisibility(parameterFilter.InternalElement.Id, visible);
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
        ///     Set View Scale
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

        #endregion
    }
}
