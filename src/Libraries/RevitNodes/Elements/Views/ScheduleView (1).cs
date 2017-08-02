using RevitServices.Persistence;
using Autodesk.DesignScript.Runtime;
<<<<<<< HEAD
=======
using View = Revit.Elements.Views.View;
>>>>>>> origin/master
using RevitServices.Transactions;
using DynamoServices;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
using System;
using Revit.Schedules;
=======
>>>>>>> origin/master

namespace Revit.Elements.Views
{
    /// <summary>
<<<<<<< HEAD
    /// Base class for Revit Plan views
=======
    ///     Base class for Revit Plan views
>>>>>>> origin/master
    /// </summary>
    [RegisterForTrace]
    public class ScheduleView : View
    {
        #region Internal properties

        /// <summary>
<<<<<<< HEAD
        /// An internal handle on the Revit element
=======
        ///     An internal handle on the Revit element
>>>>>>> origin/master
        /// </summary>
        internal Autodesk.Revit.DB.ViewSchedule InternalViewSchedule
        {
            get;
            private set;
        }

        /// <summary>
<<<<<<< HEAD
        /// Reference to the Element
=======
        ///     Reference to the Element
>>>>>>> origin/master
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalViewSchedule; }
        }

<<<<<<< HEAD
        /// <summary>
        /// Reference to Schedule Filters
        /// </summary>
        internal IList<Autodesk.Revit.DB.ScheduleFilter> InternalScheduleFilters
        {
            get;
            private set;
        }

=======
>>>>>>> origin/master
        #endregion

        #region Private constructors

        /// <summary>
<<<<<<< HEAD
        /// Private constructor
=======
        ///     Private constructor
>>>>>>> origin/master
        /// </summary>
        private ScheduleView(Autodesk.Revit.DB.ViewSchedule view)
        {
            SafeInit(() => InitScheduleView(view));
        }

        /// <summary>
<<<<<<< HEAD
        /// Private constructor
=======
        ///     Private constructor
>>>>>>> origin/master
        /// </summary>
        private ScheduleView(Category category, string name, ScheduleType type)
        {
            SafeInit(() => InitScheduleView(category, name, type));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
<<<<<<< HEAD
        /// Initialize a ScheduleView element
=======
        ///     Initialize a ScheduleView element
>>>>>>> origin/master
        /// </summary>
        private void InitScheduleView(Autodesk.Revit.DB.ViewSchedule view)
        {
            InternalSetScheduleView(view);
        }

        /// <summary>
<<<<<<< HEAD
        /// Initialize a ViewSchedule Element.
=======
        ///     Initialize a ViewSchedule Element.
>>>>>>> origin/master
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        private void InitScheduleView(Category category, string name, ScheduleType type)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(doc);

<<<<<<< HEAD
            // Get existing view if possible
            var vs = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ViewSchedule>(doc);

            if (vs == null)
            {
                vs = CreateViewSchedule(category, name, type);
            }
=======
            var vs = CreateViewSchedule(category, name, type);
>>>>>>> origin/master

            InternalSetScheduleView(vs);

            TransactionManager.Instance.TransactionTaskDone();
<<<<<<< HEAD
=======

>>>>>>> origin/master
            ElementBinder.CleanupAndSetElementForTrace(doc, this.InternalElement);
        }

        #endregion

        #region Private mutators

        /// <summary>
<<<<<<< HEAD
        /// Set the InternalViewSchedule property and the associated element id and unique id
=======
        ///     Set the InternalViewSchedule property and the associated element id and unique id
>>>>>>> origin/master
        /// </summary>
        /// <param name="view"></param>
        private void InternalSetScheduleView(Autodesk.Revit.DB.ViewSchedule view)
        {
            this.InternalViewSchedule = view;
            this.InternalElementId = view.Id;
            this.InternalUniqueId = view.UniqueId;
<<<<<<< HEAD
            this.InternalScheduleFilters = view.Definition.GetFilters();
=======
>>>>>>> origin/master
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
<<<<<<< HEAD
                case ScheduleType.RegularSchedule:
                    viewSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc, new Autodesk.Revit.DB.ElementId(category.Id));
                    viewSchedule.Name = name;
                    break;
                case ScheduleType.MaterialTakeoff:
                    viewSchedule = Autodesk.Revit.DB.ViewSchedule.CreateMaterialTakeoff(doc, new Autodesk.Revit.DB.ElementId(category.Id));
                    viewSchedule.Name = name;
                    break;
