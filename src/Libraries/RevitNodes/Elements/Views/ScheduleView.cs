using RevitServices.Persistence;
using Autodesk.DesignScript.Runtime;
using RevitServices.Transactions;
using DynamoServices;
using System.Collections.Generic;
using System.Linq;
using System;
using Revit.Schedules;

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
            SafeInit(() => InitScheduleView(view));
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private ScheduleView(Category category, string name, ScheduleType type)
        {
            SafeInit(() => InitScheduleView(category, name, type));
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
        /// Initialize a ViewSchedule Element.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        private void InitScheduleView(Category category, string name, ScheduleType type)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            // Get existing view if possible
            var vs = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ViewSchedule>(doc);

            if (vs == null)
            {
                vs = CreateViewSchedule(category, name, type);
            }

            InternalSetScheduleView(vs);

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.CleanupAndSetElementForTrace(doc, this.InternalElement);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the InternalViewSchedule property and the associated element id and unique id
        /// </summary>
        /// <param name="view"></param>
        private void InternalSetScheduleView(Autodesk.Revit.DB.ViewSchedule view)
        {
            this.InternalViewSchedule = view;
            this.InternalElementId = view.Id;
            this.InternalUniqueId = view.UniqueId;
            this.InternalScheduleFilters = view.Definition.GetFilters();
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
                throw new System.ArgumentNullException(Properties.Resources.CategoryArgumentException);
            }
            if (name == null)
            {
                throw new System.ArgumentNullException(Properties.Resources.NameArgumentException);
            }
            if (scheduleType == null)
            {
                throw new System.ArgumentNullException(Properties.Resources.ScheduleTypeArgumentException);
            }

            ScheduleType t = (ScheduleType)System.Enum.Parse(typeof(ScheduleType), scheduleType);
            return new ScheduleView(category, name, t);
        }

        /// <summary>
        /// Remove Schedule Field from Schedule View.
        /// </summary>
        /// <param name="fields">Schedule Fields (columns) to be removed.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
        public ScheduleView RemoveFields(List<Revit.Schedules.ScheduleField> fields)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            foreach (var field in fields)
            {
                if (this.Fields.Any(x => x.Name == field.Name))
                {
                    this.InternalViewSchedule.Definition.RemoveField(field.InternalScheduleField.FieldId);
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
        public ScheduleView AddFields(List<Revit.Schedules.SchedulableField> fields)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

            foreach (var field in fields)
            {
                if (!this.Fields.Any(x => x.Name == field.Name))
                {
                    this.InternalViewSchedule.Definition.AddField(field.InternalSchedulableField);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Export View Schedule to CSV, TSV etc.
        /// </summary>
        /// <param name="path">A valid file path with file extension.</param>
        /// <param name="exportOptions">Export Options. If null, default will be used.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
        public ScheduleView Export(
            string path, 
            ScheduleExportOptions exportOptions)
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
            string folder = System.IO.Path.GetDirectoryName(path);
            string name = System.IO.Path.GetFileName(path);
            try
            {
                this.InternalViewSchedule.Export(folder, name, exportOptions.InternalScheduleExportOptions);
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

            var currentFilters = this.InternalScheduleFilters.Select(x => new ScheduleFilter(x)).ToList();
            foreach (var sf in scheduleFilters)
            {
                if (!currentFilters.Contains(sf))
                {
                    this.InternalViewSchedule.Definition.AddFilter(sf.InternalScheduleFilter);
                    currentFilters.Add(sf);
                }
            }
            this.InternalScheduleFilters = currentFilters.Select(x => x.InternalScheduleFilter).ToList();

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

            this.InternalViewSchedule.Definition.ClearFilters();
            this.InternalScheduleFilters.Clear();

            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Schedule Filters.
        /// </summary>
        public List<Revit.Schedules.ScheduleFilter> ScheduleFilters
        {
            get
            {
                return this.InternalScheduleFilters.Select(x => new ScheduleFilter(x)).ToList();
            }
        }

        /// <summary>
        /// Schedule Fields.
        /// </summary>
        public List<Revit.Schedules.ScheduleField> Fields
        {
            get
            {
                IList<Autodesk.Revit.DB.ScheduleFieldId> fieldIds = this.InternalViewSchedule.Definition.GetFieldOrder();
                return fieldIds.Select(id => new Revit.Schedules.ScheduleField(this.InternalViewSchedule.Definition.GetField(id))).ToList();
            }
        }

        /// <summary>
        /// Schedulable Fields.
        /// </summary>
        public List<Revit.Schedules.SchedulableField> SchedulableFields
        {
            get
            {
                IList<Autodesk.Revit.DB.SchedulableField> fields = this.InternalViewSchedule.Definition.GetSchedulableFields();
                return fields.Select(x => new Revit.Schedules.SchedulableField(x)).ToList();
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
