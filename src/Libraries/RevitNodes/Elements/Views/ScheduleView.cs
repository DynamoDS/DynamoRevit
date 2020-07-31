using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Revit.Schedules;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements.Views
{
    /// <summary>
    /// Base class for Revit Plan views
    /// </summary>
    [RegisterForTrace]
    public class ScheduleView : View
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit element
        /// </summary>
        internal Autodesk.Revit.DB.ViewSchedule InternalViewSchedule
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalViewSchedule; }
        }

        /// <summary>
        /// Reference to Schedule Filters
        /// </summary>
        internal IList<Autodesk.Revit.DB.ScheduleFilter> InternalScheduleFilters
        {
            get;
            private set;
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private ScheduleView(Autodesk.Revit.DB.ViewSchedule view)
        {
            SafeInit(() => InitScheduleView(view), true);
        }

        /// <summary>
        /// Private constructor for creating Schedules from Category/ScheduleType.
        /// </summary>
        private ScheduleView(Category category, string name, ScheduleType type)
        {
            SafeInit(() => InitScheduleView(category, name, type));
        }

        /// <summary>
        /// Private constructor for creating Schedules from AreaScheme.
        /// </summary>
        private ScheduleView(Element areaScheme, string name)
        {
            SafeInit(() => InitScheduleView(areaScheme, name));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a ScheduleView element
        /// </summary>
        private void InitScheduleView(Autodesk.Revit.DB.ViewSchedule view)
        {
            InternalSetScheduleView(view);
        }

        /// <summary>
        /// Initialize a ScheduleView Element by Category, name and ScheduleType.
        /// </summary>
        /// <param name="category">Category of elements that Schedule will display.</param>
        /// <param name="name">Name of the Schedule.</param>
        /// <param name="type">ScheduleType ex: Key Schedule.</param>
        private void InitScheduleView(Category category, string name, ScheduleType type)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            // Get existing view if possible
            var vs = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ViewSchedule>(doc) ?? 
                CreateViewSchedule(category, name, type);

            InternalSetScheduleView(vs);

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.CleanupAndSetElementForTrace(doc, InternalElement);
        }

        /// <summary>
        /// Initialize a ScheduleView Element by AreaScheme and name.
        /// </summary>
        /// <param name="areaScheme">Area Scheme.</param>
        /// <param name="name">Name of the Schedule.</param>
        private void InitScheduleView(Element areaScheme, string name)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            // Get existing view if possible
            var vs = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ViewSchedule>(doc) ??
                     CreateViewSchedule(areaScheme, name);

            InternalSetScheduleView(vs);

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.CleanupAndSetElementForTrace(doc, InternalElement);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the InternalViewSchedule property and the associated element id and unique id
        /// </summary>
        /// <param name="view"></param>
        private void InternalSetScheduleView(Autodesk.Revit.DB.ViewSchedule view)
        {
            InternalViewSchedule = view;
            InternalElementId = view.Id;
            InternalUniqueId = view.UniqueId;
            InternalScheduleFilters = view.Definition.GetFilters();
        }

        #endregion

        #region Private helper methods

        private static Autodesk.Revit.DB.ViewSchedule CreateViewSchedule(Category category, string name, ScheduleType type)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            Autodesk.Revit.DB.ViewSchedule viewSchedule = null;
            switch (type)
            {
                case ScheduleType.KeySchedule:
                    viewSchedule = Autodesk.Revit.DB.ViewSchedule.CreateKeySchedule(doc, new Autodesk.Revit.DB.ElementId(category.Id));
                    viewSchedule.Name = name;
                    break;
                case ScheduleType.RegularSchedule:
                    viewSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc, new Autodesk.Revit.DB.ElementId(category.Id));
                    viewSchedule.Name = name;
                    break;
                case ScheduleType.MaterialTakeoff:
                    viewSchedule = Autodesk.Revit.DB.ViewSchedule.CreateMaterialTakeoff(doc, new Autodesk.Revit.DB.ElementId(category.Id));
                    viewSchedule.Name = name;
                    break;
            }

            TransactionManager.Instance.TransactionTaskDone();

            return viewSchedule;
        }

        private static Autodesk.Revit.DB.ViewSchedule CreateViewSchedule(Element areaScheme, string name)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            var viewSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc,
                new Autodesk.Revit.DB.ElementId(Autodesk.Revit.DB.BuiltInCategory.OST_Areas),
                areaScheme.InternalElement.Id);
            viewSchedule.Name = name;

            TransactionManager.Instance.TransactionTaskDone();

            return viewSchedule;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create Schedule by Category, Type and Name.
        /// </summary>
        /// <param name="category">Category that Schedule will be associated with.</param>
        /// <param name="name">Name of the Schedule View.</param>
        /// <param name="scheduleType">Type of Schedule View to be created. Ex. Key Schedule.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
        public static ScheduleView CreateSchedule(Category category, string name, string scheduleType)
        {
            if (category == null)
            {
                throw new ArgumentNullException(Properties.Resources.CategoryArgumentException);
            }
            if (name == null)
            {
                throw new ArgumentNullException(Properties.Resources.NameArgumentException);
            }

            try
            {
                var t = (ScheduleType)Enum.Parse(typeof(ScheduleType), scheduleType);
                return new ScheduleView(category, name, t);
            }
            catch (Exception)
            {
                throw new ArgumentNullException(Properties.Resources.ViewUnsupportedScheduleType);
            }
        }

        /// <summary>
        /// Create Area Schedule by Area Scheme and Name.
        /// </summary>
        /// <param name="areaScheme">Area Scheme that Schedule will be associated with.</param>
        /// <param name="name">Name of the Schedule View.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
        public static ScheduleView CreateAreaSchedule(Element areaScheme, string name)
        {
            if (areaScheme == null)
            {
                throw new ArgumentNullException(Properties.Resources.AreaSchemeArgumentException);
            }
            if (name == null)
            {
                throw new ArgumentNullException(Properties.Resources.NameArgumentException);
            }

            return new ScheduleView(areaScheme, name);
        }

        /// <summary>
        /// Remove Schedule Field from Schedule View.
        /// </summary>
        /// <param name="fields">Schedule Fields (columns) to be removed.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
        public ScheduleView RemoveFields(List<ScheduleField> fields)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            foreach (var field in fields)
            {
                if (Fields.Any(x => x.Name == field.Name))
                {
                    InternalViewSchedule.Definition.RemoveField(field.InternalScheduleField.FieldId);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Add Field (Column) to Schedule View.
        /// </summary>
        /// <param name="fields">Schedulable Field retrieved from ScheduleView.SchedulableFields.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
        public ScheduleView AddFields(List<SchedulableField> fields)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            foreach (var field in fields)
            {
                if (!Fields.Any(x => x.Name == field.Name))
                {
                    InternalViewSchedule.Definition.AddField(field.InternalSchedulableField);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Export View Schedule to CSV, TSV etc.
        /// </summary>
        /// <param name="path">A valid file path with file extension.</param>
        /// <param name="exportOptions">Export Options.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
        public ScheduleView Export(string path, ScheduleExportOptions exportOptions)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Properties.Resources.View_ExportAsImage_Path_Invalid, "path");
            }
            if (exportOptions == null)
            {
                throw new ArgumentException(Properties.Resources.ExportOptionsArgumentException);
            }

            // Run export
            var folder = System.IO.Path.GetDirectoryName(path);
            var name = System.IO.Path.GetFileName(path);
            try
            {
                InternalViewSchedule.Export(folder, name, exportOptions.InternalScheduleExportOptions);
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.ScheduleExportError, ex);
            }

            return this;
        }

        /// <summary>
        /// Add Schedule Filters to Schedule View.
        /// </summary>
        /// <param name="scheduleFilters">List of Schedule Filters.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
        public ScheduleView AddFilters(List<ScheduleFilter> scheduleFilters)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            var currentFilters = InternalScheduleFilters.Select(x => new ScheduleFilter(x)).ToList();
            foreach (var sf in scheduleFilters)
            {
                if (!currentFilters.Contains(sf))
                {
                    InternalViewSchedule.Definition.AddFilter(sf.InternalScheduleFilter);
                    currentFilters.Add(sf);
                }
            }
            InternalScheduleFilters = currentFilters.Select(x => x.InternalScheduleFilter).ToList();

            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Clear all Schedule Filters from Schedule View.
        /// </summary>
        /// <returns name="scheduleView">Schedule View.</returns>
        public ScheduleView ClearAllFilters()
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            InternalViewSchedule.Definition.ClearFilters();
            InternalScheduleFilters.Clear();

            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Schedule Filters.
        /// </summary>
        public List<ScheduleFilter> ScheduleFilters
        {
            get
            {
                return InternalScheduleFilters.Select(x => new ScheduleFilter(x)).ToList();
            }
        }

        /// <summary>
        /// Schedule Fields.
        /// </summary>
        public List<ScheduleField> Fields
        {
            get
            {
                var fieldIds = InternalViewSchedule.Definition.GetFieldOrder();
                return fieldIds.Select(id => new ScheduleField(InternalViewSchedule.Definition.GetField(id))).ToList();
            }
        }

        /// <summary>
        /// Schedulable Fields.
        /// </summary>
        public List<SchedulableField> SchedulableFields
        {
            get
            {
                var fields = InternalViewSchedule.Definition.GetSchedulableFields();
                return fields.Select(x => new SchedulableField(x)).ToList();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// ScheduleType Enumeration
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public enum ScheduleType
        {
            KeySchedule,
            RegularSchedule,
            MaterialTakeoff
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a View from a user selected Element.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static ScheduleView FromExisting(Autodesk.Revit.DB.ViewSchedule view, bool isRevitOwned)
        {
            return new ScheduleView(view)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