=======
>>>>>>> origin/master
            }

            TransactionManager.Instance.TransactionTaskDone();

            return viewSchedule;
        }

        #endregion

        #region Public static constructors

        /// <summary>
<<<<<<< HEAD
        /// Create Schedule by Category, Type and Name.
        /// </summary>
        /// <param name="category">Category that Schedule will be associated with.</param>
        /// <param name="name">Name of the Schedule View.</param>
        /// <param name="scheduleType">Type of Schedule View to be created. Ex. Key Schedule.</param>
        /// <returns name="scheduleView">Schedule View.</returns>
=======
        ///     Create Schedule by Category, Type and Name.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <param name="scheduleType"></param>
        /// <returns name="scheduleView">Schedule</returns>
>>>>>>> origin/master
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
<<<<<<< HEAD
            if (scheduleType == null)
            {
                throw new System.ArgumentNullException(Properties.Resources.ScheduleTypeArgumentException);
            }
=======
>>>>>>> origin/master

            ScheduleType t = (ScheduleType)System.Enum.Parse(typeof(ScheduleType), scheduleType);
            return new ScheduleView(category, name, t);
        }

        /// <summary>
<<<<<<< HEAD
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

=======
        ///     Remove Schedule Field from Schedule.
        /// </summary>
        /// <param name="field"></param>
        /// <returns name="View">View Schedule</returns>
        public ScheduleView RemoveField(Revit.Schedules.ScheduleField field)
        {
            if (this.Fields.Any(x => x.Name == field.Name))
            {
                var doc = DocumentManager.Instance.CurrentDBDocument;
                TransactionManager.Instance.EnsureInTransaction(doc);
                this.InternalViewSchedule.Definition.RemoveField(field.InternalScheduleField.FieldId);
                TransactionManager.Instance.TransactionTaskDone();
            }

>>>>>>> origin/master
            return this;
        }

        /// <summary>
<<<<<<< HEAD
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
=======
        ///     Add Field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public ScheduleView AddField(Revit.Schedules.SchedulableField field)
        {
            if (!this.Fields.Any(x => x.Name == field.Name))
            {
                var doc = DocumentManager.Instance.CurrentDBDocument;
                TransactionManager.Instance.EnsureInTransaction(doc);
                this.InternalViewSchedule.Definition.AddField(field.InternalSchedulableField);
                TransactionManager.Instance.TransactionTaskDone();
            }

            return this;
        }

        /// <summary>
        ///     Schedule Fields.
>>>>>>> origin/master
        /// </summary>
        public List<Revit.Schedules.ScheduleField> Fields
        {
            get
            {
                IList<Autodesk.Revit.DB.ScheduleFieldId> fieldIds = this.InternalViewSchedule.Definition.GetFieldOrder();
                return fieldIds.Select(id => new Revit.Schedules.ScheduleField(this.InternalViewSchedule.Definition.GetField(id))).ToList();
            }
<<<<<<< HEAD
        }

        /// <summary>
        /// Schedulable Fields.
=======
            
        }

        /// <summary>
        ///     Schedulable Fields.
>>>>>>> origin/master
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
<<<<<<< HEAD
        /// ScheduleType Enumeration
=======
        ///     ScheduleType Enumeration
>>>>>>> origin/master
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public enum ScheduleType
        {
<<<<<<< HEAD
            KeySchedule,
            RegularSchedule,
            MaterialTakeoff
=======
            KeySchedule
>>>>>>> origin/master
        }

        #endregion

        #region Internal static constructors

        /// <summary>
<<<<<<< HEAD
        /// Create a View from a user selected Element.
=======
        ///     Create a View from a user selected Element.
>>>>>>> origin/master
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
